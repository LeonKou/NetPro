using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.Web.Manager
{
    public interface IDemoService
    {
        void Test();
    }

    public class DemoService : IDemoService
    {
        private readonly ILogger<DemoService> _logger;
        public DemoService(ILogger<DemoService> logger)
        {
            _logger = logger;
        }
        public void Test()
        {
            _logger.LogError("YYYYYYYYYYYYYYYY ");
        }
    }
}
