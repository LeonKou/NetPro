using FreeSql;

namespace XXX.Plugin.FreeSql
{
    public class FreeSqlStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="typeFinder"></param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
