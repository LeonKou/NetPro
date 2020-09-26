using AutoMapper;
using Leon.XXX.Domain.XXX.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Sign;
using NetPro.Web.Core.Filters;
using NetPro.Web.Core.Models;
using System.Web;

namespace Leon.XXX.Api
{
    /// <summary>
    /// 签名范例
    /// 路由推荐规则:
    /// api/项目名称/接口版本
    /// </summary>
    /// <remarks>
    /// 推荐路由好处：所有项目使用同一域名的情况下根据项目名称确定实际地址，方便管理域；
    /// 版本号可在不停服情况下更新版本并做到兼容老版本
    /// </remarks>
    [Route("api/microservice/v1/[controller]")]
    public class SignController : ControllerBase
    {
        private readonly IRedisOptionService _redisOptionService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="redisOptionService"></param>
        /// <param name="mapper"></param>
        public SignController(
            ILogger<DatabaseCurdController> logger
            , IRedisOptionService redisOptionService
            , IMapper mapper)
        {
            _logger = logger;
            _redisOptionService = redisOptionService;
            _mapper = mapper;
        }

        /// <summary>
        /// 生成签名
        /// 签名最佳实践放于Query中
        /// </summary>
        /// <returns></returns>
        [HttpGet("createsign")]
        [ResponseCache(Duration = 3)]
        public IActionResult CreateSign()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["serverid"] = "1";
            var sign = SignCommon.CreateSign("secret", query: query, body: new { a = 1, b = "1" });
            query.Add("sign", sign);
            return Ok(new ResponseResult { Result = sign });
        }

        /// <summary>
        /// 有效签名访问;[VerifySign]特性标记需要签名访问;[IgnoreSign]忽略签名
        /// </summary>
        /// <returns></returns>
        [HttpGet("validsign")]
        [ResponseCache(Duration = 30)]
        [VerifySign]
        public IActionResult ValidSign()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["serverid"] = "1";
            var sign = SignCommon.CreateSign("secret", query: query, body: new { a = 1, b = "1" });
            query.Add("sign", sign);
            return Ok(new ResponseResult { Result = sign });
        }
    }
}
