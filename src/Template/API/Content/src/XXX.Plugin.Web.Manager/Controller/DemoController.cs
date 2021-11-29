using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;

namespace XXX.Plugin.Web.Manager
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
        private readonly XXX.Plugin.Web.Demo.IDemoService _managerdemoService;

        public DemoController(ILogger<DemoController> logger,
            IWebHelper webHelper
            , IDemoService demoService,
            XXX.Plugin.Web.Demo.IDemoService managerdemoService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _demoService = demoService;
            _managerdemoService = managerdemoService;
        }

        /// <summary>
        /// 测试组件
        /// </summary>
        [HttpGet("TestManagerCompount")]
        public string Get()
        {
            var enWeb = EngineContext.Current.Resolve<IWebHelper>();
            var ip = enWeb.GetCurrentIpAddress();
            var diweb = _webHelper.GetCurrentIpAddress();
            _demoService.Test();
            _managerdemoService.Test();
            return "";
        }
    }

}
