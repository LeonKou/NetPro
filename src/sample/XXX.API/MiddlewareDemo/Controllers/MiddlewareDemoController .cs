using Consul;
using JWT;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

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
        private readonly IBaiduProxy _baiduProxy;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="baiduProxy"></param>
        /// <param name="httpClientFactory"></param>
        public MiddlewareDemoController(IHostEnvironment hostEnvironment
            , ILogger<MiddlewareDemoController> logger
            , IStringLocalizer<NetPro.Globalization.Globalization> localizer
            , IBaiduProxy baiduProxy
            , IHttpClientFactory httpClientFactory
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _localizer = localizer;
            _consulClient = EngineContext.Current.Resolve<IConsulClient>();//避开平台初始化的依赖检查
            _baiduProxy = baiduProxy;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// consul发现服务
        /// </summary>
        [HttpGet("ConsulDiscovery")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> ConsulDiscovery(string serviceName = "XXX.API")
        {            
            //以下几种方式都可拿到注册的服务地址
            var resultAgent = await _consulClient.Agent.DiscoveryAsync();
            var resultCatalog = await _consulClient.Catalog.DiscoveryAsync(serviceName);
            var resultDiscovery = await _consulClient.DiscoveryAsync(serviceName);
            return Ok(new { resultAgent, resultCatalog, resultDiscovery });
        }

        /// <summary>
        /// 远程请求api,此处以请求远程baidu为例
        /// </summary>
        [HttpGet("RemoteRequestApi")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> RemoteRequestApi()
        {
            var result = await _baiduProxy.SharepageAsync("请求query");
            return Ok(result);
        }

        /// <summary>
        /// 远程请求api(原生HttpClient方式)
        /// </summary>
        [HttpPost("RemoteNative")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> RemoteNativeAsync()
        {
            var command = @"
                            show databases";
            var client = _httpClientFactory.CreateClient();

            string name = "root";
            string pwd = "taosdata";
            string token = $"{name}:{pwd}".Base64();
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
            var request = new HttpRequestMessage
            {
                //rest/sql
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:31181/rest/sql"),
                Content = new StringContent(command, Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject<dynamic>(responseString));
            }
            return BadRequest();
        }

        /// <summary>
        /// 随机数生成
        /// </summary>
        [HttpPost("RandomGenerate")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult RandomGenerate()
        {
            var number = RandomNumberGenerator.GetInt32(0, 100);

            return Ok(number);
        }

        /// <summary>
        /// JWT格式化接口,将token字符串格式化为字典项
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("TokenStringToDic")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult TokenStringToDic(string token)
        {
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, urlEncoder);

            try
            {
                JwtHeader header = decoder.DecodeHeader<JwtHeader>(token);
                dynamic body = decoder.DecodeToObject<dynamic>(token);

                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                foreach (var item in body)
                {
                    result.Add(item.Name, item.Value.Value);
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("token有误");
            }
        }

        /// <summary>
        /// 强制GC
        /// </summary>
        /// <param name="generation">代数</param>
        /// <returns></returns>
        [HttpGet("gc")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult GC([FromQuery] int generation = 2)
        {
            var assemblies = AssemblyLoadContext.Default.Assemblies;
            System.GC.Collect(generation);
            return Ok(assemblies.Count());
        }
    }
}
