using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ManagerController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ManagerController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ManagerController(ILogger<ManagerController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 主站点Get方法
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> Get()
        {
            var er = EngineContext.Current.Resolve<IWebHelper>();
            var sd = er.GetCurrentIpAddress();
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            _logger.LogInformation("系统调用成功");
            return Ok();
        }
    }

}
