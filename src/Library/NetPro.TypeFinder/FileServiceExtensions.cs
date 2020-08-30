using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.TypeFinder
{
    public static class FileServiceExtensions
    {
        public static IServiceCollection AddFileProcessService(this IServiceCollection services)
        {
            var hostEnvironment = services.BuildServiceProvider().GetRequiredService<IHostEnvironment>();

            //create default file provider
            CoreHelper.DefaultFileProvider = new NetProFileProvider(hostEnvironment);
            services.AddSingleton<ITypeFinder>(s => new WebAppTypeFinder());
            services.AddScoped<INetProFileProvider, NetProFileProvider>();
            return services;
        }
    }
}
