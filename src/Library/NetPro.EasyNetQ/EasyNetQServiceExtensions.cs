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

using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.EasyNetQ
{
    /// <summary>
    /// 
    /// </summary>
    public static class EasyNetQServiceExtensions
    {
        /// <summary>
        /// 依赖注入EasyNetQ服务扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQ(this IServiceCollection services, Func<IServiceProvider, IList<ConnectionString>> connectionFactory)
        {
            services.Replace(ServiceDescriptor.Singleton(sp =>
            {
                var connection = connectionFactory.Invoke(sp);
                var config = sp.GetRequiredService<IConfiguration>();
                var option = new EasyNetQOption(config);
                option!.ConnectionString = connection.ToList();
                return option;
            }));

            return AddEasyNetQ(services);
        }

        /// <summary>
        /// 依赖注入EasyNetQ服务扩展方法
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyNetQ(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceCount = services.Count;
            var easyNetQOption = new EasyNetQOption(configuration);
            services.TryAddSingleton(easyNetQOption);

            if (serviceCount != services.Count)
            {
                return AddEasyNetQ(services);
            }

            return services;
        }

        /// <summary>
        /// 启动EasyNetQ
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddEasyNetQ(this IServiceCollection services)
        {
            //1、The first way
            services.TryAddSingleton<IEasyNetQMulti>(EasyNetQMulti.Instance);

            //2、The second way
            services.TryAddSingleton((sp) =>
            {
                var option = sp.GetRequiredService<EasyNetQOption>();
                var idleBus = new IdleBus<IBus>(TimeSpan.FromSeconds(option.Idle == 0 ? 60 : option.Idle));
                foreach (var item in option.ConnectionString)
                {
                    idleBus.Register(item.Key, () =>
                    {
                        try
                        {
                            var bus = RabbitHutch.CreateBus(item.Value);
                            return bus;
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException($"{ex}");
                        }
                    });
                }

                return idleBus;
            });

            return services;
        }
    }
}
