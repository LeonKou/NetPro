
namespace XXX.Plugin.EasyNetQ
{
    /// <summary>
    /// EasyNetQ示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EasyNetQController : ControllerBase
    {
        private readonly ILogger<EasyNetQController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly IEasyNetQService _easyNetQServicel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="webHelper"></param>
        /// <param name="easyNetQServicel"></param>
        public EasyNetQController(ILogger<EasyNetQController> logger,
            IWebHelper webHelper,
            IEasyNetQService easyNetQServicel)
        {
            _logger = logger;
            _webHelper = webHelper;
            _easyNetQServicel = easyNetQServicel;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="dbKey">rabbitmq别名标识</param>
        /// <param name="stop">是否停止发布消息</param>
        /// <returns></returns>
        [HttpPost("Publishc")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> PublishAsync(string dbKey = "rabbit1", bool stop = false)
        {
            await _easyNetQServicel.PublishAsync(dbKey, stop);
            return Ok();
        }
    }
}
