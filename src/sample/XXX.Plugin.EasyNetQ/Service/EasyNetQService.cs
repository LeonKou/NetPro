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

        public async Task PublishAsync(string dbKey = "rabbit1")
        {
            Task.Factory.StartNew(() =>
            {
                var bus2 = _easyNetQMulti[dbKey];
                while (true)
                {
                    Task.Delay(300);
                    //using (var bus2 = _easyNetQMulti[dbKey])
                    {
                        bus2.PubSub.PublishAsync(new RabbitMessageModel { Text = "this is a message" });
                    }
                }
            }, TaskCreationOptions.LongRunning);
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
