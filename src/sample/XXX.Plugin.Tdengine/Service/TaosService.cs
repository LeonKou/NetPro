using Maikebing.Data.Taos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public TaosService(IdleBus<TaosConnection> taosdbMulti)
        {
            _taosdbMulti = taosdbMulti;
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
            var taos = _taosdbMulti.Get(dbKey);
            var command = taos.CreateCommand(@$"
                                 INSERT INTO  {taosAo.DeviceId} 
                                 USING {"meters"} TAGS (mongo, 67) 
                                 values ( 1608173534840 2 false 'Channel1.窑.烟囱温度' '烟囱温度' '122.00' );");
            var reader = await command.ExecuteReaderAsync();
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="database"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(DatabaseConfig database, string dbKey = "taos1")
        {
            var taos = _taosdbMulti.Get(dbKey);
            var command = taos.CreateCommand(@$"
                                 CREATE DATABASE {database.Database} 
                                 KEEP {database.Keep} 
                                 DAYS {database.Database} 
                                 BLOCKS {database.Blocks} 
                                 UPDATE {database.AllowUpdate};");
            var reader = await command.ExecuteReaderAsync();
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
            var taos = _taosdbMulti.Get(dbKey);
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

            string fields = string.Empty;
            var command = taos.CreateCommand(@$"
                                 CREATE STABLE {superTable.TableName} 
                                  {stringBuilder};");
            var reader = await command.ExecuteReaderAsync();
        }
    }
}
