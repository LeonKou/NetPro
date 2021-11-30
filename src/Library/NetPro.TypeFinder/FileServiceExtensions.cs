using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace NetPro.TypeFinder
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="typeFinderOption"></param>
        /// <returns></returns>
        public static IServiceCollection AddFileProcessService(this IServiceCollection services, TypeFinderOption typeFinderOption = null)
        {
            services.AddSingleton(new TypeFinderOption { MountePath = typeFinderOption?.MountePath });

            //var hostEnvironment = services.BuildServiceProvider().GetRequiredService<IHostEnvironment>();

            //create default file provider
            //CoreHelper.DefaultFileProvider = new NetProFileProvider(hostEnvironment);
            //services.AddSingleton<ITypeFinder>(s => new WebAppTypeFinder());
            services.AddSingleton<ITypeFinder, WebAppTypeFinder>();
            //services.AddScoped<INetProFileProvider, NetProFileProvider>();
            services.AddSingleton<INetProFileProvider, NetProFileProvider>();
            return services;
        }
    }
}
