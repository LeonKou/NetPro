using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.IO;
using System.Text;

namespace NetPro.Logging.AspNetCore
{
    public class NetProExcepitonFilter : IExceptionFilter
    {
        private static readonly ILogger Logger = Serilog.Log.ForContext<NetProExcepitonFilter>();


        public void OnException(ExceptionContext context)
        {
            string requestPara = string.Empty;//请求参数

            // var body =  context.HttpContext.Request.conte

            var request = context.HttpContext.Request;
            var method = request.Method.ToUpper();
            if (method == "GET")
            {
                requestPara = request.Path.ToString();
            }
            else if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                request.Body.Position = 0;
                using (StreamReader reader
                          = new StreamReader(request.Body, Encoding.UTF8))
                {
                    requestPara = reader.ReadToEnd();
                }
            }
            //Serilog.Log.Error(context.Exception, "WebAPI异常 请求URL:{0},参数:{1}", request.Path.ToString(), requestPara);
            Logger.Error(context.Exception, "WebAPI异常 请求URL:{0},参数:{1}", request.Path.ToString(), requestPara);
        }
    }
}
