using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pulsar.Client.Api;
using Pulsar.Client.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Pulsar
{
    /// <summary>
    ///
    /// </summary>
    public class PulsarQueneService : IPulsarQueneService
    {
        private readonly Random _random = new Random();
        private readonly PulsarClient _pulsarClient;
        private readonly IPulsarAdminApi _pulsarAdminApi;
        private readonly ILogger<PulsarQueneService> _logger;
        private readonly ConcurrentDictionary<string, IProducer<byte[]>> _producersf;
        private readonly JsonSerializerSettings _serializerSettings;
        public static int _socketCount;

        /// <summary>
        ///
        /// </summary>
        /// <param name="pulsarClient"></param>
        /// <param name="idleBus"></param>
        /// <param name="logger"></param>
        /// <param name="pulsarDotNetClient"></param>
        /// <param name="env"></param>
        /// <param name="pulsarAdminApi"></param>
        public PulsarQueneService(PulsarClient pulsarClient, ILogger<PulsarQueneService> logger, IPulsarAdminApi pulsarAdminApi)
        {
            _pulsarClient = pulsarClient;
            _logger = logger;
            _pulsarAdminApi = pulsarAdminApi;
            _producersf = new ConcurrentDictionary<string, IProducer<byte[]>>();
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// 消息订阅(正则topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumePatternAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .TopicsPattern(topic)
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.ReceiveAsync(state);
                            _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功，【tenantId】{tenantId}");
                            if (func != null)
                            {
                                try
                                {
                                    await func(tenantId, projectId, message.MessageId.TopicName, message.Data);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e.Message);
                                }
                            }
                            await consumer.AcknowledgeAsync(message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 批量消息订阅(正则topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumePatternBatchAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .TopicsPattern(topic)
                        .BatchReceivePolicy(new BatchReceivePolicy())
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.BatchReceiveAsync(state);
                            foreach (var item in message)
                            {
                                _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                                if (func != null)
                                {
                                    try
                                    {
                                        await func(tenantId, projectId, item.MessageId.TopicName, item.Data);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e.Message);
                                    }
                                }
                                await consumer.AcknowledgeAsync(item.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }


        /// <summary>
        /// 消息订阅(完整topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumeAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .Topic(topic)
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.ReceiveAsync(state);
                            _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功，【tenantId】{tenantId}");
                            if (func != null)
                            {
                                try
                                {
                                    await func(tenantId, projectId, message.MessageId.TopicName, message.Data);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e.Message);
                                }
                            }
                            await consumer.AcknowledgeAsync(message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 批量消息订阅(完整topic)
        /// </summary>
        /// <param name="tenantId">租户id</param>
        /// <param name="projectId">项目id</param>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumeBatchAsync(string tenantId, string projectId, string topic, string subscription, Func<string, string, string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .Topic(topic)
                        .BatchReceivePolicy(new BatchReceivePolicy())
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.BatchReceiveAsync(state);
                            foreach (var item in message)
                            {
                                _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                                if (func != null)
                                {
                                    try
                                    {
                                        await func(tenantId, projectId, item.MessageId.TopicName, item.Data);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e.Message);
                                    }
                                }
                                await consumer.AcknowledgeAsync(item.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 消息订阅(正则topic)
        /// </summary>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumePatternAsync( string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .TopicsPattern(topic)
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.ReceiveAsync(state);
                            _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                            if (func != null)
                            {
                                try
                                {
                                    await func(message.MessageId.TopicName, message.Data);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e.Message);
                                }
                            }
                            await consumer.AcknowledgeAsync(message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 批量消息订阅(正则topic)
        /// </summary>
        /// <param name="topic">订阅topic(正则表达式)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumePatternBatchAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .TopicsPattern(topic)
                        .BatchReceivePolicy(new BatchReceivePolicy())
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.BatchReceiveAsync(state);
                            foreach (var item in message)
                            {
                                _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                                if (func != null)
                                {
                                    try
                                    {
                                        await func(item.MessageId.TopicName, item.Data);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e.Message);
                                    }
                                }
                                await consumer.AcknowledgeAsync(item.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }


        /// <summary>
        /// 消息订阅(完整topic)
        /// </summary>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumeAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .Topic(topic)
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.ReceiveAsync(state);
                            _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                            if (func != null)
                            {
                                try
                                {
                                    await func(message.MessageId.TopicName, message.Data);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e.Message);
                                }
                            }
                            await consumer.AcknowledgeAsync(message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 批量消息订阅(完整topic)
        /// </summary>
        /// <param name="topic">订阅topic(完整topic)</param>
        /// <param name="subscription">订阅组</param>
        /// <param name="subscriptionType">订阅类型：默认为Shared</param>
        /// <param name="initialPosition">从首位开始订阅还是最后开始，默认最后</param>
        /// <param name="func">异步委托</param>
        /// <param name="state">CancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task ConsumeBatchAsync(string topic, string subscription, Func<string, byte[], Task> func, CancellationToken state, SubscriptionType subscriptionType = SubscriptionType.Shared, SubscriptionInitialPosition initialPosition = SubscriptionInitialPosition.Latest)
        {
            //每个topic创建一个消费者
            return Task.Run(async () =>
            {
                try
                {
                    var consumer = _pulsarClient.NewConsumer()
                        .SubscriptionName($"{subscription}")
                        .SubscriptionInitialPosition(initialPosition)
                        .SubscriptionType(subscriptionType)
                        .Topic(topic)
                        .BatchReceivePolicy(new BatchReceivePolicy())
                        .SubscribeAsync().Result;

                    while (!state.IsCancellationRequested)
                    {
                        try
                        {
                            var message = await consumer.BatchReceiveAsync(state);
                            foreach (var item in message)
                            {
                                _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费成功");
                                if (func != null)
                                {
                                    try
                                    {
                                        await func(item.MessageId.TopicName, item.Data);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e.Message);
                                    }
                                }
                                await consumer.AcknowledgeAsync(item.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                        finally
                        {
                        }
                    }
                    await consumer.DisposeAsync();
                    _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】消费结束-------{topic}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

            });
        }

        /// <summary>
        /// 发布消息（不等待，提高发送速度，pulsar服务异常时消息会丢失）
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题</param>
        /// <param name="data">消息实例</param>
        /// <param name="producerNum">创建的发送端数量</param>
        /// <returns></returns>
        public Task ProduceMessagesServiceAsync<T>(string topic, T? data, int producerNum = 1)
        {
            Task.Run(async () =>
            {
                if (data == null)
                {
                    return;
                }

                try
                {
                    var producer = GetProducerF(topic, producerNum);
                    if (producer == null)
                    {
                        _logger.LogError($"创建Plusar生产者失败：{topic}");
                        throw new NullReferenceException();
                    }
                    try
                    {
                        var msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, _serializerSettings));
                        await producer.SendAndForgetAsync(msg);
                        _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】发送成功，【topic】{topic},【内容】{System.Text.Encoding.Default.GetString(msg)}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{ex.ToString}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ProduceMessagesServiceAsync：{ex.Message}", ex);
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题</param>
        /// <param name="data">消息实例</param>
        /// <returns></returns>
        public Task ProduceMessagesAsync<T>(string topic, T? data)
        {
            Task.Run(async () =>
            {
                if (data == null)
                {
                    return;
                }
                try
                {
                    var producer = GetProducerF(topic);
                    if (producer == null)
                    {
                        _logger.LogError($"创建Plusar生产者失败：{topic}");
                        throw new NullReferenceException();
                    }
                    try
                    {
                        var msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, _serializerSettings));
                        await producer.SendAsync(msg);
                        _logger.LogInformation($"【{DateTime.Now.ToString("HH:MM:ss fff")}】发送成功，【topic】{topic},【内容】{System.Text.Encoding.Default.GetString(msg)}");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ProduceMessagesAsync：{ex.Message}", ex);
                }
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建生产者
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="producerNum"></param>
        /// <returns></returns>
        private IProducer<byte[]> GetProducerF(string topic, int producerNum = 1)
        {
            producerNum = producerNum > 1 ? _random.Next(1, producerNum) : producerNum;
            return _producersf.AddOrUpdate($"{topic}_{producerNum}", (keys) =>
            {
                try
                {
                    return _pulsarClient.NewProducer().Topic(topic).MaxPendingMessages(10000).MaxPendingMessagesAcrossPartitions(500000).SendTimeout(TimeSpan.FromSeconds(10)).CreateAsync().Result;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Namespace not found", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = topic.Split('/');
                        _pulsarAdminApi.CreateNameSpaceAsync($"/admin/v2/namespaces/{list[2]}/{list[3]}");
                    }
                    _logger.LogError($"{ex.Message}----{topic}");
                    throw;
                }
                return null;
            }, (key, value) =>
            {
                if (value == null)
                {
                    try
                    {
                        return _pulsarClient.NewProducer().Topic(topic).MaxPendingMessages(10000).MaxPendingMessagesAcrossPartitions(500000).SendTimeout(TimeSpan.FromSeconds(10)).CreateAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Namespace not found", StringComparison.OrdinalIgnoreCase))
                        {
                            var list = topic.Split('/');
                            _pulsarAdminApi.CreateNameSpaceAsync($"/admin/v2/namespaces/{list[2]}/{list[3]}");
                        }
                        _logger.LogError($"{ex.Message}----{topic}");
                        throw;
                    }
                    return null;
                }
                else
                {
                    return value;
                }
            });
        }
    }
}
