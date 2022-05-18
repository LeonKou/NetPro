namespace XXX.Plugin.FreeRedis
{
    /// <summary>
    /// Redis 操作示例
    /// </summary>
    /// <remarks>redisjson: https://blog.alumdb.org/using-redisjson/
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class FreeRedisDemoController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public FreeRedisDemoController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        /// <summary>
        /// Redis插入
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="value">插入值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [HttpPost("Set")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public bool Set(string key = "one_key", string value = "one_value", int timeSpan = int.MaxValue)
        {
            var result = _redisService.Set(key, value, TimeSpan.FromMinutes(timeSpan));
            return result;
        }

        /// <summary>
        ///  Redis通过key获取值
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        [HttpGet("Get")]
        [ProducesResponseType(200, Type = typeof(string))]
        public string Get(string key = "one_key")
        {
            var result = _redisService.Get(key);
            return result;
        }

        /// <summary>
        /// Redis消息发布
        /// </summary>
        /// <param name="channel">发布消息的管道名称</param>
        /// <param name="message">要发布的消息</param>
        /// <returns></returns>
        [HttpPost("Publish")]
        [ProducesResponseType(200, Type = typeof(long))]
        public long Publish(string channel, string message)
        {
            var result = _redisService.Publish(channel, message);
            return result;
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="lockKey">分布式锁的key</param>
        /// <param name="timeoutSeconds">过期时间</param>
        /// <param name="autoDelay">是否自动延长过期时间</param>
        /// <returns></returns>
        [HttpGet("DistributeLock")]
        [ProducesResponseType(200)]
        public IActionResult DistributeLock(string lockKey = "lockKey", int timeoutSeconds = 30, bool autoDelay = false)
        {
            var result = _redisService.DistributeLock(lockKey, timeoutSeconds, autoDelay);
            return Ok(result);
        }
    }

}
