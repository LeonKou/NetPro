using HealthChecks.UI.Client;
using NetPro.Checker;
using NetPro.TypeFinder;

namespace XXX.API
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //批量注入
            services.BatchInjection("^XXX.", "Service$"); //批量注入以XXX前缀的程序集，Service结尾的类
            services.BatchInjection("^XXX.", "Repository$");//批量注入以XXX前缀的程序集，Repository结尾的类

            //freesql数据库初始化
            var connectionString = configuration.GetConnectionString("SqliteConnection");
            IFreeSql Fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.Sqlite, connectionString)
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库
            .Build(); //请务必定义成 Singleton 单例模式
            services.AddSingleton<IFreeSql>(Fsql);
            Fsql.Insert(new HealthCheckOptions());
            Fsql.Insert(new HealthCheckOptions());
            var healthbuild = services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseHealthChecks("/health", new HealthCheckOptions()//健康检查服务地址
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            application.UseCheck(envPath: "/env", infoPath: "/info");//envPath:应用环境地址；infoPath:应用自身信息地址
        }
    }
}
