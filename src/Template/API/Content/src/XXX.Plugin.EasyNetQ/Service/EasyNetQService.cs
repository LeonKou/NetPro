using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.EasyNetQ
{
    public interface IEasyNetQService
    {
        Task PublishAsync(string dbKey = "rabbit1");
    }

    public class EasyNetQService : IEasyNetQService
    {
        private readonly IdleBus<IBus> _idbus;
        private readonly EasyNetQMulti _easyNetQMulti;
        public EasyNetQService(IdleBus<IBus> idbus,
             EasyNetQMulti easyNetQMulti)
        {
            _idbus = idbus;
            _easyNetQMulti = easyNetQMulti;
        }

        public async Task PublishAsync(string dbKey= "rabbit1")
        {
            var bus = _idbus.Get(dbKey);
            await bus.PubSub.PublishAsync(new RabbitMessageModel { Text = "this is a message" });
        }
    }

    [Queue("my_queue_name", ExchangeName = "my_exchange_name")]
    public class RabbitMessageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
    }
}
