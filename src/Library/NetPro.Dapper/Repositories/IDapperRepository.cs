using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace NetPro.Dapper.Repositories
{
    public interface IDapperRepository<DapperDbContext> where DapperDbContext : DapperContext
    {
        /// <summary>
        /// 频繁更改数据库连接对象所设
        /// </summary>
        /// <remarks>用于频繁更改数据库连接对象</remarks>
        DapperDbContext DbContext { get; set; }
        #region 同步

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int? Insert<T>(T entity);

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryId"></param>
        /// <returns></returns>
        T QueryById<T>(object primaryId);

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        int InsertBulk<T>(string tableName, string fields, List<T> list);

        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        int Delete<T>(T entity);

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(object id);

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        int Update<T>(T entity);

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        IList<T> QueryAll<T>(bool withNoLock = false);

        /// <summary>
        /// 依据条件查询数据
        /// <![CDATA[  例如：QueryList<User>(new { Age = 10 })]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        IList<T> QueryList<T>(object whereConditions, bool withNoLock = false);

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[  例如：QueryList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        IList<T> QueryList<T>(string conditions, DynamicParameters parame, bool withNoLock = false);

        /// <summary>
        /// 分页
        ///  <![CDATA[ 例如：QueryListPaged<User>(1,10,"where age = 10 or Name like '%Smith%'","Name desc")]]>  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <param name="parame"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        IPagedList<T> QueryListPaged<T>(int pageIndex, int pageSize, string conditions, string orderBy, DynamicParameters parame, bool withNoLock = false);

        /// <summary>
        ///  原生Query
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        IList<TAny> Query<TAny>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>影响的行数</returns>
        int Execute(string query, object parameters = null);

        /// <summary>
        /// 执行查询单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// 查询单个对象
        /// 无结果将导致报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns>异常情况:未将对象引用到对象实例，请检查是否查询后的结果包含null值，如若包含请将返回类型置为可空</returns>
        T QuerySingle<T>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T QueryFirstOrDefault<T>(string sqlText, DynamicParameters parameters = null);

        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        int ExecuteProc(string StoredProcedure, object parms = null);

        /// <summary>
        /// 执行存储过程并返回int
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns>存储过程返回的int数值</returns>
        int ExecuteProcWithReturn(string StoredProcedure, DynamicParameters parms = null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        T ExecuteScalarProc<T>(string StoredProcedure, object parms = null, bool withNoLock = false);

        /// <summary>
        ///  存储过程返回list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        IList<T> QueryProc<T>(string StoredProcedure, object parms = null, bool withNoLock = true) where T : class;

        /// <summary>
        /// 获取分页集合的存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="totalCountName"></param>
        /// <param name="pageIndexName"></param>
        /// <param name="pageSizeName"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        IPagedList<T> QueryPagedListProc<T>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = false) where T : class;

        /// <summary>
        /// 执行查询的存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        GridReader QueryProcMultiple(string StoredProcedure, object parms = null);

        /// <summary>
        /// 查询分页列表(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        IList<T> QueryListPagedByDynamic<T>(PageFilterDto pageFilterDto);

        /// <summary>
        /// 查询分页总数(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        int QueryCountByDynamic<T>(PageFilterDto pageFilterDto);
        #endregion

        #region 异步	

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int?> InsertAsync<T>(T entity);

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        Task<int> InsertBulkAsync<T>(string tableName, string fields, List<T> list);

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        Task<int> DeleteAsync<T>(T entity);

        Task<T> QueryByIdAsync<T>(object primaryId);

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(object id);

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        Task<int> UpdateAsync<T>(T entity);

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        Task<IList<T>> QueryAllAsync<T>(bool withNoLock = false);

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[例如：QueryListAsync<User>(new { Age = 10 })]]>  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        Task<IList<T>> QueryListAsync<T>(object whereConditions, bool withNoLock = false);

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[ 例如：QueryListAsync<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]>  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        Task<IList<T>> QueryListAsync<T>(string conditions, DynamicParameters parame, bool withNoLock = false);

        /// <summary>
        /// 分页
        ///  <![CDATA[ 例如：QueryListPagedAsync<User>(1,10,"where age = 10 or Name like '%Smith%'","Name desc")]]>  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <param name="parame"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<IPagedList<T>> QueryListPagedAsync<T>(int pageIndex, int pageSize, string conditions, string orderBy, DynamicParameters parame, bool withNoLock = false);

        /// <summary>
        ///  原生Query
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<IList<TAny>> QueryAsync<TAny>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>影响的行数</returns>
        Task<int> ExecuteAsync(string query, object parameters = null);

        /// <summary>
        /// 执行查询单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// 查询单个对象
        /// 无结果将导致报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns>异常情况:未将对象引用到对象实例，请检查是否查询后的结果包含null值，如若包含请将返回类型置为可空</returns>
        Task<T> QuerySingleAsync<T>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sqlText, DynamicParameters parameters = null);

        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType? commandType = null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        Task<int> ExecuteProcAsync(string StoredProcedure, object parms = null);

        /// <summary>
        /// 执行存储过程并返回int
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns>存储过程返回的int数值</returns>
        Task<int> ExecuteProcWithReturnAsync(string StoredProcedure, DynamicParameters parms = null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = false);

        /// <summary>
        ///  存储过程返回list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<IList<T>> QueryProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = true) where T : class;

        /// <summary>
        /// 获取分页集合的存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="totalCountName"></param>
        /// <param name="pageIndexName"></param>
        /// <param name="pageSizeName"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<IPagedList<T>> QueryPagedListProcAsync<T>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = false) where T : class;

        /// <summary>
        /// 执行查询的存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        Task<GridReader> QueryProcMultipleAsync(string StoredProcedure, object parms = null);


        /// <summary>
        /// 查询分页列表(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        Task<IList<T>> QueryListPagedByDynamicAsync<T>(PageFilterDto pageFilterDto);

        /// <summary>
        /// 查询分页总数(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        Task<int> QueryCountByDynamicAsync<T>(PageFilterDto pageFilterDto);
        #endregion
    }
}
