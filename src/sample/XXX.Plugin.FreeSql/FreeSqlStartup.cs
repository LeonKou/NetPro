using FreeSql;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
using System.Text.RegularExpressions;

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
                var sqlConnectionString = configuration.GetConnectionString("SqliteConnection");
                using (var connection = new SQLiteConnection(sqlConnectionString))// "Data Source=LocalizationRecords.sqlite"
                {
                    connection.Open();  //  <== The database file is created here.
                }
                //Register方法注册一个名为sqlite的数据库实例
                return new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, sqlConnectionString)
                .UseAutoSyncStructure(true) //true:自动同步实体结构到数据库
                .Build();
            });

            var mysqlConnection = configuration.GetConnectionString("MysqlConnection");
            using (var connection = new MySqlConnection(mysqlConnection.Replace("Database=netpro_microservice_demo;", "")))// "Data Source=LocalizationRecords.sqlite"
            {
                connection.Open();  //  <== The database file is created here.
                using var cmd = new MySqlCommand(@$"Create Database If Not Exists {_getvaluebyConnectstring(mysqlConnection, "Database")} Character Set UTF8", connection);
                cmd.ExecuteScalar();

                string _getvaluebyConnectstring(string connectionString, string itemName)
                {
                    if (!connectionString.EndsWith(";"))
                        connectionString += ";";

                    string regexStr = itemName + @"\s*=\s*(?<key>.*?);";
                    Regex r = new Regex(regexStr, RegexOptions.IgnoreCase);
                    Match mc = r.Match(connectionString);
                    return mc.Groups["key"].Value;
                }
            }
            fsql.Register("mysql", () =>
            {
                return new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, mysqlConnection)
                .UseAutoSyncStructure(true) //true:自动同步实体结构到数据库；false:默认不迁移数据
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
