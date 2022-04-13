using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System.NetPro;
using System.Text;

namespace XXX.Plugin.MQTTServer.StartTask
{
    /// <summary>
    /// MQTTClientTask
    /// </summary>
    public class MQTTServerTask //: IStartupTask
    {
        public MQTTServerTask()
        {
        }

        public int Order => 0;

        public void Execute()
        {
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
            List<MqttApplicationMessage> messages = new List<MqttApplicationMessage>();

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
                Console.WriteLine($"[mqttserver]客户端[{e.ClientId}]已断开连接！");
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
                //持久化，记录,更丰富操处理...
                messages.Add(e.ApplicationMessage);
                Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                Console.WriteLine(@$"客户端[{e.ClientId}]>> Topic[{e.ApplicationMessage.Topic}]
                                    Payload[{Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? new byte[] { })}]
                                    Qos[{e.ApplicationMessage.QualityOfServiceLevel}] 
                                    Retain[{e.ApplicationMessage.Retain}]");
            }
        }
    }
}
