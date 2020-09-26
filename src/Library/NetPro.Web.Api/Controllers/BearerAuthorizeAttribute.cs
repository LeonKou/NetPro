using Microsoft.AspNetCore.Authorization;

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
