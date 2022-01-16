using Microsoft.AspNetCore.Authorization;

namespace System.NetPro
{
    /// <summary>
    /// 需要认证api基类
    /// </summary>
    [Authorize]
    public abstract class AuthorizeApiController : ApiControllerBase
    {
        /// <summary>
        /// 登陆用户名
        /// </summary>
        protected string UserName
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return User.Identity.Name;
                }
                return null;
            }
        }

        /// <summary>
        /// 用户id
        /// </summary>
        protected long UserId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return User.Identity.GetUserId<long>();
                }
                return 0;
            }
        }
    }
}
