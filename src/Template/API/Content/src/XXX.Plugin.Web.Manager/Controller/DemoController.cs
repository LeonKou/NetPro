using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro;

namespace XXX.Plugin.Web.Manager
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<UserController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly XXX.Plugin.Web.Demo.IDemoService _managerdemoService;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger,
            IWebHelper webHelper
            , XXX.Plugin.Web.Demo.IDemoService managerdemoService
            , IUserService userService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _managerdemoService = managerdemoService;
            _userService = userService;
        }

        /// <summary>
        /// 新增
        /// </summary>
        [HttpPost("insert")]
        public async Task<string> InsertAsync()
        {
            await _userService.InsertAsync();
            var enWeb = EngineContext.Current.Resolve<IWebHelper>();
            var ip = enWeb.GetCurrentIpAddress();
            var diweb = _webHelper.GetCurrentIpAddress();
            _managerdemoService.Test();
            var file = EngineContext.Current.Resolve<INetProFileProvider>();
            return "";
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpDelete("delete")]
        public async Task<int> DeleteAsync(uint id)
        {
            var result = await _userService.DeleteAsync(id);
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        [HttpPatch("update")]
        public async Task<int> UpdatePatchAsync(uint id, uint age)
        {
            var result = await _userService.UpdatePatchAsync(id, age);
            return result;
        }

        /// <summary>
        /// 查询
        /// </summary>
        [HttpGet("search")]
        public async Task<PagedList<User>> SearchAsync([FromQuery]UserSearchAo search)
        {
            var result = await _userService.SearchAsync(search);
            return result;
        }
    }

}
