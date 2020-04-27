using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetPro.Web.Core.Helpers
{
    /// <summary>
    /// 权限辅助类
    /// </summary>
   public class PermissionHelper
    {
        /// <summary>
        /// 用户权限判断.判断action权限
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="controllerName">controllerName</param>
        /// <param name="actionName"></param>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public static bool HasPermission(long userId, string controllerName, string actionName, string roleName = "")
        {
            return true;
            if (userId <= 0 || string.IsNullOrWhiteSpace(controllerName) || string.IsNullOrWhiteSpace(actionName))
            {
                return false;
            }
            if (IsSuperRole(roleName))
            {
                return true;
            }
            //TODO 根据controllerName,actionName判断该用户是否有权限
            return true;
        }

        /// <summary>
        /// 判断角色是否为超级角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        private static bool IsSuperRole(string roleName)
        {
            var config = EngineContext.Current.Resolve<NetProOption>();
            var superRole = config.SuperRole.ToLower();
            //超级用户跳过功能权限验证
            if (!string.IsNullOrWhiteSpace(superRole) && !string.IsNullOrWhiteSpace(roleName))
            {
                if (superRole.Split(',').Contains(roleName.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 验证用户是否拥有功能权限
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="accessCode">权限编号(功能编号)</param>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public static bool HasPermission(long userId,string accessCode,string roleName = "")
        {
            if (userId <= 0||string.IsNullOrWhiteSpace(accessCode))
            {
                return false;
            }
            if (IsSuperRole(roleName))
            {
                return true;
            }

            //TODO 根据权限编号
            return true;
        }
    }
}
