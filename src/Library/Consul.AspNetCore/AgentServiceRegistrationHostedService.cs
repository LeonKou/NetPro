using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consul.AspNetCore
{
    public class AgentServiceRegistrationHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger<AgentServiceRegistrationHostedService> _logger;
        private readonly AgentServiceRegistration _serviceRegistration;
        private Timer _timer;
        internal static int PORT = 0;
        private static bool _registered = false;

        public AgentServiceRegistrationHostedService(
            IConsulClient consulClient,
            ILogger<AgentServiceRegistrationHostedService> logger,
            AgentServiceRegistration serviceRegistration)
        {
            _consulClient = consulClient;
            _logger = logger;
            _serviceRegistration = serviceRegistration;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async state => await Execute(cancellationToken), null, dueTime: TimeSpan.FromSeconds(3), period: TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                if (PORT != 0 && !_registered)
                {
                    _serviceRegistration.Port = PORT;
                    await _consulClient.Agent.ServiceRegister(_serviceRegistration, cancellationToken);
                    _registered = true;
                }
                else if (_registered)
                {
                    await _consulClient.Agent.PassTTL((_serviceRegistration.Check as AgentCheckRegistration_1_7).CheckID, $"{DateTimeOffset.UtcNow} was checked");
                    _logger.LogInformation($"Consul is executing passTTL for a health report");
                }
            }
            catch (Exception ex)
            {
                _serviceRegistration.Port = PORT;
                await _consulClient.Agent.ServiceRegister(_serviceRegistration, cancellationToken);
                _logger.LogError(ex, $"Consul failed to execute health report");
            }
            finally
            {
                _timer.Change(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _consulClient.Agent.ServiceDeregister(_serviceRegistration.ID, cancellationToken);
        }
    }

    public class CatalogRegistrationHostedService : IHostedService
    {
        private readonly ILogger<CatalogRegistrationHostedService> _logger;
        private readonly IConsulClient _consulClient;
        private readonly CatalogRegistration _catalogRegistration;
        private Timer _timer;
        internal static int PORT = 0;
        private static bool _registered = false;

        public CatalogRegistrationHostedService(
            ILogger<CatalogRegistrationHostedService> logger,
            IConsulClient consulClient,
            CatalogRegistration catalogRegistration)
        {
            _logger = logger;
            _consulClient = consulClient;
            _catalogRegistration = catalogRegistration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async state => await Execute(cancellationToken), null, dueTime: TimeSpan.FromSeconds(3), period: TimeSpan.FromSeconds(3));
            return Task.CompletedTask;
        }

        private async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                if (PORT != 0 && !_registered)
                {
                    _catalogRegistration.Service.Port = PORT;
                    await _consulClient.Catalog.Register(_catalogRegistration, cancellationToken);
                    _registered = true;
                }
                else if (_registered)
                {
                    await _consulClient.Agent.UpdateTTL($"{_catalogRegistration.Check.CheckID}", $"{DateTimeOffset.UtcNow} was checked", TTLStatus.Pass);
                    _logger.LogInformation($"Consul is executing passTTL for a health report");
                }
            }
            catch (Exception ex)
            {
                await _consulClient.Catalog.Register(_catalogRegistration, cancellationToken);
                _logger.LogError(ex, $"Consul failed to execute health report");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _consulClient.Catalog.Deregister(new CatalogDeregistration { ServiceID = _catalogRegistration.Service.ID, Node = _catalogRegistration.Node }, cancellationToken);
        }
    }
}
