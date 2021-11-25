using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;

namespace XXX.Plugin.Web.Demo
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("qq")]
        public void Get()
        {
            var er= EngineContext.Current.Resolve<IWebHelper>();
            var sd= er.GetCurrentIpAddress();
        }
    }

}
