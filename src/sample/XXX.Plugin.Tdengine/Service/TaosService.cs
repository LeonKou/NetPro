using Maikebing.Data.Taos;
using System.Text;

namespace XXX.Plugin.Tdengine
{
    public interface ITaosService
    {
        Task CreateDatabaseAsync(DatabaseConfig database, string dbKey = "taos1");
        Task CreateSuperTableAsync(SuperTable superTable, string dbKey = "taos1");
        Task InsertAsync(TaosAo taosAo, string dbKey = "taos1");
    }

    public class TaosService : ITaosService
    {
        private readonly IdleBus<TaosConnection> _taosdbMulti;
        private readonly ITaosProxy _taosProxy;
        public TaosService(IdleBus<TaosConnection> taosdbMulti,
            ITaosProxy taosProxy)
        {
            _taosdbMulti = taosdbMulti;
            _taosProxy = taosProxy;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="taosAo"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public async Task InsertAsync(TaosAo taosAo, string dbKey = "taos1")
        {
            // INSERT INTO d1001 USING meters TAGS ("Beijng.Chaoyang", 2) VALUES (now, 10.2, 219, 0.3

            //根据设备找到合适数据库，
            //根据设备找到合适超级表，
            //根据超级表元数据进行准确的数据插入
            var sql = @$"
                       INSERT INTO power.device_{"db001"} 
                       USING {dbKey}.meters
                       TAGS ('device_{taosAo.DeviceId}') 
                       VALUES({DateTimeOffset.Now.ToUnixTimeMilliseconds()},{taosAo.att[0].Value})(1546272060000,72)";

            var taos = _taosdbMulti.Get(dbKey);
            var command = taos.CreateCommand(sql);
            var reader = await command.ExecuteReaderAsync();

            //var result = await _taosProxy.ExecuteSql(sql, "test");
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(DatabaseConfig database, string dbKey = "taos1")
        {
            var sql = @$"
                      CREATE DATABASE {database.Database} 
                      KEEP {database.Keep} 
                      DAYS {database.Days} 
                      BLOCKS {database.Blocks} 
                      UPDATE {database.AllowUpdate}";

            var taos = _taosdbMulti.Get(dbKey);
            var command = taos.CreateCommand(sql);
            var reader = await command.ExecuteReaderAsync();

            var result = await _taosProxy.ExecuteSql(sql, "test");
            //记录存储数据库与设备关系，创建超级表和插入数据时需关联到合适的数据库，例如状态类设备灯源，开关存储在存储周期较短的数据库
        }

        /// <summary>
        /// 创建超级表
        /// </summary>
        /// <param name="superTable"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public async Task CreateSuperTableAsync(SuperTable superTable, string dbKey = "taos1")
        {
            //记录存储超级表元数据，用于后续插入数据时找到超级表元数据
            //例如 CREATE STABLE meters (ts timestamp, current float, voltage int, phase float) TAGS (location binary(64), groupId int);

            var stringBuilder = new StringBuilder($"( {superTable.TimestampName} timestamp, ");

            var fieldCount = superTable.Fields.Count();
            for (int i = 0; i < fieldCount; i++)
            {
                if (i == fieldCount - 1)
                    stringBuilder.Append($"{superTable.Fields[i]} )");
                stringBuilder.Append($"{superTable.Fields[i]},");
            }
            var tagsCount = superTable.Tags.Count();
            for (int i = 0; i < superTable.Tags.Count(); i++)
            {
                if (i == 0)
                    stringBuilder.Append($" TAGS ( {superTable.Tags[i]} ,");
                else if (i == tagsCount - 1)
                    stringBuilder.Append($"{superTable.Tags[i]} )");
                else
                    stringBuilder.Append($"{superTable.Tags[i]},");
            }

            var sql = @$"
                       CREATE STABLE {superTable.TableName} 
                       {stringBuilder};";

            //var taos = _taosdbMulti.Get(dbKey);
            //var command = taos.CreateCommand(sql);
            //var reader = await command.ExecuteReaderAsync();

            var result = await _taosProxy.ExecuteSql(sql, "test");
        }
    }
}
