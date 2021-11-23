using HealthChecks.UI.Client;
using NetPro.Checker;
using NetPro.TypeFinder;

namespace XXX.API
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //批量注入 Service 后缀的类
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("XXX.API")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            //批量注入 Repository 后缀的类
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("XXX.API")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            var connectionString = configuration.GetConnectionString("MysqlConnection");
            Fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            .UseAutoSyncStructure(false) //自动同步实体结构到数据库
            .Build(); //请务必定义成 Singleton 单例模式
            services.AddSingleton<IFreeSql>(Fsql);

            services.AddFreeRepository(null,
           this.GetType().Assembly);//批量注入Repository


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
