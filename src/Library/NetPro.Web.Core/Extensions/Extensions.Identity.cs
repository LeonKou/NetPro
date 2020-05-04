using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace NetPro.Web.Core
{
    /// <summary>
    /// IdentityExtensions
    /// </summary>
    public static partial class Extensions
    {
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            if (identity is ClaimsIdentity claimsIdentity)// 等价于 ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            {
                Claim claim = claimsIdentity.FindFirst(r=>r.Type== ClaimTypes.NameIdentifier);
                string claimValue;
                if (claim == null)
                {
                    claimValue= identity.GetClaimValue(JwtClaimTypes.Subject);
                }
                else
                {
                    claimValue = claim.Value;
                }
                if (string.IsNullOrWhiteSpace(claimValue))
                {
                    claimValue = "0";
                }
                return (T)Convert.ChangeType(claimValue, typeof(T), CultureInfo.InvariantCulture);
            }
            return default(T);
        }

        /// <summary>
        /// 获取用户角色名称
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetRoleName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            if (identity is ClaimsIdentity claimsIdentity)// 等价于 ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            {
                return claimsIdentity.GetClaimValue(JwtClaimTypes.Role);
            }
            return string.Empty;
        }

        /// <summary>
        /// 从cookie取出对应值
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueFromCookie(this IIdentity identity, HttpContext context, string key)
        {
            string value = context.Request.Cookies[key];

            return Convert.ToString(value ?? string.Empty);
        }

        /// <summary>
        /// 批量设定cookie
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="context"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public static void SetValueToCookie(this IIdentity identity, HttpContext context, Dictionary<string, string> keyValue)
        {
            foreach (var item in keyValue)
            {
                context.Response.Cookies.Append(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 取回当前使用者的指定Claim类型的值
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string GetClaimValue(this IIdentity identity, string claimType)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst(claimType);
            if (claim == null)
            {
                return string.Empty;
            }
            return claim.Value;
        }
    }
}
