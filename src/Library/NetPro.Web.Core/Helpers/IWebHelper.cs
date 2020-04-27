using Microsoft.AspNetCore.Http;

namespace NetPro.Web.Core.Helpers
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public partial interface IWebHelper
    {
        /// <summary>
        /// 返回http header Referer url 
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// 获取当前访问者ip
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// 获取当前设备信息
        /// </summary>
        string GetClientInfo();

        /// <summary>
        ///当前是否为https访问
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        bool IsCurrentConnectionSecured();

       

        /// <summary>
        /// 当前请求资源是否为静态文件
        /// </summary>
        /// <returns>true 静态资源文件</returns>
        bool IsStaticResource();

        /// <summary>
        /// 修改 query string 字符串
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL</returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// 移除 querystring 参数
        /// </summary>
        /// <param name="url">要修改的url</param>
        /// <param name="queryString">要移除的querystring</param>
        /// <returns>新的url</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// 从Query String中获取指定参数的值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// 从Query String中获取指定参数的值，可指定默认值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        T QueryString<T>(string name, T defaultValue);

        /// <summary>
        /// 从Form中获取指定参数的值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        T QueryFormString<T>(string name);

        /// <summary>
        /// 从Form中获取指定参数的值，可指定默认值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        T QueryFormString<T>(string name, T defaultValue);

        /// <summary>
        ///当前请求协议
        /// </summary>
        string CurrentRequestProtocol { get; }

        /// <summary>
        /// Gets whether the specified HTTP request URI references the local host.
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        bool IsLocalRequest(HttpRequest req);

        /// <summary>
        /// Get the raw path and full query of request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        string GetRawUrl(HttpRequest request);

        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        bool IsPost();

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        bool IsGet();
    }
}
