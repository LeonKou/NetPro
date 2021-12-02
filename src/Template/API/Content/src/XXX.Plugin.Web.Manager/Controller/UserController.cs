using FreeSql.Internal.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;
using System.ComponentModel.DataAnnotations;
using XXX.Plugin.Web.Demo;

namespace XXX.Plugin.Web.Manager
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
        private readonly IRedisService _redisService;
        private readonly IFreeSQLDemoService _userService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="webHelper"></param>
        /// <param name="redisService"></param>
        /// <param name="userService"></param>
        public FreeSQLDemoController(ILogger<FreeSQLDemoController> logger,
            IWebHelper webHelper
            , IRedisService redisService
            , IFreeSQLDemoService userService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _redisService = redisService;
            _userService = userService;
        }

        /// <summary>
        /// 多库操作示例(切换数据库)
        /// </summary>
        /// <returns></returns>
        [HttpPost("MultiFreeSql")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> MultiFreeSqlAsync(string dbKey)
        {
            var constring = await _userService.MultiFreeSqlAsync(dbKey);
            return constring;
        }

        /// <summary>
        /// 新增示例
        /// </summary>
        [HttpPost("insert")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> InsertAsync(UserInsertAo userInsertAo)
        {
            var result = await _userService.InsertAsync(userInsertAo);
            return result;
        }

        /// <summary>
        /// 删除示例
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> DeleteAsync(uint id)
        {
            var result = await _userService.DeleteAsync(id);
            return result;
        }

        /// <summary>
        /// 更新单个值示例
        /// </summary>
        [HttpPatch("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdatePatchAsync(uint id, uint age)
        {
            var result = await _userService.UpdatePatchAsync(id, age);
            return result;
        }

        /// <summary>
        /// 更新整个对象示例
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdateAsync(UserUpdateAo user)
        {
            var result = await _userService.UpdateAsync(user);
            return result;
        }

        /// <summary>
        /// 关联查询示例
        /// </summary>
        [HttpGet("searchjoin")]
        [ProducesResponseType(200, Type = typeof(PagedList<User>))]
        public async Task<PagedList<User>> SearchJoinAsync([FromQuery] UserSearchAo search)
        {
            var result = await _userService.SearchJoinAsync(search);
            return result;
        }

        /// <summary>
        /// 类GraphQL查询示例
        /// reference：https://github.com/dotnetcore/FreeSql/wiki/查询
        /// </summary>
        /// <param name="dyfilter"></param>
        /// <param name="searchPageBase"></param>
        /// <returns></returns>
        [HttpGet("graphql")]
        [ProducesResponseType(200, Type = typeof(PagedList<User>))]
        public async Task<PagedList<User>> GraphQLAsync([FromQuery] DynamicFilterInfo dyfilter, [FromQuery] SearchPageBase searchPageBase)
        {
            var result = await _userService.GraphQLAsync(dyfilter, searchPageBase);
            return result;
        }

        /// <summary>
        /// 通过linq生成执行的sql字符串示例
        /// </summary>
        /// <param name="dyfilter"></param>
        /// <param name="searchPageBase"></param>
        /// <returns></returns>
        [HttpGet("GenerateSqlByLinq")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<string> GenerateSqlByLinq([FromQuery] DynamicFilterInfo dyfilter, [FromQuery] SearchPageBase searchPageBase)
        {
            var result = await _userService.GenerateSqlByLinq(dyfilter, searchPageBase);
            return result;
        }

        /// <summary>
        /// 事务示例
        /// reference：https://github.com/dotnetcore/FreeSql/wiki/事务
        /// </summary>
        /// <returns></returns>
        [HttpGet("Transaction")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public bool Transaction()
        {
            var succeed = _userService.Transaction();
            return succeed;
        }
    }
}
