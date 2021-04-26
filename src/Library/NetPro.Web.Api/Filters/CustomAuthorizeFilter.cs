using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using NetPro.Core.Configuration;
using System.Threading.Tasks;

namespace NetPro.Web.Api.Filters
{
    /// <summary>
    /// 自定义认证过滤器
    /// </summary>
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        private readonly NetProOption _config;

        public CustomAuthorizeFilter(NetProOption config)
        {
            _config = config;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

        }
    }
}
