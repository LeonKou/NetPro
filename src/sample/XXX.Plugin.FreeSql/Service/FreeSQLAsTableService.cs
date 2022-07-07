using FreeSql.Internal.Model;
using IdGen;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XXX.Plugin.FreeSql
{
    public interface IFreeSQLAsTableByDependency
    {
        Task<int> InsertAsync(LogInsertAo input, string dbKey = "sqlite");
        Task<int> UpdateAsync(LogUpdateAo log, string dbKey = "sqlite");
        Task<int> UpdateBatchAsync(LogUpdatePatchAo input, string dbKey = "sqlite");
        Task<int> DeleteAsync(List<string> ids, string dbKey = "sqlite");
        Task<PagedList<Log>> PageQueryAsync(LogSearchAo input, string dbKey = "sqlite");
        string[] CreateTableByTimeRange(CreateLogTable input, string dbKey = "sqlite");
        string CreateTable(Type tableEntity, string dbKey = "sqlite");
    }

    /// <summary>
    /// Freesql分表示例服务
    /// reference：https://github.com/dotnetcore/FreeSql/wiki/%e5%88%86%e8%a1%a8%e5%88%86%e5%ba%93
    /// reference：https://github.com/dotnetcore/FreeSql/discussions/1066
    /// </summary>
    public class FreeSQLAsTableByDependency : IFreeSQLAsTableByDependency, IScopedDependency//通过继承注入接口实现依赖注入
    {
        private readonly ILogger<FreeSQLAsTableByDependency> _logger;
        private readonly IdleBus<IFreeSql> _fsql;
        private readonly IMapper _mapper;
        private static Regex _regTableNameFormat = new Regex(@"\{([^\\}]+)\}");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="idleFsql"></param>
        /// <param name="mapper"></param>
        public FreeSQLAsTableByDependency(ILogger<FreeSQLAsTableByDependency> logger
            , IdleBus<IFreeSql> idleFsql
            , IMapper mapper)
        {
            _logger = logger;
            _fsql = idleFsql;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增分表日志
        /// <param name="input"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public async Task<int> InsertAsync(LogInsertAo input, string dbKey = "sqlite")
        {
            //AO实体映射为数据库DO实体
            var logEntity = _mapper.Map<LogInsertAo, Log>(input);
            logEntity.Id = Extenisons.GenerateIdByDateTime(logEntity.CreateTime);

            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
 
            // Insert方法多个重载，支持单对象、集合对象 
            var sqlatd = freesql.Insert(logEntity).NoneParameter();
            _logger.LogInformation(sqlatd.ToSql());
            var affrows = await sqlatd.ExecuteAffrowsAsync();
            _logger.LogInformation($"影响行数:{affrows}");

            if (affrows == 1)
                _logger.LogInformation("新增成功");
            else
                _logger.LogError("插入失败");
            return affrows;
        }

        /// <summary>
        /// 更新分表日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(LogUpdateAo log, string dbKey = "sqlite")
        {
            int affrows = 0;
            try
            {
                //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
                var freesql = _fsql.Get(dbKey);
                var time = Extenisons.GetDateTimeById(log.Id);
                var dbLog = await freesql.Select<Log>().Where(p => p.Id == log.Id && p.CreateTime.Between(time.Item1, time.Item2)).FirstAsync();
                if (dbLog != null)
                {
                    _mapper.Map(log, dbLog);
                    var sqlatd = freesql.Update<Log>().SetSource(dbLog);
                    _logger.LogInformation(sqlatd.ToSql());
                    affrows = await sqlatd.ExecuteAffrowsAsync();
                    _logger.LogInformation($"影响行数:{affrows}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("更新整个对象失败");
                return 0;
            }

            _logger.LogInformation("更新整个对象成功");
            return affrows;
        }

        /// <summary>
        /// 批量更新分表日志单个值示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<int> UpdateBatchAsync(LogUpdatePatchAo input, string dbKey = "sqlite")
        {
            int affrows = 0;
            try
            {
                //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
                var freesql = _fsql.Get(dbKey);
                var time = Extenisons.GetDateTimeByIds(input.Ids);
                var sqlatd = freesql.Update<Log>(input.Ids).Set(p => p.Content == input.Content).Where(p => p.CreateTime.Between(time.Item1, time.Item2));
                _logger.LogInformation(sqlatd.ToSql());
                affrows = await sqlatd.ExecuteAffrowsAsync();
                _logger.LogInformation($"影响行数:{affrows}");
            }
            catch (Exception ex)
            {
                _logger.LogError("批量更新对象失败");
                return 0;
            }

            _logger.LogInformation("批量更新对象成功");
            return affrows;
        }

        /// <summary>
        /// 批量删除分表日志示例
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(List<string> ids, string dbKey = "sqlite")
        {
            int affrows = 0;
            try
            {
                //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
                var freesql = _fsql.Get(dbKey);
                var time = Extenisons.GetDateTimeByIds(ids);
                var sqlatd = freesql.Delete<Log>(ids).Where(p => p.CreateTime.Between(time.Item1, time.Item2));
                _logger.LogInformation(sqlatd.ToSql());
                affrows = await sqlatd.ExecuteAffrowsAsync();
                _logger.LogInformation($"影响行数:{affrows}");
            }
            catch (Exception ex)
            {
                _logger.LogError("批量删除失败");
                return 0;
            }

            _logger.LogInformation("批量删除成功");
            return affrows;
        }

        /// <summary>
        /// 分页查询分表日志示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<PagedList<Log>> PageQueryAsync(LogSearchAo input, string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            Expression<Func<Log, bool>> where = (b => 1 == 1);
            if (input.StartTime != null)
                where = where.And<Log>(p => p.CreateTime >= DateTimeOffset.FromUnixTimeMilliseconds(input.StartTime.Value).DateTime);
            if (input.EndTime != null)
                where = where.And<Log>(p => p.CreateTime <= DateTimeOffset.FromUnixTimeMilliseconds(input.EndTime.Value).DateTime);
            var freesql = _fsql.Get(dbKey);
            var sqlatd = freesql.Select<Log>()
                .Where(where)
                .OrderBy(p => p.CreateTime)
                .Page(input.PageIndex, input.PageSize)
                .Count(out long totalCount);
            _logger.LogInformation(sqlatd.ToSql());
            var list = await sqlatd.ToListAsync();
            var result = new PagedList<Log>(list, input.PageIndex, input.PageSize, totalCount);
            return result;
        }

        /// <summary>
        /// 根据传入时间批量创建日志表示例
        /// <param name="input"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public string[] CreateTableByTimeRange(CreateLogTable input, string dbKey = "sqlite")
        {
            var freesql = _fsql.Get(dbKey);
            var tb = freesql.CodeFirst.GetTableByEntity(typeof(Log));
            //根据时间查询满足条件的表名，所有表范围：设置开始时间到现在
            string[] names = tb.AsTableImpl.GetTableNamesByColumnValueRange(DateTimeOffset.FromUnixTimeMilliseconds(input.StartTime).DateTime, DateTimeOffset.FromUnixTimeMilliseconds(input.EndTime).DateTime);
            foreach (var name in names)
                 freesql.CodeFirst.SyncStructure(typeof(Log), name);
            return names;
        }

        /// <summary>
        /// 创建下一个月的表
        /// <param name="tableEntity"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public string CreateTable(Type tableEntity,string dbKey = "sqlite")
        {           
            var freesql = _fsql.Get(dbKey);
            var tb = freesql.CodeFirst.GetTableByEntity(tableEntity);
            var tableNameFormat = _regTableNameFormat.Match(tb.DbName);
            var tableName = tb.DbName.Replace(tableNameFormat.Groups[0].Value, DateTimeOffset.UtcNow.AddMonths(1).ToString(tableNameFormat.Groups[1].Value));
            freesql.CodeFirst.SyncStructure(tableEntity, tableName);       
            return tableName;
        }
    }

    /// <summary>
    /// 公共服务
    /// </summary>
    internal static partial class Extenisons
    {
        /// <summary>
        /// 生成时间+唯一Id
        /// 例如 20220101991576137677144064
        /// </summary>
        /// <returns>雪花Id</returns>
        public static string GenerateIdByDateTime(DateTime time)
        {
            var uniqueId = new IdGenerator(0).CreateId().ToString();
            return $"{time.ToString("yyyyMMdd")}{uniqueId}";
        }

        /// <summary>
        /// 将Id转换为开始时间和结束时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static (DateTime, DateTime) GetDateTimeById(string id)
        {
            var startTime = DateTime.Parse($"{id.Substring(0, 4)}-{id.Substring(4, 2)}-{id.Substring(6, 2)}");
            var endTime = startTime.AddDays(1);
            return (startTime, endTime);
        }

        /// <summary>
        /// 将Id列表转换为开始时间和结束时间
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static (DateTime, DateTime) GetDateTimeByIds(List<string> ids)
        {
            ids = ids.OrderBy(p => p).ToList();
            var min = ids.FirstOrDefault();
            var max = ids.LastOrDefault();
            var startTime = DateTime.Parse($"{min.Substring(0, 4)}-{min.Substring(4, 2)}-{min.Substring(6, 2)}");
            var endTime = DateTime.Parse($"{max.Substring(0, 4)}-{max.Substring(4, 2)}-{max.Substring(6, 2)}").AddDays(1);
            return (startTime, endTime);
        }
    }
}
