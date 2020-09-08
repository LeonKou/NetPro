using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using NetPro.Utility;
using NetPro.Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NetPro.TypeFinder;

namespace NetPro.Web.Core.Helpers
{
    /// <summary>
    /// Represents a web helper
    /// </summary>
    public partial class WebHelper : IWebHelper
    {
        #region Const

        private const string NullIpAddress = "::1";

        #endregion

        #region Fields 

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HostingConfig _hostingConfig;
        private readonly INetProFileProvider _fileProvider;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="hostingConfig">Hosting config</param>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        /// <param name="fileProvider">File provider</param>
        public WebHelper(HostingConfig hostingConfig, IHttpContextAccessor httpContextAccessor,
            INetProFileProvider fileProvider)
        {
            this._hostingConfig = hostingConfig;
            this._httpContextAccessor = httpContextAccessor;
            this._fileProvider = fileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns>True if available; otherwise false</returns>
        protected virtual bool IsRequestAvailable()
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                return false;

            try
            {
                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Is IP address specified
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>Result</returns>
        protected virtual bool IsIpAddressSet(IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }

        /// <summary>
        /// Try to write web.config file
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryWriteWebConfig()
        {
            try
            {
                _fileProvider.SetLastWriteTimeUtc(_fileProvider.MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            //URL referrer is null in some case (for example, in IE 8)
            return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var stringIp = string.Empty;
            try
            {
                //first try to get IP address from the forwarded header
                if (_httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
                    {
                        stringIp = _httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];
                        return stringIp;
                    }
                    else if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        stringIp = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
                        return stringIp;
                    }
                    else
                    {
                        stringIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                        return stringIp;
                    }
                }

                //if this header not exists try get connection remote IP address
                if (string.IsNullOrEmpty(stringIp) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                    stringIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            //some of the validation
            if (stringIp != null && stringIp.Equals("::1", StringComparison.InvariantCultureIgnoreCase))
                stringIp = "127.0.0.1";

            //"TryParse" doesn't support IPv4 with port number
            if (IPAddress.TryParse(stringIp ?? string.Empty, out IPAddress ip))
                //IP address is valid 
                stringIp = ip.ToString();
            else if (!string.IsNullOrEmpty(stringIp))
                //remove port
                stringIp = stringIp.Split(':').FirstOrDefault();

            return stringIp;
        }

        /// <summary>
        /// 获取当前设备信息
        /// </summary>
        public virtual string GetClientInfo()
        {
            if (!IsRequestAvailable())
                return string.Empty;

            var result = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Headers != null)
            {
                HttpContext context = _httpContextAccessor.HttpContext;
                // "客户端的操作系统：" + context.Request.UserAgent;//获取操作系统
                string HostName = Dns.GetHostName();
                Regex reg = new Regex("mozilla|m3gate|winwap|openwave|Windows NT|Windows 3.1|95|Blackcomb|98|ME|X Window|Longhorn|ubuntu|AIX|Linux|AmigaOS|BEOS|HP-UX|OpenBSD|FreeBSD|NetBSD|OS/2|OSF1|SUN");
                result += "\r\n硬件：" + (reg.IsMatch(context.Request.Headers["User-Agent"]) ? "电脑" : "手机");//获取计算机还是手机
                                                                                                         //info += "\r\nwin32系统：" + (context.Request.Browser.Win32 ? "是" : "否");
                result += "\r\n计算机名称：" + HostName;
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            if (!IsRequestAvailable())
                return false;

            //check whether hosting uses a load balancer
            //use HTTP_CLUSTER_HTTPS?
            if (_hostingConfig.UseHttpClusterHttps)
                return _httpContextAccessor.HttpContext.Request.Headers["HTTP_CLUSTER_HTTPS"].ToString().Equals("on", StringComparison.OrdinalIgnoreCase);

            //use HTTP_X_FORWARDED_PROTO?
            if (_hostingConfig.UseHttpXForwardedProto)
                return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-Proto"].ToString().Equals("https", StringComparison.OrdinalIgnoreCase);

            return _httpContextAccessor.HttpContext.Request.IsHttps;
        }

        /// <summary>
        /// Gets store host location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>Store host location</returns>
        public virtual string GetStoreHost(bool useSsl)
        {
            if (!IsRequestAvailable())
                return string.Empty;

            //try to get host from the request HOST header
            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (StringValues.IsNullOrEmpty(hostHeader))
                return string.Empty;

            //add scheme to the URL
            var storeHost = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}://{hostHeader.FirstOrDefault()}";

            //ensure that host is ended with slash
            storeHost = $"{storeHost.TrimEnd('/')}/";

            return storeHost;
        }



        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <returns>True if the request targets a static resource file.</returns>
        public virtual bool IsStaticResource()
        {
            if (!IsRequestAvailable())
                return false;

            string path = _httpContextAccessor.HttpContext.Request.Path;

            //a little workaround. FileExtensionContentTypeProvider contains most of static file extensions. So we can use it
            //source: https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs
            //if it can return content type, then it's a static file
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            return contentTypeProvider.TryGetContentType(path, out string _);
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="anchor">Anchor</param>
        /// <returns>New URL</returns>
        public virtual string ModifyQueryString(string url, string queryStringModification, string anchor)
        {
            if (url == null)
                url = string.Empty;

            if (queryStringModification == null)
                queryStringModification = string.Empty;

            if (anchor == null)
                anchor = string.Empty;

            var str = string.Empty;
            var str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                if (!dictionary.ContainsKey(strArray[0]))
                                {
                                    //do not add value if it already exists
                                    //two the same query parameters? theoretically it's not possible.
                                    //but MVC has some ugly implementation for checkboxes and we can have two values
                                    //find more info here: http://www.mindstorminteractive.com/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                                    //we do this validation just to ensure that the first one is not overridden
                                    dictionary[strArray[0]] = strArray[1];
                                }
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (var str4 in queryStringModification.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            var strArray2 = str4.Split(new[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(anchor))
            {
                str2 = anchor;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2)));
        }

        /// <summary>
        /// Remove query string from the URL
        /// </summary>
        /// <param name="url">URL to modify</param>
        /// <param name="queryString">Query string to remove</param>
        /// <returns>New URL without passed query string</returns>
        public virtual string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;

            if (queryString == null)
                queryString = string.Empty;

            var str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var str3 in str.Split(new[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            var strArray = str3.Split(new[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (var str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        /// <summary>
        /// 从Query String中获取指定参数的值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        public virtual T QueryString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
                return default(T);

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Query[name].ToString());
        }

        /// <summary>
        /// 从Query String中获取querystring指定参数的值，可指定默认值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        public virtual T QueryString<T>(string name, T defaultValue)
        {
            if (!IsRequestAvailable())
                return defaultValue;

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Query[name]))
                return defaultValue;

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Query[name].ToString());
        }

        /// <summary>
        /// 从Form中获取指定参数的值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        public virtual T QueryFormString<T>(string name)
        {
            if (!IsRequestAvailable())
                return default(T);

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Form[name]))
                return default(T);

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Form[name].ToString());
        }

        /// <summary>
        /// 从Form中获取指定参数的值，可指定默认值
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="name">参数名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        public virtual T QueryFormString<T>(string name, T defaultValue)
        {
            if (!IsRequestAvailable())
                return defaultValue;

            if (StringValues.IsNullOrEmpty(_httpContextAccessor.HttpContext.Request.Form[name]))
                return defaultValue;

            return CommonHelper.To<T>(_httpContextAccessor.HttpContext.Request.Form[name].ToString());
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="makeRedirect">A value indicating whether we should made redirection after restart</param>
        public virtual void RestartAppDomain(bool makeRedirect = false)
        {
            //the site will be restarted during the next request automatically
            //_applicationLifetime.StopApplication();

            //"touch" web.config to force restart
            var success = TryWriteWebConfig();
            if (!success)
            {
                throw new NetProException("修改web.config文件失败,请检查权限");
            }
        }

        /// <summary>
        /// Gets current HTTP request protocol
        /// </summary>
        public virtual string CurrentRequestProtocol => IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        /// <summary>
        /// Gets whether the specified HTTP request URI references the local host.
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        public virtual bool IsLocalRequest(HttpRequest req)
        {
            //source: https://stackoverflow.com/a/41242493/7860424
            var connection = req.HttpContext.Connection;
            if (IsIpAddressSet(connection.RemoteIpAddress))
            {
                //We have a remote address set up
                return IsIpAddressSet(connection.LocalIpAddress)
                    //Is local is same as remote, then we are local
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    //else we are remote if the remote IP address is not a loopback address
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }

        /// <summary>
        /// Get the raw path and full query of request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        public virtual string GetRawUrl(HttpRequest request)
        {
            //first try to get the raw target from request feature
            //note: value has not been UrlDecoded
            var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            //or compose raw URL manually
            if (string.IsNullOrEmpty(rawUrl))
                rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            return rawUrl;
        }

        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public virtual bool IsPost()
        {
            return _httpContextAccessor.HttpContext.Request.Method.Equals("POST");
        }

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public virtual bool IsGet()
        {
            return _httpContextAccessor.HttpContext.Request.Method.Equals("GET");
        }

        #endregion
    }
}