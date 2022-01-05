using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using NetPro;
using System.NetPro;
using System.Threading;

namespace XXX.Plugin.MQTTServer.StartTask
{
    /// <summary>
    /// MQTTClientTask
    /// </summary>
    public class MQTTClientTask : IStartupTask
    {
        private readonly IdleBus<IMqttClient> _mqttClientIds;
        public MQTTClientTask()
        {
            _mqttClientIds = EngineContext.Current.Resolve<IdleBus<IMqttClient>>();
        }

        public int Order => 0;

        public void Execute()
        {
            var filter = new MqttTopicFilter()
            {
                Topic = "netpro/#",//"家/客厅/空调/#",
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,

            };
            var _mqttClient = _mqttClientIds.Get("1");
            var result = _mqttClient.SubscribeAsync(filter);
            //消费消息
            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(arg =>
            {
                string payload = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                System.Console.WriteLine("Message received, topic [" + arg.ApplicationMessage.Topic + "], payload [" + payload + "]");
            });
        }
    }
}
