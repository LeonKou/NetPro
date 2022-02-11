﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using HttpPostAttribute = WebApiClientCore.Attributes.HttpPostAttribute;

namespace XXX.Plugin.Tdengine.Proxy
{
    /// <summary>
    /// 主机地址配置在 appsetting.json >NetProProxyOption
    /// 获取或设置Http服务完整主机域名 例如http://www.abc.com设置了HttpHost值，HttpHostAttribute将失效
    /// [HttpHost("https://ug.baidu.com/")]
    /// </summary>
    public interface ITaosProxy
    {
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        [HttpPost("/rest/sql/{database}")]
        [ApiClientFilter]
        ITask<dynamic> ExecuteSql([Parameter(Kind.Json)] string sql, string database="test");
    }

    public class ApiClientFilter : ApiFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var configuration = context.HttpContext.ServiceProvider.GetService<IConfiguration>();
            configuration.GetValue<string>("Taos:Name");
            configuration.GetValue<string>("Taos:Pwd");
            string name = "root";
            string pwd = "taosdata";
           
            string token = $"{name}:{pwd}".Base64();

            context.HttpContext.RequestMessage.Headers.Add("Authorization", $"Basic {token}");
            var uri = context.HttpContext.RequestMessage.RequestUri;
            Console.WriteLine($"request uri is：{uri}");
            return Task.CompletedTask;
        }

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

                var resultString = await context.HttpContext.ResponseMessage?.Content.ReadAsStringAsync();
                if (context.ResultStatus == ResultStatus.HasException)
                {
                    if (_logger != null)
                        _logger.LogError($"远程调用异常：{context.HttpContext.RequestMessage.RequestUri}--errorMessage={resultString};StatusCode={context.HttpContext.ResponseMessage.StatusCode}");
                }

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
