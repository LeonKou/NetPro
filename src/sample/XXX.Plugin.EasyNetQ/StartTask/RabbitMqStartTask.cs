using EasyNetQ;
using NetPro.EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.EasyNetQ
{
    /// <summary>
    /// rabbitmq消费者
    /// </summary>
    internal class RabbitMqStartTask : IStartupTask
    {
        private readonly EasyNetQMulti _easyNetQMulti;
        public RabbitMqStartTask()
        {
            _easyNetQMulti = EngineContext.Current.Resolve<EasyNetQMulti>();
        }
        public int Order => 0;

        public void Execute()
        {
            //订阅/Subscribe
            //订阅需要保持长连接，请使用EasyNetQMulti获取连接对象，_idbus会在指定时间内销毁对象导致可能导致无法消费
            var bus = _easyNetQMulti["rabbit1"];
            bus.PubSub.Subscribe<RabbitMessageModel>("subscriptionId", tm =>
            {
                Console.WriteLine("Recieve Message: {0}", tm.Text);
            });
        }
    }
}
