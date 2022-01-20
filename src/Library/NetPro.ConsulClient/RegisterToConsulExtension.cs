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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetPro.ConsulClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class RegisterToConsulExtension
    {
        /// <summary>
        /// 宿主机ip(环境变量：LANIP)
        /// 例如192.168.74.58
        /// </summary>
        private static string LANIP { get; set; }

        private static int? PORT { get; set; }
        /// <summary>
        /// Add Consul
        /// 添加consul
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            //配置consul注册地址
            var consulOptionSection = configuration.GetSection(nameof(ConsulOption));
            services.Configure<ConsulOption>(consulOptionSection);
            var consulOption = services.BuildServiceProvider().GetService<IOptions<ConsulOption>>();
            if (!consulOption.Value.Enabled)
            {
                return services;
            }
            //http://+:80
            LANIP = configuration["LANIP"];
            PORT = configuration.GetValue<int?>("PORT");

            // configuration Consul register address
            //configuration Consul client
            //配置consul客户端
            services.AddSingleton<IConsulClient>(sp => new Consul.ConsulClient(config =>
            {
                var consulOptions = sp.GetRequiredService<IOptions<ConsulOption>>().Value;
                if (!string.IsNullOrWhiteSpace(consulOptions.EndPoint))
                {
                    config.Address = new Uri(consulOptions.EndPoint);
                }
            }));

            return services;
        }

        /// <summary>
        /// use Consul
        /// 使用consul
        /// The default health check interface format is http://host:port/HealthCheck
        /// 默认的健康检查接格式是 http://host:port/HealthCheck
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            IOptions<ConsulOption> serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulOption>>();
            if (!serviceOptions.Value.Enabled)
            {
                return app;
            }
            //***

            Microsoft.Extensions.Hosting.IHostApplicationLifetime appLife = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();

            var features = app.ServerFeatures;

            IConsulClient consul = app.ApplicationServices.GetRequiredService<IConsulClient>();

            if (!PORT.HasValue)
            {
                var addresses = features.Get<IServerAddressesFeature>().Addresses;
                var uriString = addresses.FirstOrDefault();
                var uri = new Uri(uriString);
                PORT = uri.Port;
                if (!(uri.Host.Contains("0.0.0.0") || uri.Host.Contains("+") || uri.Host.Contains("*")))//http://+:80
                {
                    LANIP = uri.Host;
                }
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"application port is :{PORT.Value}");
            var addressIpv4Hosts = NetworkInterface.GetAllNetworkInterfaces()
            .OrderByDescending(c => c.Speed)
            .Where(c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up);

            if (string.IsNullOrWhiteSpace(LANIP))
            {
                foreach (var item in addressIpv4Hosts)
                {
                    var props = item.GetIPProperties();
                    //this is ip for ipv4
                    //这是ipv4的ip地址
                    var firstIpV4Address = props.UnicastAddresses
                        .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select(c => c.Address)
                        .FirstOrDefault().ToString();
                    var serviceId = $"{serviceOptions.Value.ServiceName}_{firstIpV4Address}:{PORT}";

                    var httpCheck = new AgentServiceCheck()
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                        Interval = TimeSpan.FromSeconds(30),
                        //this is default health check interface
                        //这个是默认健康检查接口
                        HTTP = $"{Uri.UriSchemeHttp}://{firstIpV4Address}:{PORT}{serviceOptions.Value.HealthPath}",
                    };

                    var registration = new AgentServiceRegistration()
                    {
                        Checks = new[] { httpCheck },
                        Address = firstIpV4Address.ToString(),
                        ID = serviceId,
                        Name = serviceOptions.Value.ServiceName,
                        Port = PORT.Value,
                        Tags = serviceOptions.Value.Tags
                    };

                    var logger = app.ApplicationServices.GetRequiredService<ILogger<ConsulOption>>();

                    try
                    {
                        consul.Agent.ServiceRegister(registration).ConfigureAwait(false).GetAwaiter().GetResult();
                        //send consul request after service stop
                        //当服务停止后想consul发送的请求
                        appLife.ApplicationStopping.Register(() =>
                        {
                            consul.Agent.ServiceDeregister(serviceId).ConfigureAwait(false).GetAwaiter().GetResult();
                        });
                    }

                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"consul error");
                    }
                    finally
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        logger.LogInformation($"health check service:{httpCheck.HTTP}");
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{LANIP}:{PORT}";
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    //this is default health check interface
                    //这个是默认健康检查接口
                    HTTP = $"{Uri.UriSchemeHttp}://{LANIP}:{PORT}{serviceOptions.Value.HealthPath}",
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = LANIP.ToString(),
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = PORT.Value,
                    Tags = serviceOptions.Value.Tags,
                };

                try
                {
                    consul.Agent.ServiceRegister(registration).ConfigureAwait(false).GetAwaiter().GetResult();

                    //send consul request after service stop
                    //当服务停止后想consul发送的请求
                    appLife.ApplicationStopping.Register(() =>
                    {
                        consul.Agent.ServiceDeregister(serviceId).ConfigureAwait(false).GetAwaiter().GetResult();
                    });
                }
                catch (Exception ex)
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<ConsulOption>>();
                    logger.LogError(ex, $"consul error");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"health check service:{httpCheck.HTTP}");
            }

            //register localhost address
            //注册本地地址
            //var localhostregistration = new AgentServiceRegistration()
            //{
            //    Checks = new[] { new AgentServiceCheck()
            //    {
            //        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
            //        Interval = TimeSpan.FromSeconds(30),
            //        HTTP = $"{Uri.UriSchemeHttp}://localhost:{PORT}{serviceOptions.Value.HealthPath}",
            //    } },
            //    Address = "localhost",
            //    ID = $"{serviceOptions.Value.ServiceName}_localhost:{PORT}",
            //    Name = serviceOptions.Value.ServiceName,
            //    Port = PORT.Value
            //};

            //consul.Agent.ServiceRegister(localhostregistration).ConfigureAwait(false).GetAwaiter().GetResult();

            ////send consul request after service stop
            ////当服务停止后向consul发送的请求
            //appLife.ApplicationStopping.Register(() =>
            //{
            //    consul.Agent.ServiceDeregister(localhostregistration.ID).ConfigureAwait(false).GetAwaiter().GetResult();
            //});

            app.Map($"{serviceOptions.Value.HealthPath}", s =>
            {
                s.Run(async context =>
                {
                    await context.Response.WriteAsync("ok");
                });
            });
            return app;
        }
    }
}
