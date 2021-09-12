using Localization.SqlLocalizer;
using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

            var sqlConnectionString = globalization?.ConnectionString;

            if (globalization != null)
            {
                services.AddDbContext<LocalizationModelContext>(options =>
            options.UseSqlite(
             sqlConnectionString,
                b => b.MigrationsAssembly("ImportExportLocalization")
            ),
            ServiceLifetime.Singleton,
            ServiceLifetime.Singleton
            );

                // Requires that LocalizationModelContext is defined
                services.AddSqlLocalization(options => options.UseTypeFullNames = true);
            }

            //注册自制的混合国际化服务：
            services.AddHybridLocalization(opts =>
            {
                opts.ResourcesPath = "";//"Resources";
            }, options => options.UseSettings(true, false, true, true));
            //services.AddScoped<LanguageActionFilter>();

            ////注册请求本地化配置：
            //services.Configure<RequestLocalizationOptions>(
            //options =>
            //{
            //    var cultures = globalization?.Cultures
            //    .Select(x => new CultureInfo(x)).ToList();
            //    var supportedCultures = cultures;

            //    var defaultRequestCulture = cultures.FirstOrDefault() ?? new CultureInfo("zh-CN");
            //    options.DefaultRequestCulture = new RequestCulture(culture: null, uiCulture: defaultRequestCulture);

            //    options.DefaultRequestCulture = new RequestCulture("zh-CN");
            //    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            //    options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
            //    options.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());

            //    options.SupportedCultures = supportedCultures;
            //    options.SupportedUICultures = supportedCultures;
            //});

            //注册mvc
            services.AddControllers()
            //注册数据注解本地化服务
            .AddDataAnnotationsLocalization();

            return services;
        }
    }

}
