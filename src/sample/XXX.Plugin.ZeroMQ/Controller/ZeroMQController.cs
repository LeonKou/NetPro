using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.NetPro;
using System.Text;

namespace XXX.Plugin.Tdengine
{
    /// <summary>
    /// zeromq Demo
    /// 81：发布订阅；82：推拉
    /// scope生命周期不支持zeromq对象
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ZeroMQController : ControllerBase
    {
        private readonly ILogger<ZeroMQController> _logger;
        private readonly PublisherSocket _publisherSocket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="publisherSocket"></param>
        public ZeroMQController(
            ILogger<ZeroMQController> logger,
            PublisherSocket publisherSocket
            )
        {
            _logger = logger;
            _publisherSocket = publisherSocket;
        }

        /// <summary>
        /// 发布zeromq
        /// </summary>
        /// <returns></returns>
        [HttpGet("PublisherSocket")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult PublisherSocket()
        {
            _publisherSocket.SendMoreFrame("A:b") // Topic支持特殊符号，topic命名最佳实践：模块名/功能命/功能层级
                   .SendFrame(DateTimeOffset.Now.ToString());

            return Ok();
        }

        /// <summary>
        /// 推zeromq
        /// </summary>
        /// <returns></returns>
        [HttpGet("PushSocket")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public IActionResult PushSocket()
        {
            //推数据 https://github.com/zeromq/netmq/blob/ea0a5a7e1b77a1ade9311f187f4ff37a20d5d964/src/NetMQ.Tests/PushPullTests.cs
            _publisherSocket.SendFrame("Hello Clients"); ;

            return Ok();
        }
    }
}
