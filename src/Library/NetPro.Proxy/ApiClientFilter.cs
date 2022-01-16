using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace System.NetPro
{
    /// <summary>
    /// WebApiClientCore过滤器
    /// </summary>
    public class ApiClientFilter : ApiFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var uri = context.HttpContext.RequestMessage.RequestUri;
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
            var _logger = context.HttpContext.ServiceProvider.GetService<ILogger<ApiClientFilter>>();
            if (_logger != null)
            {
                _logger.LogInformation($"HasResult：{context.ResultStatus}");
                _logger.LogInformation($"context.Result：{context.Result}");
            }
            else
            {
                Console.WriteLine($"HasResult：{context.ResultStatus}");
                Console.WriteLine($"context.Result：{context.Result}");
            }

            if (context.ResultStatus == ResultStatus.HasException)
            {
                var errorMessage = context.HttpContext.ResponseMessage.Content.ReadAsStringAsync().Result;
                if (_logger != null)
                    _logger.LogError($"远程调用异常：{context.HttpContext.RequestMessage.RequestUri}--errorMessage={errorMessage};StatusCode={context.HttpContext.ResponseMessage.StatusCode}");
            }
            var resultString = await context.HttpContext.ResponseMessage.Content.ReadAsStringAsync();
            var statusCode = context.HttpContext.ResponseMessage.StatusCode;
            if (_logger != null)
            {
                _logger.LogInformation($"ReadAsStringAsync()：   {resultString}");
                _logger.LogInformation($"StatusCode：   {statusCode}");
            }
            else
            {
                Console.WriteLine($"ReadAsStringAsync()：   {resultString}");
                Console.WriteLine($"StatusCode：   {statusCode}");
            }

            if (context.ResultStatus == ResultStatus.None)
            {
                context.Exception = new Exception($"message={resultString};statusCode={statusCode}");
            }
        }
    }
}
