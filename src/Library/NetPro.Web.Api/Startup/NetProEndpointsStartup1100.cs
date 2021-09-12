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
    /// NetProEndpointsStartup1100
    /// </summary>
    public class NetProEndpointsStartup1100 : INetProStartup
    {
        public string Description => $"{this.GetType().Namespace} 支持UseEndpoints配置";
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

        public int Order
        {
            //UseEndpoints should be loaded last
            get { return 1100; }
        }
    }
}
