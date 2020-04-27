using Microsoft.AspNetCore.Http;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace NetPro.Web.Core.Middlewares
{
    public class MiniProfilerMiddleware
    {
        private readonly RequestDelegate _next;

        public MiniProfilerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            using (MiniProfiler.Current.Step("HttpRequest"))
            {
                await _next(context);
            }
        }
    }
}
