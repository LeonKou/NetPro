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
        private readonly IMqttClientMulti _mqttClientMulti;
        public MQTTClientTask()
        {
            _mqttClientMulti = EngineContext.Current.Resolve<IMqttClientMulti>();
        }

        public int Order => 2;

        public void Execute()
        {
            var filter = new MqttTopicFilter()
            {
                //https://www.hivemq.com/blog/mqtt-client-load-balancing-with-shared-subscriptions/
                //共享标识$queue和$share;
                Topic = "$share/g/netpro",//"家/客厅/空调/#",topic通过/分割主题层级，一般层级由高到低                
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
        }
    }
}
