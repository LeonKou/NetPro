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

using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NetPro.Globalization
{
    public static class CustomLocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddHybridLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> setupBuiltInAction = null,
            Action<SqlLocalizationOptions> setupSqlAction = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IHybridStringLocalizerFactory, HybridResourceManagerStringLocalizerFactory>();

            services.AddSingleton<IHybridStringLocalizerFactory, HybridSqlStringLocalizerFactory>();
            services.TryAddSingleton<IStringExtendedLocalizerFactory, HybridSqlStringLocalizerFactory>();
            services.TryAddSingleton<DevelopmentSetup>();

            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            services.AddSingleton<IStringLocalizerFactory, CustomStringLocalizerFactory>();

            if (setupBuiltInAction != null) services.Configure(setupBuiltInAction);
            if (setupSqlAction != null) services.Configure(setupSqlAction);

            return services;
        }
    }

    public static class GlobalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddGlobalization(
            this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            var globalization = configuration.GetSection(nameof(Globalization)).Get<Globalization>();

            if (globalization == null)
            {
                globalization = new Globalization();
                globalization.ConnectionString = $"Data Source ={nameof(Globalization)}.db";
                globalization.Cultures = new string[2];
                globalization.Cultures[0] = "zh-CN";
                globalization.Cultures[1] = "en-US";
            }

            using (var connection = new SqliteConnection(globalization.ConnectionString))// "Data Source=LocalizationRecords.sqlite"
            {
                connection.Open();  //  <== The database file is created here.
                using var cmd = new SqliteCommand(@$"
                    CREATE TABLE IF NOT EXISTS ""LocalizationRecords"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_DataEventRecord"" PRIMARY KEY AUTOINCREMENT,
                    ""Key"" TEXT,
                    ""ResourceKey"" TEXT,
                    ""Text"" TEXT,
                    ""LocalizationCulture"" TEXT,
                    ""UpdatedTimestamp"" TEXT NOT NULL
                    )", connection);
                cmd.ExecuteScalar();
            }

            services.AddDbContext<LocalizationModelContext>(options =>
            options.UseSqlite(
            globalization.ConnectionString,
            b => b.MigrationsAssembly("ImportExportLocalization")),
            ServiceLifetime.Singleton,
            ServiceLifetime.Singleton
            );

            // Requires that LocalizationModelContext is defined
            services.AddSqlLocalization(options => options.UseTypeFullNames = true);

            //注册自制的混合国际化服务：
            services.AddHybridLocalization(opts =>
            {
                opts.ResourcesPath = "";//"Resources";
            }, options => options.UseSettings(true, false, true, globalization.Record));
            //services.AddScoped<LanguageActionFilter>();

            //注册mvc
            var builder = services.AddControllers();
            //注册数据注解本地化服务
            if (globalization.Annotations)
            {
                builder.AddDataAnnotationsLocalization();
            }

            return services;
        }
    }

}
