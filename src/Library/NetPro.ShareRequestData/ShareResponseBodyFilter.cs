using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.ShareRequestBody
{
    /// <summary>
    /// 控制器响应body
    /// </summary>
    public class ShareResponseBodyFilter : IAsyncResultFilter
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private ResponseCacheData _responseCacheData { get; set; }

        public ShareResponseBodyFilter(ILogger<ShareResponseBodyFilter> logger
            , IConfiguration configuration
            , ResponseCacheData responseCacheData)
        {
            _logger = logger;
            _configuration = configuration;
            _responseCacheData = responseCacheData;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_responseCacheData == null || string.IsNullOrEmpty(_responseCacheData.Body))
            {
                var responseHeadersText = await CommonTools.FormatResponseBody(context.HttpContext.Response);
                _responseCacheData = new ResponseCacheData { Body = responseHeadersText };
            }
            await next();
        }
    }

    public static class CommonTools
    {
        public static async Task<string> FormatRequestBody(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body.Position = 0;

            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        public static async Task<string> FormatResponseBody(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            response.Body.Position = 0;

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return text;
        }
    }
}
