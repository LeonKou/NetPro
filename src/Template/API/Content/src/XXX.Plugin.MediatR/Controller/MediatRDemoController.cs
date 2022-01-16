using MediatR;
using XXX.Plugin.MediatR.Model;

namespace XXX.Plugin.MediatR.Controllers
{
    /// <summary>
    /// MediatR进程内发布订阅、请求响应模式的示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MediatRDemoController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<MediatRDemoController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="mediator"></param>
        public MediatRDemoController(IHostEnvironment hostEnvironment
            , ILogger<MediatRDemoController> logger
            , IMediator mediator
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// 发布订阅(通知)模式
        /// 场景：数据更改后，通知订阅此数据变化事件的地方进行处理
        /// </summary>
        [HttpPost("Publish")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult Publish()
        {
            //https://ardalis.com/using-mediatr-in-aspnet-core-apps/
            _mediator.Publish(new MediatorEvent("Hello World"));
            return Ok();
        }

        /// <summary>
        /// 请求响应模式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Send")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> Send([FromBody] SendRequestModel request)
        {
            var result = await _mediator.Send<string>(request);
            return Ok(result);
        }
    }
}
