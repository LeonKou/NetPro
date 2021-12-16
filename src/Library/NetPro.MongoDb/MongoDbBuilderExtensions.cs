using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetPro.MongoDb;
using System;

/// <summary>
/// mongodb扩展
/// </summary>
public static class MongoDbBuilderExtensions
{
    /// <summary>
    ///  依赖注入mongodb服务扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoDbOption optionsAction)
    {
        MongoDBMulti.MongoDbOption = optionsAction;
        services.AddSingleton(optionsAction);
        services.AddSingleton(MongoDBMulti.Instance);
        return services;
    }

    /// <summary>
    ///  依赖注入mongodb服务扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbOptions = new MongoDbOption(configuration);
        MongoDBMulti.MongoDbOption = mongoDbOptions;
        services.AddSingleton(mongoDbOptions);
        services.AddSingleton(MongoDBMulti.Instance);

        return services;
    }
}
