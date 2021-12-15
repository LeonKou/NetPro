using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;

namespace XXX.Plugin.MongoDB
{
    /// <summary>
    /// Redis 操作示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MongoDBDemoController : ControllerBase
    {
        private readonly ILogger<MongoDBDemoController> _logger;
        private readonly IWebHelper _webHelper;

        public MongoDBDemoController(ILogger<MongoDBDemoController> logger,
            IWebHelper webHelper)
        {
            _logger = logger;
            _webHelper = webHelper;
        }

        [HttpPost("TimeZone")]
        public void TimeZone()
        {
            var localTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var utcTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Console.WriteLine(localTime);
            Console.WriteLine(utcTime);
            //要用TimeZoneInfo对象指定时区时统一用linux用的时区表示法,
            //即大洲/城市名代表，hangfire定时作业时注意
            TimeZoneInfo linuxTimezone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai");
            TimeZoneInfo windowsTimezone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            
            try
            {
                TimeZoneInfo local = TimeZoneInfo.FindSystemTimeZoneById("Local");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            //Local会取本地时区，现在让容器限定了utc,以下方式始终都是取UTC时区，所以
            var dd = TimeZoneInfo.Local;
        }
    }
}
