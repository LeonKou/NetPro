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
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<ZeroMQController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public ZeroMQController(
            IHostEnvironment hostEnvironment,
            ILogger<ZeroMQController> logger,
            IHttpClientFactory httpClientFactory
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("PublisherSocket")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> PublisherSocket()
        {
            Task.Factory.StartNew(() => {
                using (var publisher = new PublisherSocket())
                {
                    //发布是由于本机承载故配回环地址即可
                    //发布者优先使用bind方法；订阅者和拉取侧优先使用Connect;发布者和推送者优先使用回环地址
                    publisher.Bind("tcp://*:81");
                    publisher
                   .SendMoreFrame("A:b") // Topic支持特殊符号，topic命名最佳实践：模块名/功能命/功能层级
                   .SendFrame(DateTimeOffset.Now.ToString()); // Message
                }
            });
           
            return Ok();
        }

        [HttpGet("PushSocket")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> PushSocket()
        {
            //推数据 https://github.com/zeromq/netmq/blob/ea0a5a7e1b77a1ade9311f187f4ff37a20d5d964/src/NetMQ.Tests/PushPullTests.cs

            Task.Run(() =>
            {
                using (var pushSocket = new PushSocket())
                {
                    //发布是由于本机承载故配回环地址即可
                    //发布者优先使用bind方法
                    pushSocket.Bind("tcp://*:82");
                    while (true)
                    {
                        pushSocket.SendFrame("Hello Clients");
                        //Console.WriteLine("推送-Hello Clients");
                    }
                }
            });
            return Ok();
        }
    }
}
