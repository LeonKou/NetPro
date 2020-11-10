using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Leon.XXX.Proxy
{
    public interface IExampleProxy
    {
        [HttpGet("")]
        [WebApiClientFilter]
        ITask<dynamic> GetAsync([Parameter(Kind.Query)]string account);

        [HttpPost("api/v1/NetProgoods/list")]
        [Timeout(10 * 1000)] // 10s超时
        [WebApiClientFilter]
        ITask<dynamic> GetGoodsList(int appid, string appVersion);

        // POST api/user 
        [HttpPost("api/user")]
        [WebApiClientFilter]
        ITask<dynamic> AddAsync([FormContent] dynamic user);
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
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnResponseAsync(ApiResponseContext context)
        {
            Console.WriteLine($"HasResult：{context.ResultStatus}");
            Console.WriteLine($"context.Result：{context.Result}");

            var resultString = context.HttpContext.ResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"ReadAsStringAsync()：   {resultString}");
            Console.WriteLine($"StatusCode：   {context.HttpContext.ResponseMessage.StatusCode}");

            return Task.CompletedTask;
        }
    }
}
