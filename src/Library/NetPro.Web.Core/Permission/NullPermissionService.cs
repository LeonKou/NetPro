namespace NetPro.Web.Core.Permission
{
    /// <summary>
    /// 默认权限接口实现
    /// </summary>
    public class NullPermissionService : INetProPermissionService
    {
        /// <summary>
        /// 根据权限code判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="code">权限code</param>
        /// <returns></returns>
        public bool HasPermission(string code)
        {
            return false;
        }

        /// <summary>
        /// 根据权限code判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="controllerName">controller名称</param>
        /// <param name="actionName">action名称</param>
        /// <returns></returns>
        public bool HasPermission(string controllerName, string actionName)
        {
            return false;
        }

        /// <summary>
        /// 根据权限id判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pid">权限id</param>
        /// <returns></returns>
        public bool HasPermission(int pid)
        {
            return false;
        }

    }
}
