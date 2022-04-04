using MongoDbGenericRepository;
using NetPro.MongoDb;
using System;
using System.Linq;
using System.NetPro;

public interface IMongoDBMulti
{
    /// <summary>
    /// 根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IMongoDbContext Get(string key);

    /// <summary>
    ///  根据key标识获取连接对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IMongoDbContext this[string key]
    {
        get;
    }
}

namespace NetPro.MongoDb
{
    /// <summary>
    /// 建议使用IMongoDBMulti 进行调用
    /// </summary>
    public class MongoDBMulti : IMongoDBMulti
    {
        internal static MongoDbOption MongoDbOption;
        private MongoDBMulti()
        {
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        internal static MongoDBMulti Instance
        {
            get
            {
                return new MongoDBMulti();
            }
        }


        /// <summary>
        /// 根据key标识获取mongodb对象,支持索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IMongoDbContext Get(string key)
        {
            //find mongodb connectionString by key
            return CreateInstanceByKey(key);
        }

        /// <summary>
        /// 根据key标识获取mongodb对象,支持Get方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IMongoDbContext this[string key]
        {
            get
            {
                return CreateInstanceByKey(key);
            }
        }

        /// <summary>
        /// find mongodb connectionString by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static MongoDbContext CreateInstanceByKey(string key)
        {
            //MongoDbOptions mongoDbOptions;
            if (MongoDbOption == null)
            {
                MongoDbOption = EngineContext.Current.Resolve<MongoDbOption>();
            }

            var value = MongoDbOption.ConnectionString.Where(s => s.Key == key).FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Could not find connection string for key = {key}");
            }
            return new(value);
        }
    }
}