using Leon.XXX.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Dapper;
using NetPro.Sign;

namespace Leon.XXX.Api
{
    public class ApiStartup : INetProStartup
    {
        public int Order => 900;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //启用请求签名组件
            services.AddVerifySign(s =>
            {
                //自定义摘要逻辑
                s.OperationFilter<VerifySignCustomer>();
            });

            //services.AddTransient(s =>
            //new DefaultDapperContext(configuration.GetValue<string>("DatabaseConnection")
            //, DataProvider.Mysql));//原生自带DIf方式注入
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
