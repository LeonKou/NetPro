using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace NetPro.Globalization
{
    public class GlobalizationStartup900 : INetProStartup
    {
        public string Description => $"{this.GetType().Namespace} 国际化";
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {
            services.AddGlobalization();
        }

        public void Configure(IApplicationBuilder application)
        {
            //置于app.UseRouting()后便可;
            var configuration = application.ApplicationServices.GetService<IConfiguration>();

            var globalization = configuration.GetSection(nameof(Globalization)).Get<Globalization>();

            var cultures = globalization?.Cultures ?? new string[] { };

            var localizationOptions = new RequestLocalizationOptions()
                .AddSupportedUICultures(cultures)
                .AddSupportedCultures(cultures);

            localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider());
            localizationOptions.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
            application.UseRequestLocalization(localizationOptions);
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            get { return 900; }
        }
    }
}
