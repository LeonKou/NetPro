using Dapper;
using NetPro.Dapper.Expressions;
using NetPro.Dapper.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using static Dapper.SqlMapper;

namespace NetPro.Dapper.Repositories
{
    public partial class DapperRepository<DapperDbContext> : IDapperRepository<DapperDbContext> where DapperDbContext : DapperContext
    {
        //在此可以动态更改数据库连接
        public virtual DapperDbContext DbContext { get; set; }
        public DapperRepository(DapperDbContext context)
        {
            DbContext = context;
        }

        public virtual DbConnection Connection
        {
            get
            {
                return DbContext.Connection as DbConnection;
            }
            set
            {
                DbContext.Connection = value;
            }
        }

        public virtual DbTransaction ActiveTransaction
        {
            get { return DbContext.ActiveTransaction as DbTransaction; }
        }

        #region 事务隔离级别设置
        /// <summary>
        /// 脏读
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="acquire"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        protected T ReadUncommitted<T>(Func<T> acquire, bool withNoLock)
        {
            if (withNoLock)
            {
                Execute("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            }
            try
            {
                return acquire();
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

        #region (同步)新增、修改、删除

        /// <summary>
        /// 插入实体
        /// 主键是int类型，返回主键值，非int类型返回null
        /// </summary>
        /// <param name="entity"></param>
        public int? Insert<T>(T entity)
        {
            return Connection.Insert<T>(entity, ActiveTransaction);
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T">表实体</typeparam>
        /// <param name="primaryId">主键值</param>
        /// <returns></returns>
        public T QueryById<T>(object primaryId)
        {
            return Connection.Get<T>(primaryId, ActiveTransaction);
        }

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <typeparam name="T">数据库表对应的实体类型</typeparam>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        public int InsertBulk<T>(string tableName, string fields, List<T> list)
        {
            string[] res = fields.Split(',');
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = "@" + res[i].Trim();
            }
            string sqlText = string.Format(" INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, fields, string.Join(",", res));

            return Connection.Execute(sqlText, list, transaction: ActiveTransaction);

        }

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        public int Update<T>(T entity)
        {
            return Connection.Update(entity, transaction: ActiveTransaction);

        }

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        public int Delete<T>(T entity)
        {
            return Connection.Delete(entity, transaction: ActiveTransaction);

        }

        public int Delete(object id)
        {
            return Connection.Delete(id, transaction: ActiveTransaction);

        }

        #endregion

        #region (同步)list  

        /// <summary>
        ///  原生Query
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public IList<TAny> Query<TAny>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<IList<TAny>> acquire = (() =>
            {
                return Connection.Query<TAny>(query, parameters, ActiveTransaction).ToList();
            });

            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>受影响的行数</returns>
        public IList<T> QueryAll<T>(bool withNoLock = false)
        {
            Func<IList<T>> acquire = (() =>
            {
                var data = Connection.GetList<T>(ActiveTransaction);
                return data.ToList();
            });

            return ReadUncommitted(acquire, withNoLock);

        }

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[  例如：QueryList<User>(new { Age = 10 })]]> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereConditions"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public IList<T> QueryList<T>(object whereConditions, bool withNoLock = false)
        {
            Func<IList<T>> acquire = (() =>
            {
                var data = Connection.GetList<T>(whereConditions, ActiveTransaction);
                return data.ToList();
            });

            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 依据条件查询数据
        /// <![CDATA[例如：QueryList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <param name="parame"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public IList<T> QueryList<T>(string conditions, DynamicParameters parame, bool withNoLock = false)
        {
            Func<IList<T>> acquire = (() =>
            {
                StringBuilder build = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(conditions))
                    build.Append($" where 1=1 And {conditions}");
                var data = Connection.GetList<T>(build.ToString(), parame, ActiveTransaction);
                return data.ToList();
            });

            return ReadUncommitted(acquire, withNoLock);
        }

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
        public IPagedList<T> QueryListPaged<T>(int pageIndex, int pageSize, string conditions, string orderBy, DynamicParameters parame, bool withNoLock = false)
        {
            Func<IPagedList<T>> acquire = (() =>
            {
                StringBuilder build = new StringBuilder(" where 1=1 And ");
                build.Append(conditions);
                //总数
                if (parame == null) parame = new DynamicParameters();
                int totalCount = 0;

                var multi = Connection.GetListPaged<T>(pageIndex, pageSize, build.ToString(), orderBy, parame, ActiveTransaction);

                var items = multi.Read<T>();
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

            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 查询分页列表(动态传参)暂时Mysql
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        public IList<T> QueryListPagedByDynamic<T>(PageFilterDto pageFilterDto)
        {
            return Connection.GetListPagedByDynamic<T>(pageFilterDto, ActiveTransaction).ToList();
        }

        /// <summary>
        /// 查询分页总数(动态传参)暂时Mysql
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        public int QueryCountByDynamic<T>(PageFilterDto pageFilterDto)
        {
            return Connection.GetCountByDynamic<T>(pageFilterDto, ActiveTransaction);
        }

        #endregion

        #region (同步)sql语句查询

        public int Execute(string query, object parameters = null)
        {
            return Connection.Execute(query, parameters, ActiveTransaction);
        }

        public T ExecuteScalar<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<T> acquire = (() =>
            {
                return Connection.ExecuteScalar<T>(query, parameters, ActiveTransaction);
            });
            return ReadUncommitted(acquire, withNoLock);
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
        public T QuerySingle<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<T> acquire = (() =>
            {
                return Connection.QuerySingle<T>(query, parameters, ActiveTransaction);
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sqlText, DynamicParameters parameters = null)
        {
            return Connection.QueryFirstOrDefault<T>(sqlText, parameters, ActiveTransaction);
        }

        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (transaction == null)
            {
                return Connection.ExecuteReader(sql, param, ActiveTransaction, commandTimeout, commandType);
            }
            return Connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
        }

        #endregion

        #region (同步)存储过程


        public int ExecuteProc(string StoredProcedure, object parms = null)
        {
            return Connection.Execute(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程，接收return值
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public int ExecuteProcWithReturn(string StoredProcedure, DynamicParameters parms = null)
        {
            if (parms == null)
            {
                parms = new DynamicParameters();
            }
            parms.Add("@return_value", 0, DbType.Int32, ParameterDirection.ReturnValue);

            Connection.Execute(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);

            return parms.Get<int>("@return_value");
        }

        /// <summary>
        ///执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public T ExecuteScalarProc<T>(string StoredProcedure, object parms = null, bool withNoLock = false)
        {
            Func<T> acquire = (() =>
            {
                var data = Connection.ExecuteScalar<T>(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
                return data;
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 执行存储过程，返回list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<T> QueryProc<T>(string StoredProcedure, object parms = null, bool withNoLock = true) where T : class
        {
            Func<IList<T>> acquire = (() =>
            {
                return Connection.Query<T>(StoredProcedure, parms, ActiveTransaction, true, null, CommandType.StoredProcedure).ToList();
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        public IPagedList<T> QueryPagedListProc<T>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = false) where T : class
        {
            Func<IPagedList<T>> acquire = (() =>
            {
                var data = QueryProc<T>(StoredProcedure, parms, withNoLock);
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
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public GridReader QueryProcMultiple(string StoredProcedure, object parms = null)
        {
            Func<GridReader> acquire = (() =>
            {
                return Connection.QueryMultiple(StoredProcedure, parms, ActiveTransaction, null, CommandType.StoredProcedure);
            });
            return ReadUncommitted(acquire, false);
        }

        #endregion

        #region 辅助方法 参数组成			  

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        private int GetTotalPages(int totalCount, int pageSize)
        {
            return pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        protected string GetSqlParameter(params Parameter[] pars)
        {
            if (pars == null || pars.Length == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Parameter item in pars)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.Like:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.LeftLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.RightLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.NotLike:
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.In:
                        Array arr = item.Value as Array;
                        if (arr != null)
                            sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.Name, BuildSqlParameter.ArrayToString(arr));
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                            sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.Name, BuildSqlParameter.ArrayToString(arr1));
                        break;
                    case OperateType.SqlFormat:
                        object[] arr2 = item.Value as object[];
                        if (arr2 != null)
                            sb.AppendFormat(item.Name, BuildSqlParameter.ArrayToString(arr2));
                        else
                            sb.AppendFormat(item.Name, item.Value);
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr3 = item.Value as object[];
                        if (arr3 != null)
                        {
                            string[] ps = new string[arr3.Length];
                            for (int i = 0; i < arr3.Length; i++)
                            {
                                ps[i] = "@p_" + index.ToString();

                                index++;
                            }
                            sb.AppendFormat(item.Name, ps);
                        }
                        else
                        {
                            sb.AppendFormat(item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@p_" + index.ToString());

                        break;
                    default:
                        break;
                }
                index++;
            }
            return sb.ToString();
        }

        #endregion
    }
}
