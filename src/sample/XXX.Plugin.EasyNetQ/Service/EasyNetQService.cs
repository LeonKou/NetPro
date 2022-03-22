using EasyNetQ;

namespace XXX.Plugin.EasyNetQ
{
    public interface IEasyNetQService
    {
        Task PublishAsync(string dbKey = "rabbit1", bool stop = false);
    }

    public class EasyNetQService : IEasyNetQService
    {
        private readonly IdleBus<IBus> _idbus;
        private readonly IEasyNetQMulti _easyNetQMulti;
        private static bool _stop;
        public EasyNetQService(IdleBus<IBus> idbus,
             IEasyNetQMulti easyNetQMulti)
        {
            _idbus = idbus;
            _easyNetQMulti = easyNetQMulti;
        }

        public async Task PublishAsync(string dbKey = "rabbit1", bool stop = false)
        {
            _stop = stop;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_stop)
                    {
                        return;
                    }
                    Task.Delay(300);
                    using (var bus2 = _easyNetQMulti[dbKey])
                    {
                        try
                        {
                            bus2.PubSub.PublishAsync(new RabbitMessageModel { Text = $"[{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}]this is a message" });
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            });
        }
    }

    [Queue("my_queue_name", ExchangeName = "my_exchange_name_")]
    public class RabbitMessageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }
    }
}
