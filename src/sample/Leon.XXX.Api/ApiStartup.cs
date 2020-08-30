using Leon.XXX.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Dapper;
using NetPro.Sign;
using NetPro.TypeFinder;

namespace Leon.XXX.Api
{
    public class ApiStartup : INetProStartup
    {
        public int Order => 900;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();

            //启用请求签名组件
            services.AddVerifySign(s =>
             {
                 //自定义摘要逻辑
                 s.OperationFilter<VerifySignCustomer>();
             });

            //原生注入dapper
            services.AddScoped(s =>
            new DefaultDapperContext(configuration.GetValue<string>("NetProOption:ConnectionStrings:DefaultConnection")
            , DataProvider.Mysql));//原生自带DIf方式注入
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
