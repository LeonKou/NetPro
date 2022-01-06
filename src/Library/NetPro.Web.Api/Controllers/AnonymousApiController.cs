using Microsoft.AspNetCore.Authorization;
using System.NetPro;

namespace NetPro.Web.Api.Controllers
{
    /// <summary>
    /// 允许匿名访问api基类
    /// </summary>
    [AllowAnonymous]
    public class AnonymousApiController : ApiControllerBase
    {
    }
}
