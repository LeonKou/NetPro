using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using System.NetPro;

namespace XXX.Plugin.MQTTServer.StartTask
{
    /// <summary>
    /// MQTTClientTask
    /// </summary>
    public class MQTTClientTask : IStartupTask
    {
        private readonly MqttClientMulti _mqttClientMulti;
        public MQTTClientTask()
        {
            _mqttClientMulti = EngineContext.Current.Resolve<MqttClientMulti>();
        }

        public int Order => 2;

        public void Execute()
        {
            var filter = new MqttTopicFilter()
            {
                //https://www.hivemq.com/blog/mqtt-client-load-balancing-with-shared-subscriptions/
                Topic = "netpro/#",//"家/客厅/空调/#",topic通过/分割主题层级，一般层级由高到低
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
            };

            _Subscribe();
            void _Subscribe()
            {
                var _mqttClient = _mqttClientMulti["1"];
                var result = _mqttClient.SubscribeAsync(filter).ConfigureAwait(false).GetAwaiter().GetResult();
                //消费消息
                _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(arg =>
                {
                    string payload = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                    System.Console.WriteLine("Message received, topic [" + arg.ApplicationMessage.Topic + "], payload [" + payload + "]");
                });
                //重连
                _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async arg =>
                {
                    //只是重连，但是消息需要重新订阅;也可设置CleanSession为false，重连依旧启用之前的订阅。
                    var reconnectResult = await _mqttClient.ReconnectAsync();
                    _Subscribe(); //CleanSession设置为false后，可不必重复订阅。
                });
            }

            Task.Run(() =>
            {
                var _mqttPublishClient = _mqttClientMulti["1"];
                while (true)
                {
                    try
                    {
                        var messagePayload = new MqttApplicationMessageBuilder()
                                         .WithTopic("netpro/local")//共享标识$queue和$share
                                         .WithPayload($"发布消息-{DateTime.Now}")
                                         .WithAtMostOnceQoS()//WithAtMostOnceQoS:Level0;WithAtLeastOnceQoS:Level1;WithExactlyOnceQoS:Level2
                                         .WithRetainFlag(true)//服务器保持消息，有客户端连接此主题的最后一条消息；一个主题保留一条消息；
                                          .Build();           //删除消息既发送一条Payload为0的消息即可。

                        _mqttPublishClient.PublishAsync(messagePayload);
                        Console.WriteLine("消息发布成功");
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"消息发布失败，异常={ex.Message}");
                    }
                }
            });
        }
    }
}
