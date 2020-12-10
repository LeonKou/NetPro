using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQMiddleware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Leon.XXX.Domain.XXX.Service
{
    public class CustomerMessageHandler : IMessageHandler
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

        public void Handle(string message, string routingKey)
        {
            // 转盘抽奖记录 消费
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var _dataBaseOptionService = serviceScope.ServiceProvider.GetRequiredService<IDataBaseOptionService>();
            var body = JsonSerializer.Deserialize<dynamic>(message);
            _dataBaseOptionService.DeleteAsync(1).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
