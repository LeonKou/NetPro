using Consul;
using Microsoft.Extensions.Localization;
using XXX.API.FreeSql.Service;

namespace XXX.API.Controllers
{
    /// <summary>
    /// Globalization多语言实例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConsulDemoController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ConsulDemoController> _logger;
        private readonly IStringLocalizer<NetPro.Globalization.Globalization> _localizer;
        private readonly IConsulClient _consulClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="consulClient"></param>
        public ConsulDemoController(IHostEnvironment hostEnvironment
            , ILogger<ConsulDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IConsulClient consulClient)
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _localizer = localizer;
            _consulClient = consulClient;
        }

        /// <summary>
        /// consul发现服务
        /// </summary>
        [HttpGet("DiscoveryServices")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> DiscoveryServices(string serviceName = "XXX.API")
        {
            //以下几种方式都可拿到注册的服务地址
            var result = await _consulClient.Agent.DiscoveryAsync();
            var result1 = await _consulClient.Catalog.DiscoveryAsync(serviceName);
            var result2 = await _consulClient.DiscoveryAsync(serviceName);
            return Ok(new { result, result1, result2 });
        }
    }
}
