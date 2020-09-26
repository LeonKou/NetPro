using Leon.XXX.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api.Controllers;

namespace Leon.XXXV2.Api
{
    /// <summary>
    ///这是controller
    /// </summary>
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ApiControllerBase
    {
        private readonly IXXXService _xXXService;

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
            , IXXXService xXXService)
        {
            _logger = logger;
            _xXXService = xXXService;
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
        public IActionResult Get([FromQuery]XXXRequest gg)
        {
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
