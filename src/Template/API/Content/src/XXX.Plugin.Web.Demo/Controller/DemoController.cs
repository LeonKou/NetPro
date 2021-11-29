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
        private readonly IWebHelper _webHelper;
        private readonly IDemoService _demoService;

        public DemoController(ILogger<DemoController> logger,
            IWebHelper webHelper
            , IDemoService demoService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _demoService = demoService;
        }

        /// <summary>
        /// 测试组件
        /// </summary>
        [HttpGet("TestCompount")]
        public string Get()
        {
            var enWeb = EngineContext.Current.Resolve<IWebHelper>();
            var ip = enWeb.GetCurrentIpAddress();
            var diweb = _webHelper.GetCurrentIpAddress();
            _demoService.Test();
            return "";
        }
    }

}
