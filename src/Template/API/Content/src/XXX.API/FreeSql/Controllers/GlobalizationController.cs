using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XXX.API.FreeSql.Service;

namespace XXX.API.Controllers
{
    /// <summary>
    /// Globalization
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GlobalizationController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<GlobalizationController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;
        private readonly IGlobalizationService _globalizationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="globalizationService"></param>
        public GlobalizationController(IHostEnvironment hostEnvironment
            , ILogger<GlobalizationController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IGlobalizationService globalizationService)
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
        public async Task<IActionResult> Globalization()
        {
            var d= _hostEnvironment.ApplicationName;
               var message = _localizer["当前时间为"] + $"：{DateTime.Now}";
            return Ok(message);

        }
    }

}
