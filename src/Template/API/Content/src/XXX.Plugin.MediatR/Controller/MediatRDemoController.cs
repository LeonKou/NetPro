using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NetPro;
using System.Threading;
using XXX.Plugin.MediatR;

namespace XXX.Plugin.MediatR.Controllers
{
    /// <summary>
    /// 各种组件使用示例
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
        /// Mediator 示例
        /// </summary>
        [HttpGet("Mediator")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult Mediator()
        {
            //https://ardalis.com/using-mediatr-in-aspnet-core-apps/
            _mediator.Publish(new MediatorEvent("Hello World"));
            return Ok();
        }
    }
}
