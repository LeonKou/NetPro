using Dapper;
using NetPro.Dapper.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace NetPro.Dapper.Repositories
{
    public partial class DapperRepository<DapperDbContext> : IDapperRepository<DapperDbContext> where DapperDbContext : DapperContext
    {
        #region 事务隔离级别设置

        /// <summary>
        /// 异步脏读
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="acquire"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        protected async Task<T> ReadUncommittedAsync<T>(Func<Task<T>> acquire, bool withNoLock)
        {
            if (withNoLock)
            {
                Execute("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            }
            try
            {
                var result = await acquire();
                return result;
            }
            catch (Exception ex) when (ex.Message == "Sequence contains no elements")//用于处理SingleQuery查询查询不到的异常
            {
                return default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (withNoLock)
                {
                    Execute("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
                }
            }
        }
        #endregion

        #region 新增、修改、删除

        /// <summary>
        /// 插入实体
        /// 主键是int类型，返回主键值，非int类型返回null
        /// </summary>
        /// <param name="entity"></param>
        public async Task<int?> InsertAsync<T>(T entity)
        {
            var result = await Connection.InsertAsync<T>(entity, ActiveTransaction);
            return result;
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T">表实体</typeparam>
        /// <param name="primaryId">主键值</param>
        /// <returns></returns>
        public async Task<T> QueryByIdAsync<T>(object primaryId)
        {
            return await Connection.QueryAsync<T>(primaryId, ActiveTransaction);
        }

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        public async Task<int> InsertBulkAsync<T>(string tableName, string fields, List<T> list)
        {
            string[] res = fields.Split(',');
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = "@" + res[i].Trim();
            }
            string sqlText = $" INSERT INTO {tableName} ({fields}) VALUES ({string.Join(",", res)}); ";

            return await Connection.ExecuteAsync(sqlText, list, transaction: ActiveTransaction);

        }

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        public async Task<int> UpdateAsync<T>(T entity)
        {
            return await Connection.UpdateAsync(entity, transaction: ActiveTransaction);

        }

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        public async Task<int> DeleteAsync<T>(T entity)
        {
            return await Connection.DeleteAsync(entity, transaction: ActiveTransaction);
        }

        public async Task<int> DeleteAsync<T>(object id)
        {
            var result = await Connection.DeleteAsync(id, transaction: ActiveTransaction);
            return result;
        }

        #endregion

        #region list  

        /// <summary>
        ///  原生Query
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public async Task<IList<TAny>> QueryAsync<TAny>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<Task<IList<TAny>>> acquire = (async () =>
            {
                var result = await Connection.QueryAsync<TAny>(query, parameters, ActiveTransaction);
                return result.ToList();
            });

            return await ReadUncommittedAsync(acquire, withNoLock);
        }


        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        public async Task<IList<T>> QueryAllAsync<T>(bool withNoLock = false)
        {
            Func<Task<IList<T>>> acquire = (async () =>
            {
                var data = await Connection.QueryListAsync<T>(ActiveTransaction);
                return data.ToList();
            });

            return await ReadUncommittedAsync(acquire, withNoLock);

        }

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[ 例如：QueryList<User>(new { Age = 10 })]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereConditions"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public async Task<IList<T>> QueryListAsync<T>(object whereConditions, bool withNoLock = false)
        {
            Func<Task<IList<T>>> acquire = (async () =>
            {
                var data = await Connection.QueryListAsync<T>(whereConditions, ActiveTransaction);
                return data.ToList();
            });

            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[ 例如：QueryList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <param name="parame"></param>
        /// <param name="withNoLock"></param>
        /// <returns>受影响的行数</returns>
        public async Task<IList<T>> QueryListAsync<T>(string conditions, DynamicParameters parame, bool withNoLock = false)
        {
            Func<Task<IList<T>>> acquire = (async () =>
            {
                StringBuilder build = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(conditions))
                    build.Append($" where 1=1 And {conditions}");
                var data = await Connection.QueryListAsync<T>(build.ToString(), parame, ActiveTransaction);
                return data.ToList();
            });

            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 分页
        ///  <![CDATA[  例如：QueryListPaged<User>(1,10," age = 10 or Name like '%Smith%'","Name desc")]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <param name="parame"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        /// <remarks>查询条件不带where</remarks>
        public async Task<IPagedList<T>> QueryListPagedAsync<T>(int pageIndex, int pageSize, string conditions, string orderBy, DynamicParameters parame, bool withNoLock = false)
        {
            Func<Task<IPagedList<T>>> acquire = (async () =>
            {
                StringBuilder build = new StringBuilder(" where 1=1 And ");
                build.Append(conditions);
                //总数
                if (parame == null) parame = new DynamicParameters();
                int totalCount = 0;

                var multi = await Connection.QueryListPagedAsync<T>(pageIndex, pageSize, build.ToString(), orderBy, parame, ActiveTransaction);
                var items = await multi.ReadAsync<T>();
                totalCount = multi.Read<int>().First();

                return new PagedList<T>()
                {
                    Items = items.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });

            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 查询分页列表(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        public async Task<IList<T>> QueryListPagedByDynamicAsync<T>(PageFilterDto pageFilterDto)
        {
            var result = await Connection.QueryListPagedByDynamicAsync<T>(pageFilterDto, ActiveTransaction);
            return result.ToList();
        }

        /// <summary>
        /// 查询分页总数(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        public async Task<int> QueryCountByDynamicAsync<T>(PageFilterDto pageFilterDto)
        {
            return await Connection.QueryCountByDynamicAsync<T>(pageFilterDto, ActiveTransaction);
        }
        #endregion

        #region sql语句查询									 

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            return await Connection.ExecuteAsync(query, parameters, ActiveTransaction);
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<Task<T>> acquire = (async () =>
            {
                return await Connection.ExecuteScalarAsync<T>(query, parameters, ActiveTransaction);
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 查询单个对象
        /// 无结果将导致报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns>异常情况:未将对象引用到对象实例，请检查是否查询后的结果包含null值，如若包含请将返回类型置为可空</returns>
        public async Task<T> QuerySingleAsync<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<Task<T>> acquire = (async () =>
            {
                return await Connection.QuerySingleAsync<T>(query, parameters, ActiveTransaction);
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }


        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sqlText, DynamicParameters parameters = null)
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(sqlText, parameters, ActiveTransaction);
        }

        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql">sql语句或者存储过程名称</param>
        /// <param name="param"></param>
        /// <param name="commandType">查询类型(sql,存储过程)</param>
        /// <returns></returns>
        public async Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType? commandType = null)
        {
            return await Connection.ExecuteReaderAsync(sql, param, ActiveTransaction, commandType: commandType);
        }

        #endregion

        #region 存储过程

        public async Task<int> ExecuteProcAsync(string StoredProcedure, object parms = null)
        {
            return await Connection.ExecuteAsync(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程，接收return值
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<int> ExecuteProcWithReturnAsync(string StoredProcedure, DynamicParameters parms = null)
        {
            if (parms == null)
            {
                parms = new DynamicParameters();
            }
            parms.Add("@return_value", 0, DbType.Int32, ParameterDirection.ReturnValue);

            await Connection.ExecuteAsync(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);

            return parms.Get<int>("@return_value");
        }

        /// <summary>
        ///执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = false)
        {
            Func<Task<T>> acquire = (async () =>
            {
                var data = await Connection.ExecuteScalarAsync<T>(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
                return data;
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 执行存储过程，返回list集合
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<IList<T>> QueryProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = false) where T : class
        {
            Func<Task<IList<T>>> acquire = (async () =>
            {
                var data = await Connection.QueryAsync<T>(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
                return data.ToList();
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        public async Task<IPagedList<T>> QueryPagedListProcAsync<T>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = false) where T : class
        {
            Func<Task<IPagedList<T>>> acquire = (async () =>
            {
                var data = await QueryProcAsync<T>(StoredProcedure, parms, withNoLock);
                int totalCount = 0;
                int pageIndex = 0;
                int pageSize = 0;
                if (parms != null)
                {
                    if (!string.IsNullOrWhiteSpace(totalCountName))
                    {
                        totalCount = parms.Get<int>($"@{totalCountName}");
                    }
                    if (!string.IsNullOrWhiteSpace(pageIndexName))
                    {
                        pageIndex = parms.Get<int>($"@{pageIndexName}");
                    }
                    if (!string.IsNullOrWhiteSpace(pageSizeName))
                    {
                        pageSize = parms.Get<int>($"@{pageSizeName}");
                    }
                }
                return new PagedList<T>()
                {
                    Items = data.ToList(),
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<GridReader> QueryProcMultipleAsync(string StoredProcedure, object parms = null)
        {
            Func<Task<GridReader>> acquire = (async () =>
            {
                return await Connection.QueryMultipleAsync(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
            });
            return await ReadUncommittedAsync(acquire, false);
        }

        #endregion
    }
}
