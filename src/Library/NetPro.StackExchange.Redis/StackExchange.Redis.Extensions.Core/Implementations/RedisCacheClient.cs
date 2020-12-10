using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NetPro.StackExchange.Redis.StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <inheritdoc/>
    public class RedisCacheClient : IRedisCacheClient
    {
        private readonly IRedisCacheConnectionPoolManager connectionPoolManager;
        private readonly RedisConfiguration redisConfiguration;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memorycache;

        /// <summary>
        /// 初始化一个新的 <see cref="RedisCacheClient"/> 对象
        /// </summary>
        /// <param name="connectionPoolManager"> <see cref="IRedisCacheConnectionPoolManager" />.</param>
        /// <param name="serializer"> <see cref="ISerializer" />.</param>
        /// <param name="redisConfiguration"> <see cref="RedisConfiguration" />.</param>
        public RedisCacheClient(
            IRedisCacheConnectionPoolManager connectionPoolManager,
            ISerializer serializer,
            RedisConfiguration redisConfiguration,
            ILogger<RedisCacheClient> logger,
            IMemoryCache memorycache)
        {
            this.connectionPoolManager = connectionPoolManager;
            Serializer = serializer;
            this.redisConfiguration = redisConfiguration;
            _logger = logger;
            _memorycache = memorycache;
        }

        public IRedisDatabase Db0 => GetDb(0);

        public IRedisDatabase Db1 => GetDb(1);

        public IRedisDatabase Db2 => GetDb(2);

        public IRedisDatabase Db3 => GetDb(3);

        public IRedisDatabase Db4 => GetDb(4);

        public IRedisDatabase Db5 => GetDb(5);

        public IRedisDatabase Db6 => GetDb(6);

        public IRedisDatabase Db7 => GetDb(7);

        public IRedisDatabase Db8 => GetDb(8);

        public IRedisDatabase Db9 => GetDb(9);

        public IRedisDatabase Db10 => GetDb(10);

        public IRedisDatabase Db11 => GetDb(11);

        public IRedisDatabase Db12 => GetDb(12);

        public IRedisDatabase Db13 => GetDb(13);

        public IRedisDatabase Db14 => GetDb(14);

        public IRedisDatabase Db15 => GetDb(15);

        public IRedisDatabase Db16 => GetDb(16);

        public ISerializer Serializer { get; }

        public IRedisDatabase GetDb(int dbNumber, string keyPrefix = null)
        {
            if (redisConfiguration.Enabled.HasValue && !redisConfiguration.Enabled.Value)
            {
                _logger.LogInformation($"Redis已关闭，当前驱动为NullCache!!!");
                return new NullCache();
            }

            if (string.IsNullOrEmpty(keyPrefix))
                keyPrefix = redisConfiguration.KeyPrefix;

            return new RedisDatabase(
                connectionPoolManager,
                Serializer,
                redisConfiguration.ServerEnumerationStrategy,
                dbNumber,
                redisConfiguration.MaxValueLength,
                keyPrefix,
                _logger,
                _memorycache);
        }

        public IRedisDatabase GetDbFromConfiguration()
        {
            return GetDb(redisConfiguration.Database, redisConfiguration.KeyPrefix);
        }
    }
}
