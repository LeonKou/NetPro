using FreeSql.Internal.Model;
using IdGen;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace XXX.Plugin.FreeSql
{
    public interface IFreeSQLDemoByDependency
    {
        Task<int> DeleteAsync(uint id, string dbKey = "sqlite");
        Task<string> GenerateSqlByLinq(DynamicFilterInfo dyfilter, SearchPageBase searchPageBase, string dbKey = "sqlite");
        Task<PagedList<User>> GraphQLAsync(DynamicFilterInfo dyfilter, SearchPageBase searchPageBase, string dbKey = "sqlite");
        Task<int> InsertAsync(UserInsertAo userInsertAo, string dbKey = "sqlite");
        Task<User> GetList(string dbKey = "sqlite");
        Task<string> MultiFreeSqlAsync(string dbKey = "sqlite");
        Task<PagedList<User>> SearchJoinAsync(UserSearchAo search, string dbKey = "sqlite");
        bool Transaction(string dbKey = "sqlite");
        Task<int> UpdateAsync(UserUpdateAo user, string dbKey = "sqlite");
        Task<int> UpdatePatchAsync(uint id, uint age, string dbKey = "sqlite");
    }

    /// <summary>
    /// Freesql示例服务
    /// reference： https://github.com/dotnetcore/FreeSql/wiki/入门
    /// </summary>
    public class FreeSQLDemoByDependency : IFreeSQLDemoByDependency, IScopedDependency//通过继承注入接口实现依赖注入
    {
        private readonly ILogger<FreeSQLDemoByDependency> _logger;
        //private readonly IFreeSql _fsql;
        private readonly IdleBus<IFreeSql> _fsql;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fsql"></param>
        /// <param name="idleFsql"></param>
        /// <param name="mapper"></param>
        public FreeSQLDemoByDependency(ILogger<FreeSQLDemoByDependency> logger
            //, IFreeSql fsql
            , IdleBus<IFreeSql> idleFsql
            , IMapper mapper)
        {
            _logger = logger;
            //_fsql = fsql;
            _fsql = idleFsql;
            _mapper = mapper;
        }

        /// <summary>
        /// 多库操作示例(切换数据库)
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> MultiFreeSqlAsync(string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            var sqliteInstance = await freesql.Select<User>().Limit(2).ToListAsync();
            _logger.LogInformation($"当前默认数据库实例连接字符串 {freesql.Ado.ConnectionString}");
            var mysqlInstance = await freesql.Select<User>().Limit(2).ToListAsync();
            return freesql.Ado.ConnectionString;
        }

        /// <summary>
        /// 获取单行
        /// </summary>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        public async Task<User> GetList(string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            var entityUser = await freesql.Select<User>().Limit(1).FirstAsync();
            return entityUser;
        }

        /// <summary>
        /// 新增示例
        /// <param name="userInsertAo"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public async Task<int> InsertAsync(UserInsertAo userInsertAo, string dbKey = "sqlite")
        {
            //AO实体隐射为数据库DO实体
            //var userEntity = _mapper.Map<UserInsertAo, User>(userInsertAo);
            var userEntity = _mapper.Map<UserInsertAo, UserProfile>(userInsertAo);
            userEntity.Id = Extenisons.GenerateIdByTimestamp();

            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            // Insert方法多个重载，支持单对象、集合对象 
            var affrows = await freesql.Insert(userEntity).ExecuteAffrowsAsync();
            if (affrows == 1)
                _logger.LogInformation("新增成功");
            else
                _logger.LogError("插入失败");
            return affrows;
        }

        /// <summary>
        /// 根据id删除示例
        /// <param name="id"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(uint id, string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            var affrows = await freesql.Delete<User>(id)
                .Where(x => x.Id == id)
                .ExecuteAffrowsAsync();
            _logger.LogError("删除成功 ");
            return affrows;
        }

        /// <summary>
        /// 更新单个值示例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="age"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<int> UpdatePatchAsync(uint id, uint age, string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            //UpdateDiy方法只能通过GetRepository获取到Repository对象才可使用
            var affrows = await freesql.GetRepository<User>().UpdateDiy
                //.Set(s => new User { Age = age }) //将Age值覆盖
                .Set(s => s.Age + age)              //在数据库执行age数据的累加，原子操作
                .Where(x => x.Id == id)
                .ExecuteAffrowsAsync();

            _logger.LogError("更新单个值成功");
            return affrows;
        }

        /// <summary>
        /// 更新整个对象示例
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(UserUpdateAo user, string dbKey = "sqlite")
        {
            var userEntity = _mapper.Map<UserUpdateAo, User>(user);
            int affrows = 0;
            try
            {
                //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
                var freesql = _fsql.Get(dbKey);
                affrows = await freesql.GetRepository<User>()
               .UpdateAsync(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError("更新整个对象失败，不存在");
                return 0;
            }

            _logger.LogInformation("更新整个对象成功");
            return affrows;
        }

        /// <summary>
        ///  多表关联查询示例
        ///  多表关联查询方式提供多种，具体以官方文档
        ///  reference：https://github.com/dotnetcore/FreeSql/wiki/Query-from-Multi-Tables
        ///  reference：https://github.com/dotnetcore/FreeSql/wiki/查询
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<PagedList<User>> SearchJoinAsync(UserSearchAo search, string dbKey = "sqlite")
        {
            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            //方式一：通过Repository间接操作
            var list = await freesql.GetRepository<User>().Select.From<UserRelation, Company>((user, userRe, comp) => user
            //方式二：直接操作
            //var list = await _fsql.Select<User>().From<UserRelation, Company>((user, userRe, comp) => user
            .InnerJoin(u => u.Id == userRe.UserId).InnerJoin(a => userRe.CompanyId == comp.Id))
            .WhereIf(search.Age > 0, (a, s, d) => a.Age == search.Age)//查询条件
            .OrderByDescending((a, s, d) => a.Id)                   //排序：以User.Id 降序排序
            .Page(search.PageIndex, search.PageSize)            //分页
            .Count(out long totalCount)                     //返回结果总数
            .ToListAsync((u, r, c) => new User
            {
                Id = r.UserId,
                Age = u.Age,
                NickName = u.NickName
            });
            _logger.LogError("查询列表成功");
            var result = new PagedList<User>(list, search.PageIndex, search.PageSize, totalCount);
            return result;
        }

        /// <summary>
        ///  动态条件查询示例，类似graphql协议
        ///  reference：https://github.com/dotnetcore/FreeSql/wiki/查询
        /// </summary>
        /// <param name="dyfilter "></param>
        /// <param name="searchPageBase "></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<PagedList<User>> GraphQLAsync(DynamicFilterInfo dyfilter, SearchPageBase searchPageBase, string dbKey = "sqlite")
        {
            //前端构造查询条件的格式如下：
            dyfilter = JsonConvert
                .DeserializeObject<DynamicFilterInfo>(@"
                {
                  ""Logic"" : ""Or"",
                  ""Filters"" :
                  [
                    {
                      ""Field"" : ""Code"", ""Operator"" : ""NotContains"", ""Value"" : ""val1"", 
                      ""Filters"" : [{ ""Field"" : ""Name"", ""Operator"" : ""NotStartsWith"", ""Value"" : ""val2"" }]
                    },
                    {
                      ""Field"" : ""Parent.Code"", ""Operator"" : ""Equals"", ""Value"" : ""val11"",
                      ""Filters"" : [{ ""Field"" : ""Parent.Name"", ""Operator"" : ""Contains"", ""Value"" : ""val22"" }]
                    }
                  ]
                }");

            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            var list = freesql.Select<User>().WhereDynamicFilter(dyfilter)
                 .Page(searchPageBase.PageIndex, searchPageBase.PageSize)//分页
                 .Count(out long totalCount)
                 .ToList();

            var result = new PagedList<User>(list, searchPageBase.PageIndex, searchPageBase.PageSize, totalCount);
            return result;
        }

        /// <summary>
        /// 通过linq生成执行的sql字符串示例
        /// 用途：根据生成sql检查问题优化程序
        /// </summary>
        /// <param name="dyfilter"></param>
        /// <param name="searchPageBase"></param>
        /// <param name="dbKey">数据库实例别名</param>
        /// <returns></returns>
        public async Task<string> GenerateSqlByLinq(DynamicFilterInfo dyfilter, SearchPageBase searchPageBase, string dbKey = "sqlite")
        {
            //前端构造查询条件的格式如下：
            dyfilter = JsonConvert
                .DeserializeObject<DynamicFilterInfo>(@"
                {
                  ""Logic"" : ""Or"",
                  ""Filters"" :
                  [
                    {
                      ""Field"" : ""Code"", ""Operator"" : ""NotContains"", ""Value"" : ""val1"", 
                      ""Filters"" : [{ ""Field"" : ""Name"", ""Operator"" : ""NotStartsWith"", ""Value"" : ""val2"" }]
                    },
                    {
                      ""Field"" : ""Parent.Code"", ""Operator"" : ""Equals"", ""Value"" : ""val11"",
                      ""Filters"" : [{ ""Field"" : ""Parent.Name"", ""Operator"" : ""Contains"", ""Value"" : ""val22"" }]
                    }
                  ]
                }");

            //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
            var freesql = _fsql.Get(dbKey);
            var sqlString = freesql.Select<User>().WhereDynamicFilter(dyfilter)
                  .Page(searchPageBase.PageIndex, searchPageBase.PageSize)//分页
                  .Count(out long totalCount)
                  .ToSql();//生成SQL

            _logger.LogInformation($"生成sql:{sqlString }");
            return sqlString;
        }

        /// <summary>
        /// 事务示例
        /// 另类事务 reference：https://github.com/dotnetcore/FreeSql/issues/322;
        /// 常规事务方式 reference：https://github.com/dotnetcore/FreeSql/wiki/事务;
        /// <param name="dbKey">数据库实例别名</param>
        /// </summary>
        /// <returns></returns>
        public bool Transaction(string dbKey = "sqlite")
        {
            try
            {
                //将当前库sqlite切换到mysql实例上，本方法后续操作都是基于"mysql"实例的操作
                var freesql = _fsql.Get(dbKey);
                //此种事务只能使用同步方法，其他事务用法参考:https://github.com/dotnetcore/FreeSql/wiki/事务
                freesql.Transaction(() =>
                {
                    //_fsql.Ado.TransactionCurrentThread //获得当前事务对象

                    #region 1、插入企业数据生成主键
                    var company = new Company
                    {
                        Name = "公司1",
                        Region = "SZ"
                    };
                    var affrows = freesql.Insert<Company>(company).ExecuteAffrows();

                    if (affrows < 1)
                        throw new Exception("企业数据插入失败，回滚事务，事务退出");
                    #endregion

                    #region  2、插入用户数据生成主键

                    var user = new User
                    {
                        Name = "Leon",
                        Age = 28,
                        CompanyId = company.Id,
                        NickName = "xiaoliangge",
                        CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        Pwd = "密码就是不断提高"
                    };
                    affrows = freesql.Insert<User>(user).ExecuteAffrows();

                    if (affrows < 1)
                        throw new Exception("用户数据插入失败，回滚事务，事务退出");
                    #endregion

                    #region 3、插入用户企业关系数据生成主键
                    var userRelation = new UserRelation
                    {
                        CompanyId = company.Id,
                        UserId = company.Id,
                    };
                    affrows = freesql.Insert<UserRelation>(userRelation).ExecuteAffrows();

                    if (affrows < 1)
                        throw new Exception("用户企业关系表插入失败，回滚事务，事务退出");
                    #endregion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"事务执行失败");
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 公工服务
    /// </summary>
    internal static partial class Extenisons
    {
        /// <summary>
        /// 生成唯一Id
        /// 例如 991576137677144064
        /// </summary>
        /// <returns>雪花Id</returns>
        public static string GenerateIdByTimestamp()
        {
            var uniqueId = new IdGenerator(0).CreateId().ToString();
            return uniqueId;
        }

        /// <summary>
        /// 生成基于时间戳的Id
        /// 例如 164570046556820221029
        /// </summary>
        /// <returns>毫秒时间戳+2位随机数+3位随机数</returns>
        public static string Test()
        {
            var huge = BigInteger.Parse(Guid.NewGuid().ToString("N"), NumberStyles.AllowHexSpecifier).ToString();
            var uniqueId = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}{huge.Substring(2, 4)}{Environment.CurrentManagedThreadId.ToString().PadLeft(2, '0')[..2]}{RandomNumberGenerator.GetInt32(10, 99)}";
            return uniqueId;
        }

        /// <summary>
        /// 生成基于UTC时间的Id
        /// 例如 22022410470006830011910
        /// </summary>
        /// <returns>返回基于长度为23；格式：yyyyMMddHH+11位随机数</returns>
        public static string GenerateIdByTime()
        {
            var huge = BigInteger.Parse(Guid.NewGuid().ToString("N"), NumberStyles.AllowHexSpecifier).ToString();
            var timeString = DateTime.UtcNow.ToString("yyMMddHHmm");
            var uniqueId = $"{timeString}{huge.Substring(2, 11)}{Environment.CurrentManagedThreadId.ToString().PadLeft(2, '0')[..2]}";
            return uniqueId;
        }

        /// <summary>
        /// 生成指定长度随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string GenerateRandomNumber(int length)
        {
            StringBuilder startsb = new StringBuilder("1");
            StringBuilder endsb = new StringBuilder("9");
            for (int i = 0; i < length - 1; i++)
            {
                startsb.Append('0');
                endsb.Append('9');
            }
            int startNum = int.Parse(startsb.ToString());
            int endNum = int.Parse(endsb.ToString());
            var result = RandomNumberGenerator.GetInt32(startNum, endNum);

            return result.ToString();
        }

        /// <summary>
        ///  生成左补齐的随机数字
        /// </summary>
        /// <param name="fromInclusive">起始值</param>
        /// <param name="toExclusive">最大值</param>
        /// <param name="totalWidth">结果长度</param>
        /// <param name="paddingChar">补齐值，默认用0左补齐</param>
        /// <returns></returns>
        public static string GenerateRandomNumberPadLeft(int fromInclusive, int toExclusive, int totalWidth = 4, char paddingChar = '0')
        {
            var result = RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
            return result.ToString().PadLeft(totalWidth, paddingChar);
        }
    }
}
