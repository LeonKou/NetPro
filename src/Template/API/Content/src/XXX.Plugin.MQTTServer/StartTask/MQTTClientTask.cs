using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using NetPro;
using NetPro.MQTTClient;
using System.NetPro;
using System.Threading;

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
                Topic = "netpro/#",//"家/客厅/空调/#",
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,

            };
            var _mqttClient = _mqttClientMulti["1"];
            var result = _mqttClient.SubscribeAsync(filter).ConfigureAwait(false).GetAwaiter().GetResult();
            //消费消息
            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(arg =>
            {
                string payload = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                System.Console.WriteLine("Message received, topic [" + arg.ApplicationMessage.Topic + "], payload [" + payload + "]");
            });

            Task.Run(() =>
            {
                var _mqttPublishClient = _mqttClientMulti["1"];
                while (true)
                {
                    var messagePayload = new MqttApplicationMessageBuilder()
                                         .WithTopic("netpro")
                                         .WithPayload("发布消息")
                                         .WithExactlyOnceQoS()
                                         .WithRetainFlag(true)
                                         .Build();
                    _mqttPublishClient.PublishAsync(messagePayload);
                    Console.WriteLine("消息发布成功");
                    Thread.Sleep(2000);
                }
            });
        }
    }
}
