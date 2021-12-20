using Microsoft.Extensions.Localization;
using XXX.API.FreeSql.Service;

namespace XXX.API.Controllers
{
    /// <summary>
    /// Globalization多语言实例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GlobalizationDemoController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<GlobalizationDemoController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;
        private readonly IGlobalizationDemoService _globalizationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="globalizationService"></param>
        public GlobalizationDemoController(IHostEnvironment hostEnvironment
            , ILogger<GlobalizationDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IGlobalizationDemoService globalizationService)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _localizer = localizer;
            _globalizationService = globalizationService;
        }

        /// <summary>
        /// 单程序集开发方式：多语言国际化示例
        /// </summary>
        [HttpGet("Globalization")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult Globalization()
        {
            var applicationName = _hostEnvironment.ApplicationName;
            var message = _localizer["当前时间为"] + $"：{DateTime.Now}";
            return Ok(message);
        }
    }
}
