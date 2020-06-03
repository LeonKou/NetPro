using Leon.XXX.Domain;
using Leon.XXX.Proxy;
using Microsoft.AspNetCore.Mvc;
using NetPro.Web.Api.Controllers;
using NetPro.RedisManager;
using NetPro.Web.Core.Models;
using Serilog;

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
		/// 获取一个查询
		/// </summary>
		/// <param name="gg"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("pay/create")]
		[ProducesResponseType(200)]
		[ProducesResponseType(200, Type = typeof(XXXAo))]
		public IActionResult Get(XXXRequest gg)
        {
           return  ResponseResult.ToSuccessResult("");
			var dd= _redisManager.GetOrCreate<string>("");
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
	}
}
