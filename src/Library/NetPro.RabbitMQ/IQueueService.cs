using RabbitMQ.Client;
using System.Threading.Tasks;

namespace MQMiddleware
{
    /// <summary>
    /// Custom RabbitMQ queue service interface.
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// RabbitMQ connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// RabbitMQ channel.
        /// </summary>
        IModel Channel { get; }

        /// <summary>
        /// Start consuming (getting messages).
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        void Send<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class;

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendJson(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="secondsDelay">Delay time.</param>
        void SendJson(string json, string exchangeName, string routingKey, int secondsDelay);

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        void SendString(string message, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        void SendString(string message, string exchangeName, string routingKey, int secondsDelay);

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class;

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendJsonAsync(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="secondsDelay">Delay time.</param>
        Task SendJsonAsync(string json, string exchangeName, string routingKey, int secondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendStringAsync(string message, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        Task SendStringAsync(string message, string exchangeName, string routingKey, int secondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="secondsDelay">Delay time.</param>
        Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay);
    }
}