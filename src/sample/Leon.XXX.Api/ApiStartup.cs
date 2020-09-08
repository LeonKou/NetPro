using Leon.XXX.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.Sign;
using NetPro.TypeFinder;

namespace Leon.XXX.Api
{
    public class ApiStartup : INetProStartup
    {
        public int Order => 900;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();

            //启用请求签名组件
            services.AddVerifySign(s =>
             {
                 //自定义摘要逻辑
                 s.OperationFilter<VerifySignCustomer>();
             });
            var connectionString = configuration.GetValue<string>("ConnectionStrings:MysqlConnection");
            Fsql = new FreeSql.FreeSqlBuilder()
         .UseConnectionString(FreeSql.DataType.MySql, connectionString)
         .UseAutoSyncStructure(false) //自动同步实体结构到数据库
         .Build(); //请务必定义成 Singleton 单例模式
            services.AddSingleton<IFreeSql>(Fsql);

            services.AddFreeRepository(null,
           this.GetType().Assembly);
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
