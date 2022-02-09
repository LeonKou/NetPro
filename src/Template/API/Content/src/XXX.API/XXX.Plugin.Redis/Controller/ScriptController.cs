namespace XXX.Plugin.Redis
{
    /// <summary>
    /// 执行脚本示例(等价于命令行)
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
        /// 执行python脚本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        [HttpPost("ExecutePython")]
        [ProducesResponseType(200)]
        public IActionResult ExecutePython([FromBody] string cmd, string args)
        {
            _redisService.ExecutePython(cmd, args);
            return Ok();
        }
    }
}
