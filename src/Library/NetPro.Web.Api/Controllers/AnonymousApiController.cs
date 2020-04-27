using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

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
