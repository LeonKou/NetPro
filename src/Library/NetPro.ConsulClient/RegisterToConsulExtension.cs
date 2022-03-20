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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Consul.AspNetCore;

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

        //private static int PORT { get; set; }
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

            if (string.IsNullOrWhiteSpace(LANIP) || LANIP.Contains("0.0.0.0") || LANIP.Contains("127.0.0.1") || LANIP.Contains("localhost"))
            {
                LANIP = EndpointExtension.GetEndpointIp(services).FirstOrDefault();//There is only one network adapter by default
            }
            var serviceId = Guid.NewGuid().ToString("N");

            services.AddConsul(options =>
            {
                options.Datacenter = consulOption.Datacenter;
                options.Address = new Uri(consulOption.EndPoint);
                options.Token = consulOption.Token;
                options.WaitTime = consulOption.WaitTime;

            })
                .AddConsulServiceRegistration(options =>
            {
                var checkttl = new AgentCheckRegistration_1_7
                {
                    CheckID = serviceId,
                    Timeout = TimeSpan.FromSeconds(12),
                    Name = $"checkttl",
                    TTL = TimeSpan.FromSeconds(10),
                    Status = Consul.HealthStatus.Passing,
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Notes = "app does a curl internally every 10 seconds",
                };
                options.Check = checkttl;
                options.ID = serviceId;
                options.Name = consulOption.ServiceName;
                options.Address = LANIP;
                options.Tags = consulOption.Tags;
                options.Meta = consulOption.Meta;
            })
            ;
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
