using HealthChecks.UI.Client;
using NetPro.Checker;
using NetPro.TypeFinder;

namespace XXX.API
{
    /// <summary>
    /// 自定义启动类
    /// </summary>
    public class ApiStartup : INetProStartup
    {
        /// <summary>
        /// 执行顺序
        /// </summary>
        public double Order { get; set; } = int.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //批量注入(可正则匹配注入所有子程序集，也可在每个子程序集中独立注入)
            services.BatchInjection("^XXX.", "Service$"); //批量注入以XXX前缀的程序集，Service结尾的类
            services.BatchInjection("^XXX.", "Repository$");//批量注入以XXX前缀的程序集，Repository结尾的类

            #region Freesql初始化
            //多数据库初始化
            var fsql = new MultiFreeSql();

            //reference：https://github.com/dotnetcore/FreeSql/issues/44
            //第一个注册的实例是默认实例，使用时如没指定dbkey则默认连接此处第一个注册的数据库实例
            fsql.Register("sqlite", () =>
            {
                //Register方法注册一个名为sqlite的数据库实例
                return new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, configuration.GetConnectionString("SqliteConnection"))
                .UseAutoSyncStructure(true) //true:自动同步实体结构到数据库
                .Build();
            });

            fsql.Register("mysql", () =>
            {
                //Register方法注册一个名为sqlite的数据库实例
                return new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, configuration.GetConnectionString("MysqlConnection"))
                .UseAutoSyncStructure(false) //true:自动同步实体结构到数据库；false:默认不迁移数据
                .Build();
            });

            //可注册多个fsql.Register("db2",()=>{})...

            services.AddSingleton<IFreeSql>(fsql);
            #endregion

            //services.AddHealthChecks();
        }

        /// <summary>
        /// 请求管道配置
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            ////健康检查(应该限制外网访问或者注释此段)
            //application.UseHealthChecks("/health", new HealthCheckOptions()//健康检查服务地址
            //{
            //    Predicate = _ => true,
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});

            ////环境检查(应该限制外网访问或者注释此段)
            //application.UseCheck(envPath: "/env", infoPath: "/info");//envPath:应用环境地址；infoPath:应用自身信息地址
        }
    }
}
