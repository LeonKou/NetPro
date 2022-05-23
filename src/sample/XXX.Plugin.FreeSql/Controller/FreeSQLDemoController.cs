using FreeSql.Internal.Model;

namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// Freesql数据库增删改查实例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FreeSQLDemoController : ControllerBase
    {
        private readonly ILogger<FreeSQLDemoController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly IFreeSQLDemoByDependency _userService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="webHelper"></param>
        /// <param name="userService"></param>
        public FreeSQLDemoController(ILogger<FreeSQLDemoController> logger,
            IWebHelper webHelper
            , IFreeSQLDemoByDependency userService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _userService = userService;
        }

        /// <summary>
        /// 多库操作示例(切换数据库)
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        /// <returns></returns>
        [HttpPost("MultiFreeSql")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> MultiFreeSqlAsync(string dbKey = "sqlite")
        {
            var constring = await _userService.MultiFreeSqlAsync(dbKey);
            return constring;
        }

        /// <summary>
        /// 获取单行
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        /// <returns></returns>
        [HttpGet("user")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<User> GetList(string dbKey = "sqlite")
        {
            var user = await _userService.GetList(dbKey);
            return user;
        }

        /// <summary>
        /// 新增示例
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPost("insert")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> InsertAsync([FromBody] UserInsertAo userInsertAo, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.InsertAsync(userInsertAo, dbKey);
            _logger.LogInformation($"新增结果是{result}");
            return result;
        }

        /// <summary>
        /// 删除示例
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpDelete("delete")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> DeleteAsync(uint id, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.DeleteAsync(id, dbKey);
            return result;
        }

        /// <summary>
        /// 更新单个值示例
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPatch("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdatePatchAsync(uint id, uint age, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.UpdatePatchAsync(id, age, dbKey);
            return result;
        }

        /// <summary>
        /// 更新整个对象示例
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPost("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdateAsync(UserUpdateAo user, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.UpdateAsync(user, dbKey);
            return result;
        }

        /// <summary>
        /// 关联查询示例
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpGet("searchjoin")]
        [ProducesResponseType(200, Type = typeof(PagedList<User>))]
        public async Task<PagedList<User>> SearchJoinAsync([FromQuery] UserSearchAo search, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.SearchJoinAsync(search, dbKey);
            return result;
        }

        /// <summary>
        /// 类GraphQL查询示例
        /// reference：https://github.com/dotnetcore/FreeSql/wiki/查询
        /// </summary>
        /// <param name="dyfilter"></param>
        /// <param name="searchPageBase"></param>
        /// <param name="dbKey">数据库别名标识</param>
        /// <returns></returns>
        [HttpGet("graphql")]
        [ProducesResponseType(200, Type = typeof(PagedList<User>))]
        public async Task<PagedList<User>> GraphQLAsync([FromQuery] DynamicFilterInfo dyfilter, [FromQuery] SearchPageBase searchPageBase, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.GraphQLAsync(dyfilter, searchPageBase, dbKey);
            return result;
        }

        /// <summary>
        /// 通过linq生成执行的sql字符串示例
        /// </summary>
        /// <param name="dyfilter"></param>
        /// <param name="searchPageBase"></param>
        /// <param name="dbKey">数据库别名标识</param>
        /// <returns></returns>
        [HttpGet("GenerateSqlByLinq")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> GenerateSqlByLinq([FromQuery] DynamicFilterInfo dyfilter, [FromQuery] SearchPageBase searchPageBase, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _userService.GenerateSqlByLinq(dyfilter, searchPageBase, dbKey);
            return result;
        }

        /// <summary>
        /// 事务示例
        /// reference：https://github.com/dotnetcore/FreeSql/wiki/事务
        /// </summary>
        /// <param name="dbKey">数据库别名标识</param>
        /// <returns></returns>
        [HttpGet("Transaction")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public bool Transaction([FromQuery] string dbKey = "sqlite")
        {
            var succeed = _userService.Transaction(dbKey);
            return succeed;
        }
    }
}
