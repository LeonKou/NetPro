namespace NetPro.Web.Core.Permission
{
    /// <summary>
    /// 权限接口
    /// </summary>
    public interface INetProPermissionService
    {
        /// <summary>
        /// 根据权限code判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="code">权限code</param>
        /// <returns></returns>
        bool HasPermission(string code);

        /// <summary>
        /// 根据权限code判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="controllerName">controller名称</param>
        /// <param name="actionName">action名称</param>
        /// <returns></returns>
        bool HasPermission(string controllerName,string actionName);

        /// <summary>
        /// 根据权限id判断用户是否有权限
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="pid">权限id</param>
        /// <returns></returns>
        bool HasPermission(int pid);

    }
}
