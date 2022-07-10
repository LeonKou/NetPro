using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;
using NetPro.MQTTClient;
using System.Collections.Concurrent;
using System.NetPro;
using System.Text;

namespace XXX.Plugin.MQTTServer
{
    /// <summary>
    /// MQTT Broker 服务器示例
    /// 端口默认1883
    /// http中间件不会命中mqtt请求，两套管道
    /// </summary>
    public class MQTTServerStartup //: INetProStartup
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        public double Order { get; set; } = int.MaxValue;

        /// <summary>
        /// 服务注入
        /// https://github.com/chkr1011/MQTTnet
        /// https://www.cnblogs.com/night-w/p/14103391.html
        /// https://github.com/SeppPenner/SimpleMqttServer
        /// https://github.com/Atlas-LiftTech/MQTTnet.AspNetCore.AttributeRouting
        /// https://github.com/ifm/iot-core/blob/8a9b451ac0990d9f455525f3a52d5f23856e85ec/tests/ifmIoTCore.NetAdapter.Mqtt.UnitTests/MqttTests2.cs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //services.AddMQTTClient(GetConnectionString);
            var optionsBuilder = new MqttServerOptionsBuilder()
                        .WithDefaultEndpointReuseAddress()
                        .WithDefaultEndpointPort(1883)
                        .WithConnectionValidator(
                         c =>
                         {
                             //比对连接的账户密码与配置是否匹配
                             //c.ClientId //针对于clientid做校验
                             var currentUser = new { UserName = "netpro", Password = "netpro" };

                             if (currentUser == null)
                             {
                                 c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                 c.ReasonString = $"用户不合法";
                                 Console.WriteLine($"用户不存在");
                                 return;
                             }

                             if (c.Username != currentUser.UserName)
                             {
                                 c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                 c.ReasonString = $"用户不合法";
                                 Console.WriteLine($"用户不存在");
                                 return;
                             }

                             if (c.Password != currentUser.Password)
                             {
                                 c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                 c.ReasonString = $"用户不合法";
                                 Console.WriteLine($"密码错误");
                                 return;
                             }

                             c.ReasonCode = MqttConnectReasonCode.Success;
                             Console.ForegroundColor = ConsoleColor.DarkGreen;
                             Console.WriteLine($"客户端[{c.ClientId}]验证通过");
                             Console.ResetColor();
                         })
                        //订阅拦截器
                        .WithSubscriptionInterceptor(
                                                 c =>
                                                 {
                                                     if (c == null) return;
                                                     c.AcceptSubscription = true;
                                                 })
                        //应用程序消息拦截器
                        .WithApplicationMessageInterceptor(
                                                 c =>
                                                 {
                                                     if (c == null) return;
                                                     c.AcceptPublish = true;
                                                 })
                        //clean sesison是否生效
                        .WithPersistentSessions();

            //接收到的所有消息集合
            ConcurrentQueue<MqttApplicationMessage> messages = new ConcurrentQueue<MqttApplicationMessage>();

            //服务端
            IMqttServer mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());

            //客户端断开连接拦截器
            mqttServer.UseClientDisconnectedHandler(OnMqttServerClientDisconnected);

            //服务开始
            mqttServer.StartedHandler = new MqttServerStartedHandlerDelegate(OnMqttServerStarted);
            //服务停止
            mqttServer.StoppedHandler = new MqttServerStoppedHandlerDelegate(OnMqttServerStopped);
            //客户端连接
            mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(OnMqttServerClientConnected);
            //客户端断开连接（此事件会覆盖拦截器）
            mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(OnMqttServerClientDisconnected);
            //客户端订阅
            mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(OnMqttServerClientSubscribedTopic);
            //客户端取消订阅
            mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(OnMqttServerClientUnsubscribedTopic);
            //服务端收到消息
            mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMqttServerApplicationMessageReceived);

            void OnMqttServerStarted(EventArgs e)
            {
                if (mqttServer.IsStarted)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("MQTT服务启动完成！");
                    Console.ResetColor();
                }
            }
            void OnMqttServerStopped(EventArgs e)
            {
                if (!mqttServer.IsStarted)
                {
                    Console.WriteLine("MQTT服务停止完成！");
                }
            }
            void OnMqttServerClientConnected(MqttServerClientConnectedEventArgs e)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"客户端[{e.ClientId}]已连接");
                Console.ResetColor();
            }
            void OnMqttServerClientDisconnected(MqttServerClientDisconnectedEventArgs e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"客户端[{e.ClientId}]已断开连接！");
                Console.ResetColor();
            }
            void OnMqttServerClientSubscribedTopic(MqttServerClientSubscribedTopicEventArgs e)
            {
                Console.WriteLine($"客户端[{e.ClientId}]已成功订阅主题[{e.TopicFilter}]！");
            }
            void OnMqttServerClientUnsubscribedTopic(MqttServerClientUnsubscribedTopicEventArgs e)
            {
                Console.WriteLine($"客户端[{e.ClientId}]已成功取消订阅主题[{e.TopicFilter}]！");
            }
            void OnMqttServerApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
            {
                messages.Enqueue(e.ApplicationMessage);
                //持久化，记录,更丰富操处理...
                if (messages.Count > 2)//例如此处限定只持久化2条
                {
                    messages.TryDequeue(out MqttApplicationMessage removeMqttApplicationMessage);
                    Console.WriteLine($"已移除队列中过期消息{removeMqttApplicationMessage.Topic}");
                }

                Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                Console.WriteLine($"客户端[{e.ClientId}]>> Topic[{e.ApplicationMessage.Topic}] Payload[{Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[] { })}] Qos[{e.ApplicationMessage.QualityOfServiceLevel}] Retain[{e.ApplicationMessage.Retain}]");
            }
        }

        /// <summary>
        /// 请求管道配置
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }

        public IList<ConnectionString> GetConnectionString(IServiceProvider serviceProvider)
        {
            var connector = new List<ConnectionString>();
            connector.Add(new ConnectionString { Key = "2", Value = "clientid=netpro;host=mqtt://192.168.100.187:1883;username=netpro;password=netpro;timeout=5000;keepalive=2;cleansession=true;" });
            return connector;
        }
    }
}
