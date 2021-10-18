using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Core.Infrastructure.Mapper;
using NetPro.TypeFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace NetPro
{
    /// <summary>
    /// 
    /// </summary>
    public class EngineContextStartup : INetProStartup
    {
        public double Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //放最后注入可尽量包含其他注入的服务
            var engine = EngineContext.Create();
            engine.ConfigureServices(services, configuration);

        }
    }
}
