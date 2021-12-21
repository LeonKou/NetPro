using NetMQ;
using NetMQ.Sockets;
using System.Threading;

namespace XXX.API.Examples.Service
{
    public class NetMqTask : IStartupTask
    {
        public NetMqTask()
        {

        }

        public int Order => 0;

        public void Execute()
        {
            /*//Publish发布者样板代码
            using (var publisher = new PublisherSocket())
            {
                publisher.Bind("tcp://*:5001");

                publisher
                    .SendMoreFrame("A") // Topic
                    .SendFrame(DateTimeOffset.Now.ToString()); // Message
            }
            */

            //ZeroMQ订阅者，订阅者必须在生产者之后启动
            Task.Factory.StartNew(() =>
            {
                using (var subscriber = new SubscriberSocket())
                {
                    subscriber.Connect("tcp://localhost:5001");
                    subscriber.Subscribe("A");
                    //publisher.Close();
                    while (true)
                    {
                        var topic = subscriber.ReceiveFrameString();
                        var msg = subscriber.ReceiveFrameString();
                        Console.WriteLine("From Publisher: {0} {1}", topic, msg);
                    }
                }
            }, TaskCreationOptions.LongRunning);

        }
    }
}
