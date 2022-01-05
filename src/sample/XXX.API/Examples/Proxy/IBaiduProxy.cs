using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using HttpGetAttribute = WebApiClientCore.Attributes.HttpGetAttribute;

namespace XXX.API.Controllers
{
    ///获取或设置Http服务完整主机域名 例如http://www.abc.com设置了HttpHost值，HttpHostAttribute将失效
    //[HttpHost("https://ug.baidu.com/")]
    public interface IBaiduProxy
    {
        [HttpGet("/")]
        [WebApiClientFilter]
        //[RawReturn]
        ITask<string> SharepageAsync([Parameter(Kind.Query)] string queryparameter);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [WebApiClientFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [WebApiClientFilter]
        ITask<dynamic> AddAsync([FormContent] dynamic user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="Captcha"></param>
        /// <returns></returns>
        [HttpPost("/api/ldap")]
        [Timeout(10 * 1000)] // 10s超时
        [JsonReturn(Enable = false)]
        [Cache(60 * 1000)]
        [WebApiClientFilter]
        ITask<dynamic> LoginByPwd([Uri] string url, [Parameter(Kind.Query)] string username, string password, string Captcha);
    }

    public interface IExampleProxy
    {
        [HttpGet("")]
        [WebApiClientFilter]
        ITask<dynamic> GetAsync([Parameter(Kind.Query)] string account);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [WebApiClientFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [WebApiClientFilter]
        ITask<dynamic> AddAsync([FormContent] dynamic user);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="Captcha"></param>
        /// <returns></returns>
        [HttpPost("/api/ldap")]
        [Timeout(10 * 1000)] // 10s超时
        [JsonReturn(Enable = false)]
        [Cache(60 * 1000)]
        [WebApiClientFilter]
        ITask<dynamic> LoginByPwd([Uri] string url, [Parameter(Kind.Query)] string username, string password, string Captcha);
    }

    /// <summary>
    /// 过滤器
    /// </summary>
    public class WebApiClientFilter : ApiFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var uri= context.HttpContext.RequestMessage.RequestUri;
                Console.WriteLine($"request uri is：{uri}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnResponseAsync(ApiResponseContext context)
        {
            Console.WriteLine($"HasResult：{context.ResultStatus}");
            Console.WriteLine($"context.Result：{context.Result}");

            var resultString = await context.HttpContext.ResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine($"ReadAsStringAsync()：   {resultString}");
            Console.WriteLine($"StatusCode：   {context.HttpContext.ResponseMessage.StatusCode}");
        }
    }
}
