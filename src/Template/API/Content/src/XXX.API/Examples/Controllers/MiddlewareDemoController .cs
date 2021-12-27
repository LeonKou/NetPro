using Consul;
using Microsoft.Extensions.Localization;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="consulClient"></param>
        public MiddlewareDemoController(IHostEnvironment hostEnvironment
            , ILogger<MiddlewareDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IConsulClient consulClient
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _localizer = localizer;
            _consulClient = consulClient;
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
    }
}
