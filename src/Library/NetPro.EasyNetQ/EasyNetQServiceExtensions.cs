using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.NetPro
{
    /// <summary>
    /// 
    /// </summary>
    public static class EasyNetQServiceExtensions
    {
        /// <summary>
        /// 启动EasyNetQ
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection AddEasyNetQ(this IServiceCollection services, IConfiguration configuration)
        {
            // connectionstring format "host=192.168.18.129;virtualHost=/;username=admin;password=123456;timeout=60"
            //1、The first way
            services.AddSingleton(EasyNetQMulti.Instance);
            //2、The second way
            var option = new EasyNetQOption(configuration);
            services.AddSingleton(option);

            var idleBus = new IdleBus<IBus>(TimeSpan.FromMinutes(10));
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
            services.AddSingleton(idleBus);

            return services;
        }
    }
}
