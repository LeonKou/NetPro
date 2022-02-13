using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace System.NetPro
{
    /// <summary>
    /// WebApiClientCore过滤器
    /// 响应为流时使用此特性会导致多次读流产生异常
    /// </summary>
    /// <exception cref="System.Exception">occur error  when the return type is stream.</exception>
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

            try
            {
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

                string resultString = null;
                HttpStatusCode statusCode = HttpStatusCode.BadRequest;
                if (context.HttpContext.ResponseMessage != null)
                {
                    resultString = await context.HttpContext.ResponseMessage.Content.ReadAsStringAsync();
                    statusCode = context.HttpContext.ResponseMessage.StatusCode;
                }
                if (context.ResultStatus == ResultStatus.HasException)
                {
                    if (_logger != null)
                        _logger.LogError($"远程调用异常：{context.HttpContext.RequestMessage.RequestUri}--errorMessage={resultString};StatusCode={statusCode}");
                }

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
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, $"{ex.Message}");
                }
                else
                {
                    Console.WriteLine($"Error：{ex.Message}");
                }
            }
        }
    }
}
