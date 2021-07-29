using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using System.Collections.Generic;
using System.Linq;

namespace Language.Resoureces
{
    public class GlobalizationStartup : INetProStartup
    {
        public int Order => 299;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.AddLocalization(options => options.ResourcesPath = "");
        }

        public void Configure(IApplicationBuilder application)
        {
            var supportedCultures = new[] { "en-us", "zh-CN" };
            var localizationOptions = new RequestLocalizationOptions()
                .AddSupportedUICultures(supportedCultures)
                ;

            application.UseRequestLocalization(localizationOptions);
        }
    }
}
