using FreeRedis;

namespace XXX.Plugin.FreeRedis
{
    public interface IRedisService
    {
        int DistributeLock(string lockKey, int timeoutSeconds = 30, bool autoDelay = false);
        string Get(string key = "one_key");
        long Publish(string channel, string message);
        bool Set(string key, string value, TimeSpan timeSpan);
    }

    public class RedisService : IRedisService
    {
        private readonly ILogger<RedisService> _logger;
        private readonly RedisClient _redisClient;

        private static int _tempInt = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="redisIdleBus"></param>
        public RedisService(ILogger<RedisService> logger, IdleBus<RedisClient> redisIdleBus)
        {
            _logger = logger;
            // FreeRedis原生对象，以最原生方式调用，支持操作多个redis库；_redisIdleBus.Get("别名")
            _redisClient = redisIdleBus.Get("2");
        }

        /// <summary>
        /// Redis插入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan timeSpan)
        {
            //remark:
            //一秒后过期
            TimeSpan.FromSeconds(1);
            //一小时后过期
            TimeSpan.FromHours(1);

            _redisClient.Set(key, value, timeSpan);
            //var succeed = await _redisManager.SetAsync(key, value, timeSpan,"2");
            //var succeed1 = await _redisManager.SetAsync(key, value, timeSpan,"1");
            _logger.LogInformation("redis 插入成功 ");
            return true;
        }

        /// <summary>
        /// 通过key或者Redis值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            //可随时改用原生对象操作redis
            var value = _redisClient.Get<string>(key);
            _logger.LogInformation("redis 查询成功 ");
            return value;
        }

        /// <summary>
        /// 消息发布
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public long Publish(string channel, string message)
        {
            var value = _redisClient.Publish(channel, message);
            _logger.LogInformation("redis 消息发布 ");
            return value;
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="lockKey">指定锁的key</param>
        /// <param name="timeoutSeconds">超时秒数，超过时间自动释放锁</param>
        /// <param name="autoDelay">是否需要自动延时,true:自动延长过期时间false:不延长过期时间，以设置的过期时间为准</param>
        /// <returns></returns>
        public int DistributeLock(string lockKey, int timeoutSeconds = 30, bool autoDelay = false)
        {
            //通过别名为1的redis库进行分布式锁
            using (_redisClient.Lock(lockKey, timeoutSeconds, autoDelay))
            {
                //被锁住的逻辑
                _logger.LogInformation($"分布式锁的当前值---{_tempInt++}");
                return _tempInt;
            }
        }
    }
}
