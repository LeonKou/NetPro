using EasyNetQ;

namespace XXX.Plugin.EasyNetQ
{
    /// <summary>
    /// rabbitmq消费者
    /// </summary>
    internal class RabbitMqStartTask : IStartupTask
    {
        private readonly IEasyNetQMulti _easyNetQMulti;
        public RabbitMqStartTask()
        {
            _easyNetQMulti = EngineContext.Current.Resolve<IEasyNetQMulti>();
        }
        public int Order => 0;

        public void Execute()
        {
            //订阅/Subscribe
            //订阅需要保持长连接，请使用EasyNetQMulti获取连接对象并且不用使用using和调用dispose()
            var bus = _easyNetQMulti["2"];
            //同交换机，同队列下subscriptionId为订阅唯一标识，相同标识会依次收到订阅消息，类似于广播
            bus.PubSub.Subscribe<RabbitMessageModel>("subscriptionId_", tm =>
            {
                Console.WriteLine("Recieve Message: {0}", tm.Text);
            });
            //其他订阅方式介绍 https://www.likecs.com/show-305311908.html
        }
    }
}
