using Microsoft.AspNetCore.Mvc;
using NetPro.Pulsar;
using System;
using System.Web;

namespace XXX.Plugin.Pulsar.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PulsarTestController : ControllerBase
    {
        private readonly IPulsarQueneService _pulsarQuene;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pulsarQuene"></param>
        public PulsarTestController(IPulsarQueneService pulsarQuene)
        {
            _pulsarQuene = pulsarQuene;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">msg</param>
        /// <returns></returns>
        [HttpGet("send")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> SendASync(string msg)
        {
            await _pulsarQuene.ProduceMessagesAsync("persistent://public/default/service-test-pulsar", msg);
            return Ok();
        }
    }
}
