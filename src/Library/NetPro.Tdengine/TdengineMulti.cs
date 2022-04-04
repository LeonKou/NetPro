using Maikebing.Data.Taos;
using NetPro.Tdengine;
using System;
using System.Linq;
using System.NetPro;

public interface ITdengineMulti
{
    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TaosConnection Get(string key);

    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TaosConnection this[string key]
    {
        get;
    }
}

namespace NetPro.Tdengine
{
    /// <summary>
    /// 不推荐直接使用，请使用增强方式，后续会改为internal
    /// </summary>
    public class TdengineMulti : ITdengineMulti
    {
        internal static TdengineOption TdengineOption;
        private TdengineMulti()
        {
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        internal static TdengineMulti Instance
        {
            get
            {
                return new();
            }
        }

        /// <summary>
        /// 根据key标识获取连接对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TaosConnection Get(string key)
        {
            //find tdengine connectionString by key
            return CreateInstanceByKey(key);
        }

        /// <summary>
        /// 根据key标识获取连接对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TaosConnection this[string key]
        {
            get
            {
                return CreateInstanceByKey(key);
            }
        }

        /// <summary>
        /// find tdengine connectionString by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static TaosConnection CreateInstanceByKey(string key)
        {
            if (TdengineOption == null)
            {
                TdengineOption = EngineContext.Current.Resolve<TdengineOption>();
            }

            var value = TdengineOption.ConnectionString.Where(s => s.Key == key).FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Could not find tdengine connection string for key = {key}");
            }
            return new(value);
        }
    }
}