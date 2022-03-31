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
using System;
using System.Linq;

namespace NetPro.EasyNetQ
{
    /// <summary>
    /// 
    /// </summary>
    public static class EasyNetQServiceExtensions
    {
        /// <summary>
        /// 自定义连接串策略方式注册
        /// </summary>
        /// <typeparam name="GetConnections"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEasyNetQBuilder AddEasyNetQ<GetConnections>(this IServiceCollection services) where GetConnections : class, IConnectionsFactory
        {
            services.AddTransient<IConnectionsFactory, GetConnections>();
            return new EasyNetQBuilder(services);
        }

        /// <summary>
        ///  默认本地连接串策略方式注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEasyNetQBuilder AddEasyNetQ(this IServiceCollection services)
        {
            //如已经注册自定义服务，跳过
            var serviceProviderIsService = services.BuildServiceProvider().GetRequiredService<IServiceProviderIsService>();
            if (!serviceProviderIsService.IsService(typeof(IConnectionsFactory)))
            {
                services.AddTransient<IConnectionsFactory, DefaultConnections>();
            }
            return new EasyNetQBuilder(services);
        }

        /// <summary>
        /// 构造EasyNetQ实例
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection Build(
           this IEasyNetQBuilder builder,
           IConfiguration configuration)
        {
            var option = new EasyNetQOption(configuration);
            builder.Services.AddSingleton(option);
            return builder.AddEasyNetQ(option);
        }

        /// <summary>
        /// 启动EasyNetQ
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddEasyNetQ(this IEasyNetQBuilder builder, EasyNetQOption option)
        {
            // connectionstring format "host=192.168.18.129;virtualHost=/;username=admin;password=123456;timeout=60"

            var connectionsFactory = builder.Services.BuildServiceProvider().GetRequiredService<IConnectionsFactory>();
            if (connectionsFactory is not DefaultConnections)
            {
                option.ConnectionString = connectionsFactory.GetConnectionStrings().ToList();
                builder.Services.AddSingleton(option);//用最新连接串覆盖
            }
            //1、The first way
            builder.Services.AddSingleton<IEasyNetQMulti>(EasyNetQMulti.Instance);

            //2、The second way
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
            builder.Services.AddSingleton(idleBus);

            return builder.Services;
        }
    }
}
