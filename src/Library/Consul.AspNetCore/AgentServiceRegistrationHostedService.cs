using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Consul.AspNetCore
{
    public class AgentServiceRegistrationHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly AgentServiceRegistration _serviceRegistration;
        private Timer _timer;
        internal static int PORT = 0;
        private static bool _registered = false;

        public AgentServiceRegistrationHostedService(
            IConsulClient consulClient,
            AgentServiceRegistration serviceRegistration)
        {
            _consulClient = consulClient;
            _serviceRegistration = serviceRegistration;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async state => await Execute(cancellationToken), null, dueTime: TimeSpan.FromSeconds(3), period: TimeSpan.FromSeconds(5));
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
                }
            }
            catch (Exception ex)
            {
                _serviceRegistration.Port = PORT;
                await _consulClient.Agent.ServiceRegister(_serviceRegistration, cancellationToken);
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
        private readonly IConsulClient _consulClient;
        private readonly CatalogRegistration _catalogRegistration;
        private Timer _timer;
        internal static int PORT = 0;
        private static bool _registered = false;

        public CatalogRegistrationHostedService(
            IConsulClient consulClient,
            CatalogRegistration catalogRegistration)
        {
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
                }
            }
            catch (Exception ex)
            {
                await _consulClient.Catalog.Register(_catalogRegistration, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _consulClient.Catalog.Deregister(new CatalogDeregistration { ServiceID = _catalogRegistration.Service.ID, Node = _catalogRegistration.Node }, cancellationToken);
        }
    }
}
