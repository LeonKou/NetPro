using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
