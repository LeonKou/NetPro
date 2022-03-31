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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.MongoDb;
using System.Linq;

/// <summary>
/// mongodb扩展
/// </summary>
public static class MongoDbBuilderExtensions
{
    public static IMongoDbBuilder AddMongoDb<GetConnections>(this IServiceCollection services) where GetConnections : class, IConnectionsFactory
    {
        services.AddTransient<IConnectionsFactory, GetConnections>();
        return new MongoDbBuilder(services);
    }

    public static IMongoDbBuilder AddMongoDb(this IServiceCollection services)
    {
        //如已经注册自定义服务，跳过
        var serviceProviderIsService = services.BuildServiceProvider().GetRequiredService<IServiceProviderIsService>();
        if (!serviceProviderIsService.IsService(typeof(IConnectionsFactory)))
        {
            services.AddTransient<IConnectionsFactory, DefaultConnections>();
        }

        return new MongoDbBuilder(services);
    }

    /// <summary>
    /// 构造MongoDb实例
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection Build(
       this IMongoDbBuilder builder,
       IConfiguration configuration)
    {
        var option = new MongoDbOption(configuration);
        builder.Services.AddSingleton(option);
        return builder.AddMongoDb(option);
    }

    /// <summary>
    ///  依赖注入mongodb服务扩展方法
    /// </summary>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    private static IServiceCollection AddMongoDb(this IMongoDbBuilder builder, MongoDbOption optionsAction)
    {
        var connectionsFactory = builder.Services.BuildServiceProvider().GetRequiredService<IConnectionsFactory>();
        if (connectionsFactory is not DefaultConnections)
        {
            optionsAction.ConnectionString = connectionsFactory.GetConnectionStrings().ToList();
            builder.Services.AddSingleton(optionsAction);//用最新连接串覆盖
        }

        MongoDBMulti.MongoDbOption = optionsAction;
        builder.Services.AddSingleton(optionsAction);
        builder.Services.AddSingleton<IMongoDBMulti>(MongoDBMulti.Instance);
        return builder.Services;
    }
}
