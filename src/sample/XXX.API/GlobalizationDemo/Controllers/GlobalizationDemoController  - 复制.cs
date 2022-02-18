using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using XXX.API.Service;

namespace XXX.API.Controllers
{
    /// <summary>
    /// 外放接口:用户服务相关
    /// </summary>
    [ApiController]
    [Route("api/user/openapi")]
    public partial class OpenAPIController : ControllerBase
    {
        /// <summary>
        /// 需外放的用户服务的姓名查询接口
        /// </summary>
        /// <returns></returns>
        /// <remarks>外放接口迭代需考虑上一版本实现</remarks>
        [HttpGet("search/name")]
        public IActionResult SearchName()
        {
            return Ok("查询User用户服务下的用户姓名");
        }
    }

    /// <summary>
    /// 外放接口:用户服务相关
    /// </summary>
    public partial class OpenAPIController : ControllerBase
    {
        /// <summary>
        /// 需外放的用户服务的姓名更新接口
        /// </summary>
        /// <returns></returns>
        /// <remarks>外放接口迭代需考虑上一版本实现</remarks>
        [HttpPatch("name")]
        public IActionResult Name()
        {
            return Ok("更新User用户服务下的用户姓名");
        }
    }
}
