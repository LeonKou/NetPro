using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.NetPro;

namespace NetPro.Globalization
{
    /// <summary>
    /// 全局多语言支持
    ///  app.UseRequestLocalization()
    /// </summary>
    internal sealed class GlobalizationStartup : INetProStartup, System.NetPro.Startup.__._
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddGlobalization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //置于app.UseRouting()后便可;
            var configuration = app.ApplicationServices.GetService<IConfiguration>();

            var globalization = configuration.GetSection(nameof(Globalization)).Get<Globalization>();

            var cultures = globalization?.Cultures ?? new string[] { };

            var localizationOptions = new RequestLocalizationOptions()
                .AddSupportedUICultures(cultures)
                //.AddSupportedCultures(cultures)
                ;
            localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider { UIQueryStringKey = globalization?.UIQueryStringKey ?? "language" });
            localizationOptions.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Insert(2, new CookieRequestCultureProvider());
            app.UseRequestLocalization(localizationOptions);
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public double Order { get; set; } = 900;
    }
}
