using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Web.Api
{
    /// <summary>
    /// app.UseEndpoints()
    /// </summary>
    public class EndpointsStartup1000 : INetProStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {

        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseEndpoints(s =>
            {
                s.MapControllers();
            });
        }

        public double Order
        {
            //UseEndpoints should be loaded last
            get { return 1000; }
        }
    }
}
