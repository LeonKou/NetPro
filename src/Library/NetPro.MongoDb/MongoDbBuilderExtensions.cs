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
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetPro.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// mongodb扩展
/// </summary>
public static class MongoDbBuilderExtensions
{
    /// <summary>
    /// 依赖注入MongoDb服务扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionFactory"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, Func<IServiceProvider, IList<ConnectionString>> connectionFactory)
    {
        services.Replace(ServiceDescriptor.Singleton(sp =>
        {
            var connection = connectionFactory.Invoke(sp);
            var config = sp.GetRequiredService<IConfiguration>();
            var option = new MongoDbOption(config);
            option!.ConnectionString = connection.ToList();
            return option;
        }));

        return services;
    }

    /// <summary>
    /// 依赖注入MongoDb服务扩展方法
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbOptions = new MongoDbOption(configuration);
        services.TryAddSingleton(mongoDbOptions);
        services.TryAddSingleton<IMongoDBMulti>(MongoDBMulti.Instance);

        return services;
    }
}
