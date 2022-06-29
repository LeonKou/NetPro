using FreeSql;
using IdGen;
using IdGen.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using DataType = FreeSql.DataType;

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
            //注册Id生成器
            services.AddIdGen(0, () => IdGeneratorOptions.Default);

            #region Freesql初始化
            //多数据库初始化
            var idleBus = new IdleBus<IFreeSql>(TimeSpan.FromSeconds(60));

            //reference：https://github.com/dotnetcore/FreeSql/issues/44
            //第一个注册的实例是默认实例，使用时如没指定dbkey则默认连接此处第一个注册的数据库实例
            idleBus.Register("sqlite", () =>
            {
                var sqlConnectionString = configuration.GetConnectionString("SqliteConnection");

                using (var connection = new SQLiteConnection(sqlConnectionString))// "Data Source=LocalizationRecords.sqlite"
                {
                    connection.Open();  //  <== The database file is created here.
                }
                return new FreeSqlBuilder()
             .UseConnectionString(DataType.Sqlite, sqlConnectionString)
             .UseAutoSyncStructure(true) //true:自动同步实体结构到数据库；false:默认不迁移数据
             .Build();
            });
            idleBus.Register("mysql", () =>
            {
                var mysqlConnection = configuration.GetConnectionString("MysqlConnection");

                using (var connection = new MySqlConnection(mysqlConnection.Replace("Database=netpro_microservice_demo;", "").Replace("database=netpro_microservice_demo;", "")))// "Data Source=LocalizationRecords.sqlite"
                {
                    bool valid = true;
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        valid = false;
                        Console.WriteLine($"mysql连接错误--{ex.Message}");
                    }
                    if (valid)
                    {
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
                        connection.Close();
                    }
                }
                return new FreeSqlBuilder()
             .UseConnectionString(DataType.MySql, mysqlConnection)
             .UseAutoSyncStructure(true) //true:自动同步实体结构到数据库；false:默认不迁移数据
             .Build();
            });
            services.AddSingleton(idleBus);

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
