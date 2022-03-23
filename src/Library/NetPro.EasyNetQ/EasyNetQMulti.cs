using EasyNetQ;
using System;
using System.Linq;
using System.NetPro;

/// <summary>
/// Support easyNetQ multiple connections
/// It is recommended to use this object only for subscribers
/// </summary>
public interface IEasyNetQMulti
{
    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns>return Ibus;
    /// avoid using when used by subscribers;
    /// be sure to use using when used by publishers</returns>
    public IBus Get(string key);

    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns>return Ibus;
    /// avoid using when used by subscribers;
    /// be sure to use using when used by publishers</returns>
    public IBus this[string key]
    {
        get;
    }
}

namespace NetPro.EasyNetQ
{
    /// <summary>
    /// 支持多个rabbitmq server
    /// 不建议直接使用此类，请使用接口
    /// </summary>
    internal class EasyNetQMulti : IEasyNetQMulti
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
                return new();
            }
        }

        /// <summary>
        /// 根据key标识获取rabbitmq对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IBus Get(string key)
        {
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