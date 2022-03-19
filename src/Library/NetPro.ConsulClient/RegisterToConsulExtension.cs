/*
 *  MIT License
 *  
 *  Copyright (c) 2021 LeonKou
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using Consul;
using Consul.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.ConsulClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class RegisterToConsulExtension
    {
        /// <summary>
        /// IP address of the network adapter
        /// For example 192.168.74.58
        /// </summary>
        private static string LANIP { get; set; }

        private static int PORT { get; set; }
        /// <summary>
        /// Add Consul
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulOptionSection = configuration.GetSection(nameof(ConsulOption));
            services.Configure<ConsulOption>(consulOptionSection);
            var consulOption = services.BuildServiceProvider().GetService<IOptions<ConsulOption>>().Value;
            if (!consulOption.Enabled)
            {
                return services;
            }

            var url = configuration.GetValue<string>("urls");//ASPNETCORE_URLS not support multi urls
            if (string.IsNullOrWhiteSpace(url))
            {
                url = configuration.GetValue<string>("ASPNETCORE_URLS");//ASPNETCORE_URLS not support multi urls
            }
            if (string.IsNullOrWhiteSpace(url))
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException("url current endpoint not found");
                LANIP = EndpointExtension.GetEndpointIp(services).FirstOrDefault();//There is only one network adapter by default
                PORT = 80;
            }
            else
            {
                Console.WriteLine($"[NetPro.ConsulClient] The current server's listen endpoint is {url}");

                if (url.Contains(';'))
                {
                    throw new ArgumentOutOfRangeException("Multiple hosts are not supported in NetPro.ConsulClient's ASPNETCORE_URLS variable");
                }
                if (url.Contains('*') || url.Contains('+') || url.Contains('['))
                {
                    throw new ArgumentException("wildcard character (*,+) are not supported in NetPro.ConsulClient's ASPNETCORE_URLS variable");
                }
                var launchEndpoint = new Uri(url);

                PORT = launchEndpoint.Port;
                LANIP = launchEndpoint.Host;
            }

            if (string.IsNullOrWhiteSpace(LANIP) || LANIP.Contains("0.0.0.0") || LANIP.Contains("127.0.0.1") || LANIP.Contains("localhost"))
            {
                LANIP = EndpointExtension.GetEndpointIp(services).FirstOrDefault();//There is only one network adapter by default
            }
            var serviceId = $"{consulOption.ServiceName}_{LANIP}:{PORT}";

            Task.Run(async () =>
            {
                services.AddConsul(options =>
                {
                    options.Datacenter = consulOption.Datacenter;
                    options.Address = new Uri(consulOption.EndPoint);
                    options.Token = consulOption.Token;
                    options.WaitTime = consulOption.WaitTime;

                }).AddConsulServiceRegistration(options =>
                {
                    //var checkttl = new AgentCheckRegistration
                    //{
                    //    //Timeout = TimeSpan.FromSeconds(12),
                    //    Name = $"checkttl",
                    //    TTL = TimeSpan.FromSeconds(5),
                    //    //Status = Consul.HealthStatus.Passing,
                    //    ServiceID = serviceId,
                    //    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    //    Notes = "app does a curl internally every 10 seconds",
                    //};
                    var checktcp = new AgentCheckRegistration
                    {
                        Timeout = TimeSpan.FromSeconds(10),
                        Name = $"checktcp",
                        TCP = $"{LANIP}:{PORT}",
                        Status = Consul.HealthStatus.Passing,
                        ServiceID = serviceId,
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        Interval = TimeSpan.FromSeconds(10),
                        Notes = "check for tcp",
                    };
                    options.Checks = new[] { /*checkttl,*/ checktcp };
                    options.ID = serviceId;
                    options.Name = consulOption.ServiceName;
                    options.Address = LANIP;
                    options.Port = PORT;
                    options.Tags = consulOption.Tags;
                    options.Meta = consulOption.Meta;
                });
            });

            return services;
        }
    }


    internal static class EndpointExtension
    {
        internal static string[] GetEndpointIp(IServiceCollection services)
        {
            var addressIpv4Hosts = NetworkInterface.GetAllNetworkInterfaces()

          .OrderByDescending(c => c.Speed)
          .Where(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up);

            List<string> ips = new List<string>();
            foreach (var item in addressIpv4Hosts)
            {
                if (item.Supports(NetworkInterfaceComponent.IPv4))
                {
                    var props = item.GetIPProperties();
                    //this is ip for ipv4
                    var firstIpV4Address = props.UnicastAddresses
                        .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(c => c.Address)
                        .FirstOrDefault().ToString();
                    ips.Add(firstIpV4Address);
                }
            }
            return ips.ToArray();
        }
    }
}
