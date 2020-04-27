using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Web.Api.Controllers
{
    /// <summary>
    ///bearer 认证
    /// </summary>
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute() : base("Bearer")
        {

        }
    }
}
