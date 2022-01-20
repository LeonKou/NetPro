using EasyNetQ;
using System.Linq;
namespace System.NetPro
{
    /// <summary>
    /// 支持多个rabbitmq server ，不自动销毁
    /// </summary>
    public class EasyNetQMulti
    {
        internal static EasyNetQOption EasyNetQOption;
        private EasyNetQMulti()
        {
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        internal static EasyNetQMulti Instance
        {
            get
            {
                return new EasyNetQMulti();
            }
        }

        /// <summary>
        /// 根据key标识获取rabbitmq对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IBus Get(string key)
        {
            //find mongodb connectionString by key
            return CreateInstanceByKey(key);
        }

        /// <summary>
        /// 根据key标识获取rabbitmq对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IBus this[string key]
        {
            get
            {
                return CreateInstanceByKey(key);
            }
        }

        /// <summary>
        /// find rabbitmq connectionString by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static IBus CreateInstanceByKey(string key)
        {
            if (EasyNetQOption == null)
            {
                EasyNetQOption = EngineContext.Current.Resolve<EasyNetQOption>();
            }

            var connectionString = EasyNetQOption.ConnectionString.Where(s => s.Key == key).FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"Could not find connection string for key = {key}");
            }
            var bus = RabbitHutch.CreateBus(connectionString);
            return bus;
        }
    }
}