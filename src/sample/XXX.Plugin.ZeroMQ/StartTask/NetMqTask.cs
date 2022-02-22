using NetMQ;
using NetMQ.Sockets;
using System.NetPro;

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
            //Task.Factory.StartNew(() => {
            //    using (var publisher = new PublisherSocket())
            //    {
            //        //发布是由于本机承载故配回环地址即可
            //        //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
            //        publisher.Bind("tcp://*:81");
            //        while (true)
            //        {
            //            publisher
            //      .SendMoreFrame("A:b") // Topic支持特殊符号，topic命名最佳实践：模块名/功能命/功能层级
            //      .SendFrame(DateTimeOffset.Now.ToString()); // Message
            //        }                   
            //    }
            //});

            //Task.Run(() =>
            //{
            //    using (var pushSocket = new PushSocket())
            //    {
            //        //发布是由于本机承载故配回环地址即可
            //        //发布者优先使用bind方法
            //        pushSocket.Bind("tcp://*:82");
            //        while (true)
            //        {
            //            pushSocket.SendFrame("Hello Clients");
            //            //Console.WriteLine("推送-Hello Clients");
            //        }
            //    }
            //});

            //ZeroMQ订阅者，订阅者与发布者无先后顺序
            Task.Factory.StartNew(() =>
            {
                using (var subscriber = new SubscriberSocket())
                {
                    ////随机端口方式
                    //var port = subscriber.BindRandomPort("tcp://localhost");
                    subscriber.Connect("tcp://localhost:81");
                    //可同时订阅多个主题，
                    //subscriber.Subscribe("A:b:g");//支持/;:符号分割
                    subscriber.Subscribe("A:b");//订阅A:b前缀开头的topic
                    //subscriber.Subscribe("A:c");//订阅A:c前缀开头的topic

                    //subscriber.Subscribe("");//空字符串订阅所有
                    while (true)
                    {
                        var topic = subscriber.ReceiveFrameString();
                        var msg = subscriber.ReceiveFrameString();
                        Console.WriteLine($"发布订阅模式-订阅成功：{topic}-{msg}");
                    }
                }
            }, TaskCreationOptions.LongRunning);
            
            //ZeroMQ推拉模式拉取角色，没有启动顺序依赖
            //拉取模式下多个拉取对象
            Task.Factory.StartNew(() =>
            {
                using (var pullSocket = new PullSocket())
                {
                    //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
                    pullSocket.Connect("tcp://localhost:82");
                    while (true)
                    {
                        var topic = pullSocket.ReceiveFrameString();
                        var msg = pullSocket.ReceiveFrameString();
                        Console.WriteLine($"推拉模式1-拉取成功：{msg}");
                    }
                }
            }, TaskCreationOptions.LongRunning);
            //拉取模式下多个拉取对象
            Task.Factory.StartNew(() =>
            {
                using (var pullSocket = new PullSocket())
                {
                    //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
                    pullSocket.Connect("tcp://localhost:82");
                    while (true)
                    {
                        var topic = pullSocket.ReceiveFrameString();
                        var msg = pullSocket.ReceiveFrameString();
                        Console.WriteLine($"推拉模式2-拉取成功：{msg}");
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
