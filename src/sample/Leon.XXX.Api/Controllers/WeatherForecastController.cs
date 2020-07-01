using Leon.XXX.Domain;
using Leon.XXX.Proxy;
using Microsoft.AspNetCore.Mvc;
using NetPro.Web.Api.Controllers;
using NetPro.RedisManager;
using NetPro.Web.Core.Models;
using Serilog;
using NetPro.Web.Core.Filters;
using NetPro.Sign;
using MaxMind.Db;
using System.Net;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Leon.XXX.Api
{
    /// <summary>
    ///这是controller
    /// </summary>
    [Route("api/v1/[controller]")]     
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly IXXXService _xXXService;
        private readonly IRedisManager _redisManager;

        private IExampleProxy _userApi { get; set; }
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="xXXService"></param>
        /// <param name="userApi"></param>
        //[FromServices]
        public WeatherForecastController(ILogger logger
            , IXXXService xXXService
            , IExampleProxy userApi
            , IRedisManager redisManager)
        {
            _logger = logger;
            _xXXService = xXXService;
            _userApi = userApi;
            _redisManager = redisManager;
        }

        /// <summary>
        /// 测试Redis
        /// </summary>
        /// <param name="gg"></param>
        /// <returns></returns>
        [HttpPost("TestRedis")]
        [ProducesResponseType(200)]
        [ProducesResponseType(200, Type = typeof(XXXAo))]
        [IgnoreSign]
        public IActionResult TestRedis(XXXRequest gg)
        {
            var dd = _redisManager.GetOrCreate<string>("1",func: ()=>
            {
                return "1";
            });
            _logger.Information("这是系统日志");
            //return ToFailResult("", 500);
            var result = _xXXService.GetList();
            //var ss= _userApi.GetGoodsList(1,"66").GetAwaiter().GetResult();
            //测试设置数据库

            //测试自动生成代理请求
            //var resu = _userApi.GetGoodsList(1, "hhhh").GetAwaiter().GetResult();
            //Serilog.Log.Error("这是错误");
            return ToSuccessResult(result);
        }

        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <param name="gg"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("pay/gettest")]
        [ProducesResponseType(200)]
        [ProducesResponseType(200, Type = typeof(XXXAo))]
        public IActionResult GetTest([FromQuery]XXXRequest gg)
        {
            return ResponseResult.ToSuccessResult("");
            var dd = _redisManager.GetOrCreate<string>("");
            _logger.Information("这是系统日志");
            //return ToFailResult("", 500);
            var result = _xXXService.GetList();
            //var ss= _userApi.GetGoodsList(1,"66").GetAwaiter().GetResult();
            //测试设置数据库

            //测试自动生成代理请求
            //var resu = _userApi.GetGoodsList(1, "hhhh").GetAwaiter().GetResult();
            //Serilog.Log.Error("这是错误");
            return ToSuccessResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gg"></param>
        [HttpPost]
        [Route("pay/hh")]
        [ProducesResponseType(200)]
        public void HH([FromForm]FileTestInput gg)
        {
            var d = Request.Body;
        }

        /// <summary>
        /// ip地址转换经纬度
        /// </summary>
        /// <param name="gg"></param>
        /// <returns></returns>
        /// <remarks>ip库地址：https://db-ip.com/db/download/ip-to-city-lite</remarks>
        [HttpGet]
        [Route("toip")]
        [IgnoreSign]
        public IActionResult IpConvert(string ip)
        {
            using (var reader = new Reader("wwwroot/dbip-city-lite-2020-06.mmdb"))
            {
                var ipresult = IPAddress.Parse(ip);
                var data = reader.Find<Dictionary<string, object>>(ipresult);
                return ToSuccessResult(body: data);
            }
        }

        /// <summary>
        /// 生成签名 
        /// </summary>
        /// <returns></returns>
        [HttpGet("createsign")]
        [VerifySign]
        public IActionResult CreateSign()
        {
            Dictionary<string, string> queryDic = new Dictionary<string, string>();
            queryDic.Add("appid", "111");
            queryDic.Add("c", "1");
            queryDic.Add("d", "1");
            var sign = SignCommon.CreateSign("sadfsdf", queryDic: queryDic, body: new { a = 1, b = "1" });
            queryDic.Add("sign", sign);
            return Ok(sign);
        }
    }
}
