using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQMiddleware.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQMiddleware
{
    /// <summary>
    /// Implementation of the custom RabbitMQ queue service.
    /// </summary>
    internal class QueueService : IQueueService, IDisposable
    {
        /// <summary>
        /// RabbitMQ connection.
        /// </summary>
        public IConnection Connection => _connection;

        /// <summary>
        /// RabbitMQ channel.
        /// </summary>
        public IModel Channel => _channel;

        EventHandler<BasicDeliverEventArgs> _receivedMessage;
        bool _consumingStarted;

        readonly IDictionary<string, IList<IMessageHandler>> _messageHandlers;
        readonly IDictionary<string, IList<IAsyncMessageHandler>> _asyncMessageHandlers;
        readonly IDictionary<string, IList<INonCyclicMessageHandler>> _nonCyclicHandlers;
        readonly IDictionary<string, IList<IAsyncNonCyclicMessageHandler>> _asyncNonCyclicHandlers;
        readonly IDictionary<Type, List<string>> _routingKeys;
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly ILogger<QueueService> _logger;
        readonly IConnection _connection;
        readonly IModel _channel;
        readonly EventingBasicConsumer _consumer;
        readonly object _lock = new object();

        const int ResendTimeout = 60;
        const int QueueExpirationTime = 60000;

        public QueueService(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicHandlers,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncnonCyclicHandlers,
            IEnumerable<RabbitMqExchange> exchanges,
            IEnumerable<MessageHandlerRouter> routers,
            ILoggerFactory loggerFactory,
            IOptions<RabbitMqClientOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));
            }

            _exchanges = exchanges;

            _routingKeys = TransformMessageHandlerRouters(routers);
            _messageHandlers = TransformMessageHandlersCollection(messageHandlers);
            _asyncMessageHandlers = TransformMessageHandlersCollection(asyncMessageHandlers);
            _nonCyclicHandlers = TransformMessageHandlersCollection(nonCyclicHandlers);
            _asyncNonCyclicHandlers = TransformMessageHandlersCollection(asyncnonCyclicHandlers);

            _logger = loggerFactory.CreateLogger<QueueService>();

            var optionsValue = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = optionsValue.HostName,
                Port = optionsValue.Port,
                UserName = optionsValue.UserName,
                Password = optionsValue.Password,
                VirtualHost = optionsValue.VirtualHost,
                AutomaticRecoveryEnabled = optionsValue.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = optionsValue.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = optionsValue.RequestedConnectionTimeout,
                RequestedHeartbeat = optionsValue.RequestedHeartbeat
            };

            // configuration that the Cloud platform maybe require,this is an example of aliyun
            //factory.AuthMechanisms = new List<AuthMechanismFactory>() { new AliyunMechanismFactory() };//兼容阿里云,非阿里云不需要

            _connection = factory.CreateConnection();
            // Event handling.
            _connection.CallbackException += HandleConnectionCallbackException;
            _connection.ConnectionRecoveryError += HandleConnectionRecoveryError;

            _channel = _connection.CreateModel();
            // Event handling.
            _channel.CallbackException += HandleChannelCallbackException;
            _channel.BasicRecoverOk += HandleChannelBasicRecoverOk;

            _consumer = new EventingBasicConsumer(_channel);
            StartClient();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.CallbackException -= HandleConnectionCallbackException;
                _connection.ConnectionRecoveryError -= HandleConnectionRecoveryError;
            }

            if (_channel != null)
            {
                _channel.CallbackException -= HandleChannelCallbackException;
                _channel.BasicRecoverOk -= HandleChannelBasicRecoverOk;
            }

            if (_channel?.IsOpen == true)
            {
                _channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Start consuming (getting messages).
        /// </summary>
        public void StartConsuming()
        {
            if (_consumingStarted)
            {
                return;
            }

            _consumer.Received += _receivedMessage;
            _consumingStarted = true;

            var consumptionExchanges = _exchanges.Where(x => x.IsConsuming);
            foreach (var exchange in consumptionExchanges)
            {
                foreach (var queue in exchange.Options.Queues)
                {
                    _channel.BasicConsume(queue: queue.Name, autoAck: false, consumer: _consumer);
                }
            }
        }

        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            ValidateArguments(exchangeName, routingKey);
            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void Send<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            Send(@object, deadLetterExchange, delayedQueueName);
        }

        public void SendJson(string json, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void SendJson(string json, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendJson(json, deadLetterExchange, delayedQueueName);
        }

        public void SendString(string message, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        public void SendString(string message, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendString(message, deadLetterExchange, delayedQueueName);
        }

        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            lock (_lock)
            {
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            Send(bytes, properties, deadLetterExchange, delayedQueueName);
        }

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendJsonAsync(string json, string exchangeName, string routingKey) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendJsonAsync(string json, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendStringAsync(string message, string exchangeName, string routingKey) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendStringAsync(string message, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        IBasicProperties CreateProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        IBasicProperties CreateJsonProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        void HandleConnectionCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleChannelBasicRecoverOk(object sender, EventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogInformation("Connection has been reestablished.");
        }

        void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
        }

        IDictionary<Type, List<string>> TransformMessageHandlerRouters(IEnumerable<MessageHandlerRouter> routers)
        {
            var dictionary = new Dictionary<Type, List<string>>();
            foreach (var router in routers)
            {
                if (dictionary.ContainsKey(router.Type))
                {
                    dictionary[router.Type] = dictionary[router.Type].Union(router.RoutingKeys).ToList();
                }
                else
                {
                    dictionary.Add(router.Type, router.RoutingKeys);
                }
            }
            return dictionary;
        }

        IDictionary<string, IList<T>> TransformMessageHandlersCollection<T>(IEnumerable<T> messageHandlers)
        {
            var dictionary = new Dictionary<string, IList<T>>();
            foreach (var handler in messageHandlers)
            {
                var type = handler.GetType();
                foreach (var routingKey in _routingKeys[type])
                {
                    if (dictionary.ContainsKey(routingKey))
                    {
                        if (!dictionary[routingKey].Any(x => x.GetType() == handler.GetType()))
                        {
                            dictionary[routingKey].Add(handler);
                        }
                    }
                    else
                    {
                        dictionary.Add(routingKey, new List<T> { handler });
                    }
                }
            }
            return dictionary;
        }

        void StartClient()
        {
            _receivedMessage = (sender, @event) =>
            {
                var message = Encoding.UTF8.GetString(@event.Body);

                _logger.LogInformation($"New message was received with deliveryTag {@event.DeliveryTag}.");
                _logger.LogInformation(message);

                try
                {
                    if (_asyncMessageHandlers.ContainsKey(@event.RoutingKey))
                    {
                        var tasks = new List<Task>();
                        foreach (var handler in _asyncMessageHandlers[@event.RoutingKey])
                        {
                            tasks.Add(RunAsyncHandler(handler, message, @event.RoutingKey));
                        }
                        Task.WaitAll(tasks.ToArray());
                    }
                    if (_messageHandlers.ContainsKey(@event.RoutingKey))
                    {
                        foreach (var handler in _messageHandlers[@event.RoutingKey])
                        {
                            _logger.LogDebug($"Starting processing the message by message handler {handler?.GetType().Name}.");
                            handler.Handle(message, @event.RoutingKey);
                            _logger.LogDebug($"The message has been processed by message handler {handler?.GetType().Name}.");
                        }
                    }
                    if (_asyncNonCyclicHandlers.ContainsKey(@event.RoutingKey))
                    {
                        var tasks = new List<Task>();
                        foreach (var handler in _asyncNonCyclicHandlers[@event.RoutingKey])
                        {
                            tasks.Add(RunAsyncNonCyclicHandler(handler, message, @event.RoutingKey));
                        }
                        Task.WaitAll(tasks.ToArray());
                    }
                    if (_nonCyclicHandlers.ContainsKey(@event.RoutingKey))
                    {
                        foreach (var handler in _nonCyclicHandlers[@event.RoutingKey])
                        {
                            _logger.LogDebug($"Starting processing the message by non-cyclic message handler {handler?.GetType().Name}.");
                            handler.Handle(message, @event.RoutingKey, this);
                            _logger.LogDebug($"The message has been processed by non-cyclic message handler {handler?.GetType().Name}.");
                        }
                    }
                    _logger.LogInformation($"Success message with deliveryTag {@event.DeliveryTag}.");
                    Channel.BasicAck(@event.DeliveryTag, false);
                }
                catch (Exception exception)
                {
                    _logger.LogError(new EventId(), exception, $"An error occurred while processing received message with delivery tag {@event.DeliveryTag}.");


                    if (@event.BasicProperties.Headers is null)
                    {
                        @event.BasicProperties.Headers = new Dictionary<string, object>();
                    }

                    var exchange = _exchanges.FirstOrDefault(x => x.Name == @event.Exchange);
                    if (exchange is null)
                    {
                        _logger.LogError($"Could not detect exchange {@event.Exchange} to detect the necessity of resending the failed message.");
                        return;
                    }

                    if (exchange.Options.ConsumeFailedAction == ConsumeFailedAction.RETRY)
                    {
                        Channel.BasicNack(@event.DeliveryTag, false, true);
                        _logger.LogInformation("The failed message has been requeued.");
                    }
                    else if (exchange.Options.ConsumeFailedAction == ConsumeFailedAction.RETRY_ONCE
                        && !string.IsNullOrEmpty(exchange.Options.DeadLetterExchange)
                        && !@event.BasicProperties.Headers.ContainsKey("requeued"))
                    {
                        Channel.BasicAck(@event.DeliveryTag, false);
                        @event.BasicProperties.Headers.Add("requeued", true);
                        Send(@event.Body, @event.BasicProperties, @event.Exchange, @event.RoutingKey, ResendTimeout);
                        _logger.LogInformation("The failed message has been requeued once.");
                    }
                    else
                    {
                        Channel.BasicAck(@event.DeliveryTag, false);
                        _logger.LogInformation("The failed message would not be requeued.");
                    }
                }
            };

            var deadLetterExchanges = _exchanges
                .Where(x => !string.IsNullOrEmpty(x.Options.DeadLetterExchange))
                .Select(x => x.Options.DeadLetterExchange)
                .Distinct();

            foreach (var exchangeName in deadLetterExchanges)
            {
                StartDeadLetterExchange(exchangeName);
            }

            foreach (var exchange in _exchanges)
            {
                StartExchange(exchange);
            }
        }

        async Task RunAsyncHandler(IAsyncMessageHandler handler, string message, string routingKey)
        {
            _logger.LogDebug($"Starting processing the message by async message handler {handler?.GetType().Name}.");
            await handler.Handle(message, routingKey);
            _logger.LogDebug($"The message has been processed by async message handler {handler?.GetType().Name}.");
        }

        async Task RunAsyncNonCyclicHandler(IAsyncNonCyclicMessageHandler handler, string message, string routingKey)
        {
            _logger.LogDebug($"Starting processing the message by async non-cyclic message handler {handler?.GetType().Name}.");
            await handler.Handle(message, routingKey, this);
            _logger.LogDebug($"The message has been processed by async non-cyclic message handler {handler?.GetType().Name}.");
        }

        void StartDeadLetterExchange(string exchangeName) =>
            _channel.ExchangeDeclare(
                exchange: exchangeName,
                type: "direct",
                durable: true,
                autoDelete: false,
                arguments: null);

        void StartExchange(RabbitMqExchange exchange)
        {
            _channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Options.Type,
                durable: exchange.Options.Durable,
                autoDelete: exchange.Options.AutoDelete,
                arguments: exchange.Options.Arguments);

            foreach (var queue in exchange.Options.Queues)
            {
                StartQueue(queue, exchange.Name);
            }
        }

        void StartQueue(RabbitMqQueueOptions queue, string exchangeName)
        {
            _channel.QueueDeclare(queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: queue.Arguments);

            if (queue.RoutingKeys.Count > 0)
            {
                // If there are not any routing keys then make a bind with a queue name.
                foreach (var route in queue.RoutingKeys)
                {
                    _channel.QueueBind(
                        queue: queue.Name,
                        exchange: exchangeName,
                        routingKey: route);
                }
            }
            else
            {
                _channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.Name);
            }
        }

        void ValidateArguments(string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));
            }
            if (string.IsNullOrEmpty(routingKey))
            {
                _logger.LogWarning($"Argument {nameof(routingKey)} is null or empty,{ nameof(routingKey)} ");
                //throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));
            }

            var deadLetterExchanges = _exchanges.Select(x => x.Options.DeadLetterExchange).Distinct();
            if (!_exchanges.Any(x => x.Name == exchangeName) && !deadLetterExchanges.Any(x => x == exchangeName))
            {
                var exchanges = string.Join(';', _exchanges.Select(s => s.Name).ToList());
                throw new ArgumentException($"Exchange  {exchangeName}  has not been declared yet.  exchanges:{exchanges}", nameof(exchangeName));
            }
        }

        string GetDeadLetterExchange(string exchangeName)
        {
            var exchange = _exchanges.FirstOrDefault(x => x.Name == exchangeName);
            if (string.IsNullOrEmpty(exchange.Options.DeadLetterExchange))
            {
                throw new ArgumentException($"Exchange {nameof(exchangeName)} has not been configured with a dead letter exchange.", nameof(exchangeName));
            }

            return exchange.Options.DeadLetterExchange;
        }

        string DeclareDelayedQueue(string exchange, string deadLetterExchange, string routingKey, int secondsDelay)
        {
            var delayedQueueName = $"{routingKey}.delayed.{secondsDelay}";
            var arguments = CreateArguments(exchange, routingKey, secondsDelay);

            _channel.QueueDeclare(
                queue: delayedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments);

            _channel.QueueBind(
                queue: delayedQueueName,
                exchange: deadLetterExchange,
                routingKey: delayedQueueName);
            return delayedQueueName;
        }

        static Dictionary<string, object> CreateArguments(string exchangeName, string routingKey, int secondsDelay) =>
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key", routingKey },
                { "x-message-ttl", secondsDelay * 1000 },
                { "x-expires", secondsDelay * 1000 + QueueExpirationTime }
            };
    }
}