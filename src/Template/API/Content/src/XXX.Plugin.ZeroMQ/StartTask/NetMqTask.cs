using NetMQ;
using NetMQ.Sockets;

namespace XXX.Plugin.ZeroMQ.StartTask
{
    /// <summary>
    /// 初始化ZeroMQ订阅者
    /// </summary>
    public class NetMqTask : IStartupTask
    {
        public NetMqTask()
        {

        }

        public int Order => 0;

        public void Execute()
        {
            #region //Publish发布者样板代码

            /*
            Task.Run(() =>
                    {
                        using (var publisher = new PublisherSocket())
                        {   //发布是由于本机承载故配回环地址即可
                            publisher.Bind("tcp://*:81");

                            while (true)
                            {
                                publisher
                               .SendMoreFrame("A") // Topic
                               .SendFrame(DateTimeOffset.Now.ToString()); // Message
                                Thread.Sleep(1000);
                            }
                        }
                    });
            */
            #endregion
            //ZeroMQ订阅者，订阅者必须在生产者之后启动
            Task.Factory.StartNew(() =>
            {
                using (var subscriber = new SubscriberSocket())
                {
                    ////随机端口方式
                    //var port = subscriber.BindRandomPort("tcp://localhost");
                    subscriber.Connect("tcp://localhost:81");
                    subscriber.Subscribe("A");
                    while (true)
                    {
                        var topic = subscriber.ReceiveFrameString();
                        var msg = subscriber.ReceiveFrameString();
                        Console.WriteLine($"发布订阅模式-订阅成功：{topic}-{msg}");
                    }
                }
            }, TaskCreationOptions.LongRunning);

            #region //推数据样板代码
            //https://github.com/zeromq/netmq/blob/ea0a5a7e1b77a1ade9311f187f4ff37a20d5d964/src/NetMQ.Tests/PushPullTests.cs
            /*
                    Task.Run(() =>
                    {
                        using (var pushSocket = new PushSocket())
                        {   //推要指定远程地址，故不能使用回环地址
                            pushSocket.Connect("tcp://localhost:82");
                            while (true)
                            {
                                pushSocket.SendFrame("Hello Clients");
                                Console.WriteLine("Hello Clients");
                                Thread.Sleep(1000);
                            }
                        }
                    });
             */

            #endregion
            //ZeroMQ推拉模式拉取角色，没有启动顺序依赖
            Task.Factory.StartNew(() =>
            {
                using (var pullSocket = new PullSocket())
                {
                    pullSocket.Bind("tcp://localhost:82");
                    while (true)
                    {
                        var topic = pullSocket.ReceiveFrameString();
                        var msg = pullSocket.ReceiveFrameString();
                        Console.WriteLine($"推拉模式-拉取成功：{msg}");
                    }
                }
            }, TaskCreationOptions.LongRunning);

        }
    }
}
