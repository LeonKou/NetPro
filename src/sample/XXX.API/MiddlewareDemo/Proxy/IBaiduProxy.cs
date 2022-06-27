using WebApiClientCore;
using WebApiClientCore.Attributes;
using HttpGetAttribute = WebApiClientCore.Attributes.HttpGetAttribute;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 主机地址配置在 appsetting.json >NetProProxyOption
    /// 获取或设置Http服务完整主机域名 例如http://www.abc.com设置了HttpHost值，HttpHostAttribute将失效
    /// [HttpHost("https://ug.baidu.com/")]
    /// </summary>
    /// <remarks>
    /// QA:https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient-QA
    /// 文档：https://www.cnblogs.com/kewei/p/12939866.html
    /// </remarks>
    public interface IBaiduProxy:IHttpApi
    {
        [HttpGet("/")]
        [LoggingFilter]
        //[RawReturn]
        ITask<string> SharepageAsync([Parameter(Kind.Query)] string queryparameter);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [LoggingFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [LoggingFilter]
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
        [LoggingFilter]
        ITask<dynamic> LoginByPwd([Uri] string url, [Parameter(Kind.Query)] string username, string password, string Captcha);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IExampleProxy
    {
        [HttpGet("")]
        [LoggingFilter]
        ITask<dynamic> GetAsync([Parameter(Kind.Query)] string account);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [LoggingFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [LoggingFilter]
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
        [LoggingFilter]
        ITask<dynamic> LoginByPwd([Uri] string url, [Parameter(Kind.Query)] string username, string password, string Captcha);
    }
}
