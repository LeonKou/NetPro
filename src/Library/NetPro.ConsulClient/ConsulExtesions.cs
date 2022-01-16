using Consul;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.NetPro
{
    /// <summary>
    /// consul service address
    /// </summary>
    public struct ServiceAddress
    {
        /// <summary>
        /// tags
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// address
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// ServiceName
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ServiceID
        /// </summary>
        public string ServiceID { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ConsulExtesions
    {
        /// <summary>
        /// service discovery
        /// </summary>
        /// <param name="agentEndpoint"></param>
        /// <returns></returns>
        public static async Task<IList<ServiceAddress>> DiscoveryAsync(this IAgentEndpoint agentEndpoint)
        {
            var services = await agentEndpoint.Services();
            var list = services.Response.Select(s =>
              new ServiceAddress
              {
                  Address = new Uri($"http://{s.Value.Address}:{s.Value.Port}"),
                  Tags = s.Value.Tags,
                  ServiceID = s.Value.ID,
                  ServiceName = s.Value.Service
              }).ToList();
            return list;
        }

        /// <summary>
        /// service discovery
        /// </summary>
        /// <param name="consulClient"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static async Task<IList<ServiceAddress>> DiscoveryAsync(this IConsulClient consulClient, string serviceName = null)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                var services = await consulClient.Agent.Services();
                var list = services.Response.Select(s =>
                  new ServiceAddress
                  {
                      Address = new Uri($"http://{s.Value.Address}:{s.Value.Port}"),
                      Tags = s.Value.Tags,
                      ServiceID = s.Value.ID,
                      ServiceName = s.Value.Service
                  }).ToList();
                return list;
            }
            else
            {
                var services = await consulClient.Catalog.Service(serviceName);
                var list = services.Response.Select(s => new ServiceAddress
                {
                    Address = new Uri($"http://{s.ServiceAddress}:{s.ServicePort}"),
                    Tags = s.ServiceTags,
                    ServiceName = s.ServiceName,
                    ServiceID = s.ServiceID,
                }).ToList();
                return list;
            }
        }

        /// <summary>
        /// service discovery
        /// </summary>
        /// <param name="consulClient"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IList<ServiceAddress> Discovery(this IConsulClient consulClient, string serviceName = null)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                var services = consulClient.Agent.Services().ConfigureAwait(false).GetAwaiter().GetResult();
                var list = services.Response.Select(s =>
                  new ServiceAddress
                  {
                      Address = new Uri($"http://{s.Value.Address}:{s.Value.Port}"),
                      Tags = s.Value.Tags,
                      ServiceID = s.Value.ID,
                      ServiceName = s.Value.Service
                  }).ToList();
                return list;
            }
            else
            {
                var services = consulClient.Catalog.Service(serviceName).ConfigureAwait(false).GetAwaiter().GetResult();
                var list = services.Response.Select(s => new ServiceAddress
                {
                    Address = new Uri($"http://{s.ServiceAddress}:{s.ServicePort}"),
                    Tags = s.ServiceTags,
                    ServiceName = s.ServiceName,
                    ServiceID = s.ServiceID,
                }).ToList();
                return list;
            }
        }

        /// <summary>
        /// service discovery
        /// </summary>
        /// <param name="agentEndpoint"></param>
        /// <returns></returns>
        public static IList<ServiceAddress> Discovery(this IAgentEndpoint agentEndpoint)
        {
            var services = agentEndpoint.Services().ConfigureAwait(false).GetAwaiter().GetResult();
            var list = services.Response.Select(s =>
              new ServiceAddress
              {
                  Address = new Uri($"http://{s.Value.Address}:{s.Value.Port}"),
                  Tags = s.Value.Tags,
                  ServiceID = s.Value.ID,
                  ServiceName = s.Value.Service
              }).ToList();
            return list;
        }

        /// <summary>
        /// 服务发现
        /// service discovery
        /// </summary>
        /// <param name="catalogEndpoint"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static async Task<IList<ServiceAddress>> DiscoveryAsync(this ICatalogEndpoint catalogEndpoint, string serviceName)
        {
            var services = await catalogEndpoint.Service(serviceName);
            var list = services.Response.Select(s => new ServiceAddress
            {
                Address = new Uri($"http://{s.ServiceAddress}:{s.ServicePort}"),
                Tags = s.ServiceTags,
                ServiceName = s.ServiceName,
                ServiceID = s.ServiceID,
            }).ToList();
            return list;
        }

        /// <summary>
        /// 服务发现
        /// service discovery
        /// </summary>
        /// <param name="catalogEndpoint"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IList<ServiceAddress> Discovery(this ICatalogEndpoint catalogEndpoint, string serviceName)
        {
            var services = catalogEndpoint.Service(serviceName).ConfigureAwait(false).GetAwaiter().GetResult();
            var list = services.Response.Select(s => new ServiceAddress
            {
                Address = new Uri($"http://{s.ServiceAddress}:{s.ServicePort}"),
                Tags = s.ServiceTags,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
            }).ToList();
            return list;
        }
    }
}
