using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.Web.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private IWebHelper _webHelper;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWebHelper  webHelper)
        {
            _logger = logger;
            _webHelper = webHelper;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var dd= _webHelper.GetClientInfo();
            var dd1 = _webHelper.GetCurrentIpAddress();
            //throw new Exception();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
