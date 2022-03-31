namespace XXX.Plugin.Redis
{
    /// <summary>
    /// Redis 操作示例
    /// </summary>
    /// <remarks>redisjson: https://blog.alumdb.org/using-redisjson/
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class RedisDemoController : ControllerBase
    {
        private readonly ILogger<RedisDemoController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly IRedisService _redisService;

        public RedisDemoController(ILogger<RedisDemoController> logger,
            IWebHelper webHelper
            , IRedisService redisService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _redisService = redisService;
        }

        /// <summary>
        /// Redis插入
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="value">插入值</param>
        /// <param name="dbKey">dbkey</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [HttpPost("Set")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<bool> SetAsync(string key = "one_key", string value = "one_value", string dbKey = "2", int timeSpan = int.MaxValue)
        {
            //除了构造函数获取对象实例也可使用静态的EngineContext来获取对象容器中的指定容器(不鼓励使用，图便利可以使用)
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            var ip = webHelper.GetCurrentIpAddress();
            _logger.LogInformation($"通过EngineContext获取到IP={ip}");
            // 也可通过构造函数获取对象(推荐)
            //var ip = _webHelper.GetCurrentIpAddress();

            var result = await _redisService.SetAsync(key, value, dbKey, TimeSpan.FromMinutes(timeSpan));
            return result;
        }

        /// <summary>
        ///  Redis通过key获取值
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        [HttpGet("Get")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> GetAsync(string key = "one_key", string dbKey = "2")
        {
            var result = await _redisService.GetAsync(key);
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
        public async Task<long> PublishAsync(string channel, string message, string dbKey = "2")
        {
            var result = await _redisService.PublishAsync(channel, message);
            return result;
        }

        /// <summary>
        /// Redis消息订阅(空方法)
        /// 消息订阅由后台自动执行，参考继承了IStartupTask接口的RedisTask实现类
        /// </summary>
        [HttpGet("Subscribe")]
        [ProducesResponseType(200)]
        public IActionResult Subscribe()
        {
            return Ok();
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
        public IActionResult DistributeLock(string lockKey = "lockKey", string dbKey = "2", int timeoutSeconds = 30, bool autoDelay = false)
        {
            var result = _redisService.DistributeLock(lockKey, timeoutSeconds, autoDelay);
            return Ok(result);
        }
    }

}
