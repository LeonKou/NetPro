using EasyNetQ;

namespace XXX.Plugin.EasyNetQ
{
    public interface IEasyNetQService
    {
        Task PublishAsync(string dbKey = "rabbit1", bool stop = false);
    }

    public class EasyNetQService : IEasyNetQService
    {
        private readonly IdleBus<IBus> _idbus;//只能用于发布使用，内部有对象管理机制如用于订阅有连接断开可能的错误。
        private readonly IEasyNetQMulti _easyNetQMulti;//适合用于订阅者使用，无需using包裹保持对象不销毁以达到持续订阅。
        private static bool _stop;
        public EasyNetQService(IdleBus<IBus> idbus,
             IEasyNetQMulti easyNetQMulti)
        {
            _idbus = idbus;
            _easyNetQMulti = easyNetQMulti;
        }

        public async Task PublishAsync(string dbKey = "rabbit1", bool stop = false)
        {
            _idbus.Get(dbKey).PubSub.PublishAsync(new RabbitMessageModel { Text = $"[{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}]this is a message" })
                  .ContinueWith(p =>
                  {
                      if (p.Exception != null)
                          p.Exception.Handle(x =>
                          {
                              Console.WriteLine(x.Message);
                              return true;
                          });
                  });
            //return;
            //await Task.Factory.StartNew(async () =>
            // {
            //     while (true)
            //     {
            //         //单例在while中需要 Thread.Sleep(1)操作，否则会发生内存无法回收的问题。
            //         Thread.Sleep(1);
            //         await _idbus.Get(dbKey).PubSub.PublishAsync(new RabbitMessageModel { Text = $"[{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}]this is a message" })
            //          .ContinueWith(p =>
            //          {
            //              if (p.Exception != null)
            //                  p.Exception.Handle(x =>
            //                  {
            //                      Console.WriteLine(x.Message);
            //                      return true;
            //                  });
            //          });
            //     }
            // });
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
