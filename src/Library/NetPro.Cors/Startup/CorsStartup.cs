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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.NetPro;

namespace NetPro.Cors
{
    /// <summary>
    /// 跨域
    /// application.UseCors();
    /// </summary>
    internal sealed class CorsStartup : INetProStartup, System.NetPro.Startup.__._
    {
        bool enabled = true;

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <param name="typeFinder">typeFinder</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            ILogger logger = null;
            if (loggerFactory != null)
            {
                logger = loggerFactory.CreateLogger($"{nameof(CorsStartup)}");
            }

            var netProCorsOption = configuration.GetSection(nameof(NetProCorsOption)).Get<NetProCorsOption>();
            if (netProCorsOption != null && !string.IsNullOrWhiteSpace(netProCorsOption.CorsOrigins))
                services.AddSingleton(netProCorsOption);
            else
            {
                logger.LogWarning($"allow Cors disabled,because netProCorsOption:CorsOrigins is null");
                enabled = false;
                return;
            }

            string[] ToArray(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return new string[] { };
                var array = input.Split(',');
                return array;
            }

            var corsOrigins = ToArray(netProCorsOption.CorsOrigins).ToArray();

            //支持跨域访问
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", (builder) =>
                {
                    if (!string.IsNullOrEmpty(netProCorsOption.CorsOrigins) && netProCorsOption.CorsOrigins == "*")//所有域名都可跨域
                    {
                        builder = builder.SetIsOriginAllowed((host) => true);
                        logger?.LogInformation($"全局跨域已开启");
                    }
                    else
                    {
                        builder = builder.WithOrigins(corsOrigins);
                        logger?.LogInformation($"指定域名{string.Join(',', corsOrigins)}跨域已开启");
                    }
                    builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       //.WithMethods("GET", "POST")
                       .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (enabled)
                application.UseCors("CorsPolicy");
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// Cors should be enabled after routing
        /// </summary>
        public double Order { get; set; } = 300; //Cors should be enabled after routing
    }
}
