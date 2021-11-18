
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using Leon.XXX.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MQMiddleware;
using NetPro.Web.Api.Controllers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Leon.XXXV2.Api
{
    /// <summary>
    ///这是controller
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RibbitMQController : ControllerBase
    {
        private readonly IQueueService _queueService;
        private readonly ILogger<RibbitMQController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        //[FromServices]
        public RibbitMQController(ILogger<RibbitMQController> logger, IQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }

        /// <summary>
        /// 获取一个查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("send")]
        [ProducesResponseType(200)]
        [ProducesResponseType(200, Type = typeof(XXXAo))]
        public async Task<string[]> Send([FromQuery] string msg)
        {
            _queueService.Send(//发送消息
                                @object: $"{msg}",
                                exchangeName: "exchange",
                                routingKey: ""
                                 );
            return new string[] { "value1", "value2" };
        }
    }
}
