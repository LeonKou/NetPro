using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.Web.Demo.Controller
{
    /// <summary>
    /// Redis 操作示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ScriptController : ControllerBase
    {
        private readonly ILogger<ScriptController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly IRedisService _redisService;

        public ScriptController(ILogger<ScriptController> logger,
            IWebHelper webHelper
            , IRedisService redisService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _redisService = redisService;
        }

        /// <summary>
        /// 执行python
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost("ExecutePython")]
        [ProducesResponseType(200)]
        public IActionResult ExecutePython([FromBody]string cmd, string args)
        {
            _redisService.ExecutePython(cmd, args);
            return Ok();
        }
    }
}
