using Consul;
using MediatR;
using Microsoft.Extensions.Localization;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using XXX.API.FreeSql.Service;
using XXX.API.GlobalizationDemo.Service;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 各种组件使用示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MiddlewareDemoController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<MiddlewareDemoController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;
        private readonly IConsulClient _consulClient;
        private readonly IMediator _mediator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="consulClient"></param>
        /// <param name="mediator"></param>
        public MiddlewareDemoController(IHostEnvironment hostEnvironment
            , ILogger<MiddlewareDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IConsulClient consulClient
            , IMediator mediator
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _localizer = localizer;
            _consulClient = consulClient;
            _mediator = mediator;
        }

        /// <summary>
        /// consul发现服务
        /// </summary>
        [HttpGet("ConsulDiscovery")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> ConsulDiscovery(string serviceName = "XXX.API")
        {
            //以下几种方式都可拿到注册的服务地址
            var result = await _consulClient.Agent.DiscoveryAsync();
            var result1 = await _consulClient.Catalog.DiscoveryAsync(serviceName);
            var result2 = await _consulClient.DiscoveryAsync(serviceName);
            return Ok(new { result, result1, result2 });
        }

        /// <summary>
        /// zeroMQ 发布消息
        /// </summary>
        [HttpGet("ZeroMQPublish")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> ZeroMQPublish(string topc = "A")
        {
            //仅作为发布者样板代码
            using (var publisher = new PublisherSocket())
            {
                publisher.Bind("tcp://*:5001");

                publisher
                    .SendMoreFrame(topc) // Topic
                    .SendFrame(DateTimeOffset.Now.ToString()); // Message
            }
            return Ok();
        }

        /// <summary>
        /// Mediator 示例
        /// </summary>
        [HttpGet("Mediator")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> Mediator()
        {
            //https://ardalis.com/using-mediatr-in-aspnet-core-apps/
            await _mediator.Publish(new MediatorEvent("Hello World"));
            return Ok();
        }
    }
}
