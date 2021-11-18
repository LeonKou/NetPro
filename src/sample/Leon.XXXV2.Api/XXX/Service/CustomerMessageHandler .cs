using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQMiddleware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Leon.XXXV2.Api
{
    public class CustomerMessageHandler : IAsyncMessageHandler
    {
        readonly ILogger _logger;
        private IServiceScopeFactory _serviceScopeFactory;
        public CustomerMessageHandler(
            ILogger<CustomerMessageHandler> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(string message, string routingKey)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            //var _xXXService = serviceScope.ServiceProvider.GetRequiredService<IXXXService>();
            var body = JsonSerializer.Deserialize<dynamic>(message);
            //_xXXService.GetList();

            await Task.Delay(1000);

            if (body == "ex")
            {
                throw new Exception("ex");
            }

            Console.WriteLine($"消费数据-->{message},{routingKey}");

        }
    }
}
