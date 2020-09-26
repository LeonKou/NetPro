using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace NetPro.MongoDb
{
    public static class MongoDbBuilderExtensions
    {
        /// <summary>
        /// 依赖注入mongodb服务扩展方法
        /// </summary>
        /// <returns></returns>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">Options Action.</param>
        public static IServiceCollection AddMongoDb(this IServiceCollection services, Action<MongoDbOptions> optionsAction)
        {
            services.Configure(optionsAction);
            services.TryAddScoped<IMongoDbContext, MongoDbContext>();

            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var option = configuration.GetSection(nameof(MongoDbOptions)).Get<MongoDbOptions>();
            services.TryAddSingleton(option);
            services.TryAddScoped<IMongoDbContext, MongoDbContext>();

            return services;
        }
    }
}
