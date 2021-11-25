using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XXX.API.Controllers
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

        [HttpGet]
        public void Get()
        {
            var er = EngineContext.Current.Resolve<IWebHelper>();
            var sd = er.GetCurrentIpAddress();
            var ass = AppDomain.CurrentDomain.GetAssemblies();
        }
    }

}
