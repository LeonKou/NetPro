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

using BetterConsoles.Colors.Extensions;
using BetterConsoles.Tables;
using BetterConsoles.Tables.Builders;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetPro.ConsulClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.NetPro.Startup._;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;
using Consul.AspNetCore;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;

[assembly: HostingStartup(typeof(ConsulClientStartup))]
namespace NetPro.ConsulClient
{
    internal class ConsulClientStartup : IHostingStartup
    {
        /// <summary>
        /// IP address of the network adapter
        /// For example 192.168.74.58
        /// </summary>
        private static string LANIP { get; set; }
        private static int PORT { get; set; }

        public void Configure(IWebHostBuilder builder)
        {
            builder.Configure((context, app) =>
            {
                var addressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                var uriString = addressFeature.Addresses.FirstOrDefault();
                var uri = new Uri(uriString);
                PORT = uri.Port;
                if (!(uri.Host.Contains("0.0.0.0") || uri.Host.Contains("+") || uri.Host.Contains("*")))//http://+:80
                {
                    LANIP = uri.Host;
                }
            });
            builder.ConfigureServices(async (context, services) =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                var consulOptionSection = configuration.GetSection(nameof(ConsulOption));
                services.Configure<ConsulOption>(consulOptionSection);
                var consulOption = services.BuildServiceProvider().GetService<IOptions<ConsulOption>>().Value;
                if (!consulOption.Enabled)
                {
                    return;
                }

                //var url = configuration.GetValue<string>("urls");//ASPNETCORE_URLS not support multi urls
                //if (string.IsNullOrWhiteSpace(url))
                //{
                //    url = configuration.GetValue<string>("ASPNETCORE_URLS");//ASPNETCORE_URLS not support multi urls
                //}
                //if (string.IsNullOrWhiteSpace(url))
                //{
                //    if (string.IsNullOrWhiteSpace(url))
                //        throw new ArgumentNullException("url current endpoint not found");
                //    LANIP = EndpointExtension.GetEndpointIp(services).FirstOrDefault();//There is only one network adapter by default
                //    PORT = 80;
                //}
                //else
                //{
                //    Console.WriteLine($"[NetPro.ConsulClient] The current server's listen endpoint is {url}");

                //    if (url.Contains(';'))
                //    {
                //        throw new ArgumentOutOfRangeException("Multiple hosts are not supported in NetPro.ConsulClient's ASPNETCORE_URLS variable");
                //    }
                //    if (url.Contains('*') || url.Contains('+') || url.Contains('['))
                //    {
                //        throw new ArgumentException("wildcard character (*,+) are not supported in NetPro.ConsulClient's ASPNETCORE_URLS variable");
                //    }
                //    var launchEndpoint = new Uri(url);

                //    PORT = launchEndpoint.Port;
                //    LANIP = launchEndpoint.Host;
                //}

                //if (string.IsNullOrWhiteSpace(LANIP) || LANIP.Contains("0.0.0.0") || LANIP.Contains("127.0.0.1") || LANIP.Contains("localhost"))
                //{
                LANIP = EndpointExtension.GetEndpointIp(services).FirstOrDefault();//There is only one network adapter by default
                                                                                   //}
                //PORT = int.Parse(Environment.GetEnvironmentVariable("NetProPort"));
                var serviceId = $"{consulOption.ServiceName}_{LANIP}:{PORT}";

                services.AddConsul(options =>
                {
                    options.Datacenter = consulOption.Datacenter;
                    options.Address = new Uri(consulOption.EndPoint);
                    options.Token = consulOption.Token;
                    options.WaitTime = consulOption.WaitTime;

                }).AddConsulServiceRegistration(options =>
                {
                    Console.WriteLine($"{LANIP}:{PORT}");
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
                    options.Checks = new[] { checktcp };
                    options.ID = serviceId;
                    options.Name = consulOption.ServiceName;
                    options.Address = LANIP;
                    options.Port = PORT;
                    options.Tags = consulOption.Tags;
                    options.Meta = consulOption.Meta;
                });
            });

        }
    }
}
