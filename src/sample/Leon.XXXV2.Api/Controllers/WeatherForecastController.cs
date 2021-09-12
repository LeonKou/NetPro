
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using Language.Resoureces;
using Leon.XXX.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api.Controllers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leon.XXXV2.Api
{
    /// <summary>
    ///这是controller
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly IXXXService _xXXService;
        private readonly IStringLocalizer<Language.Resoureces.SharedResource> _localizer;

        private IExampleProxy _userApi { get; set; }
        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="xXXService"></param>
        /// <param name="userApi"></param>
        //[FromServices]
        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IXXXService xXXService,
            IStringLocalizer<Language.Resoureces.SharedResource> localizer)
        {
            _logger = logger;
            _xXXService = xXXService;
            _localizer = localizer;
        }

        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <param name="gg"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pay/create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(200, Type = typeof(XXXAo))]
        public async Task<IActionResult> Get([FromQuery] XXXRequest gg)
        {
            return ToSuccessResult(_localizer["BoxLengthRange"] + $"{DateTime.Now}");
            //return ToFailResult("", 500);
            //var result = _xXXService.GetList();
            //var ss= _userApi.GetGoodsList(1,"66").GetAwaiter().GetResult();
            //测试设置数据库

            //测试自动生成代理请求
            //var resu = _userApi.GetGoodsList(1, "hhhh").GetAwaiter().GetResult();
            //Serilog.Log.Error("这是错误");
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("Authorization", $"Bearer {1}");
                return Task.CompletedTask;
            });

            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var channel = GrpcChannel.ForAddress("https://localhost:5001"
                //, new GrpcChannelOptions
                //{
                //    HttpHandler = httpHandler
                //   //,Credentials = ChannelCredentials.Insecure}
                );

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "leon is dad" });
            return ToSuccessResult(_localizer["who are you"] + reply.Message + $"{DateTime.Now}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gg"></param>
        [HttpPost]
        [Route("pay/hh")]
        [ProducesResponseType(200)]
        public void HH([FromForm] FileTestInput gg)
        {
            var d = Request.Body;
        }
    }
}
