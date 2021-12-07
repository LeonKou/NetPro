using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 单程序集开发方式： RedisController 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<RedisController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        public RedisController(ILogger<RedisController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// 单程序集开发方式：多语言国际化示例
        /// </summary>
        [HttpGet("Globalization")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> Globalization()
        {
            var message = _localizer["当前时间为"] + $"：{DateTime.Now}";
            return Ok(message);

        }
    }

}
