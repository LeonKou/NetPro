using FreeSql.Internal.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;
using System.ComponentModel.DataAnnotations;

namespace XXX.Plugin.Web.Manager
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FreeSQLDemoController : ControllerBase
    {
        private readonly ILogger<FreeSQLDemoController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly Demo.IDemoService _managerdemoService;
        private readonly IFreeSQLDemoService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="webHelper"></param>
        /// <param name="managerdemoService"></param>
        /// <param name="userService"></param>
        public FreeSQLDemoController(ILogger<FreeSQLDemoController> logger,
            IWebHelper webHelper
            , Demo.IDemoService managerdemoService
            , IFreeSQLDemoService userService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _managerdemoService = managerdemoService;
            _userService = userService;
        }

        /// <summary>
        /// 多库操作示例(切换数据库)
        /// </summary>
        /// <returns></returns>
        [HttpPost("MultiFreeSql")]
        public async Task<string> MultiFreeSqlAsync(string dbKey)
        {
            var constring = await _userService.MultiFreeSqlAsync(dbKey);
            return constring;
        }

        /// <summary>
        /// 新增示例
        /// </summary>
        [HttpPost("insert")]
        public async Task<string> InsertAsync(UserInsertAo userInsertAo)
        {
            await _userService.InsertAsync(userInsertAo);
            return "";
        }

        /// <summary>
        /// 删除示例
        /// </summary>
        [HttpDelete("delete")]
        public async Task<int> DeleteAsync(uint id)
        {
            var result = await _userService.DeleteAsync(id);
            return result;
        }

        /// <summary>
        /// 更新单个值示例
        /// </summary>
        [HttpPatch("update")]
        public async Task<int> UpdatePatchAsync(uint id, uint age)
        {
            var result = await _userService.UpdatePatchAsync(id, age);
            return result;
        }

        /// <summary>
        /// 更新整个对象示例
        /// </summary>
        [HttpPost("update")]
        public async Task<int> UpdateAsync(UserUpdateAo user)
        {
            var result = await _userService.UpdateAsync(user);
            return result;
        }

        /// <summary>
        /// 关联查询示例
        /// </summary>
        [HttpGet("searchjoin")]
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
        public bool Transaction()
        {
            var succeed = _userService.Transaction();
            return succeed;
        }
    }
}
