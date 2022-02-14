using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 单程序集开发方式
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TimeZoneDemoController : ControllerBase
    {
        private readonly ILogger<TimeZoneDemoController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        public TimeZoneDemoController(ILogger<TimeZoneDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }


        /// <summary>
        /// 时间戳、时区、时间处理示例
        /// </summary>
        [HttpPost("TimeZone")]
        [ProducesResponseType(200)]
        public IActionResult TimeZone()
        {
            var localTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var utcTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            //localTime 与 utcTime等价，没有时差
            _logger.LogInformation($"Now={localTime}");
            _logger.LogInformation($"UtcNow={utcTime}");

            //要用TimeZoneInfo对象指定时区时统一用linux用的时区表示法,.net6+已同时支持所有操作系统的时区方式，无需使用TimeZoneConverter进行时区处理
            //即大洲/城市名代表，hangfire定时作业时注意
            TimeZoneInfo linuxTimezone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai");
            TimeZoneInfo windowsTimezone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            //Local会取本地时区，现在让容器限定了utc,以下方式始终都是取UTC时区
            var localTimezone = TimeZoneInfo.Local;
            _logger.LogInformation($"linuxTimezone-Asia/Shanghai={linuxTimezone}");
            _logger.LogInformation($"windowsTimezone-China Standard Time={windowsTimezone}");
            _logger.LogInformation($"localTimezone-TimeZoneInfo.Local={localTimezone}");

            //会跟随操作系统时区自动进行转换，接受到的时间字符串和转换后的时间不在同时区会不一样（不推荐使用）
            var dateTime = DateTime.Parse("2021-12-16 12:09:30");
            //不会根据操作系统时区进行处理，接受的时间字符串和转换后的时间一致（推荐使用）
            var dateTimeOffset = DateTimeOffset.Parse("2021-12-16 12:09:30");
            _logger.LogInformation($"dateTime={dateTime}");
            _logger.LogInformation($"dateTimeOffset={dateTimeOffset}");

            //时间戳转带时区的时间字符串
            var DateTimeUnix = DateTimeOffset.FromUnixTimeMilliseconds(1643095933633);
            DateTimeUnix = DateTimeUnix.AddHours(8);
            //输出为 2021/01/20 10:14:23 +00:00
            Console.WriteLine(DateTimeUnix);
            var ddd = new DateTimeOffset(DateTimeUnix.Year, DateTimeUnix.Month, DateTimeUnix.Day, DateTimeUnix.Hour, DateTimeUnix.Minute, DateTimeUnix.Second, TimeSpan.FromHours(8));
            Console.WriteLine(ddd.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            //输出为 2021-01-20T10:14:23+08:00    与上一条没有时差
            return Ok();
        }
    }
}
