using Microsoft.Extensions.Configuration;
using SkyApm.Config;
using SkyApm.Tracing;
using SkyApm.Utilities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using System.Linq;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 支持Skywalking 默认配置通过IConfiguration对象读取，以支持Apollo
    /// </summary>
    public class SkyAPMConfigAccessor : IConfigAccessor
    {
        private readonly IConfiguration _configuration;

        public SkyAPMConfigAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T Get<T>() where T : class, new()
        {
            var config = typeof(T).GetCustomAttribute<ConfigAttribute>();
            var instance = New<T>.Instance();
            _configuration.GetSection(config.GetSections()).Bind(instance);
            return instance;
        }

        public T Value<T>(string key, params string[] sections)
        {
            var config = new ConfigAttribute(sections);
            return _configuration.GetSection(config.GetSections()).GetValue<T>(key);
        }

        /// <summary>
        /// high performance
        /// </summary>
        private static class New<T> where T : new()
        {
            public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
            (
                Expression.New(typeof(T))
            ).Compile();
        }
    }

    /// <summary>
    /// 自定义监听规则
    /// </summary>
    public class CustomSamplingInterceptor : ISamplingInterceptor
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public CustomSamplingInterceptor(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public int Priority { get; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samplingContext"></param>
        /// <param name="next"></param>
        /// <returns>true:监听；false:忽略监听</returns>
        public bool Invoke(SamplingContext samplingContext, Sampler next)
        {
            try
            {
                if (string.IsNullOrEmpty(samplingContext.OperationName))
                {
                    return false;
                }

                var urls = new List<IgnoresUrl>();
                _configuration.GetSection("SkyWalking:Ignores").Bind(urls);

                if (urls != null && urls.Count > 0)
                {
                    foreach (var item in urls)
                    {
                        if (string.IsNullOrEmpty(item.Urls))
                        {
                            continue;
                        }

                        var isContains = item.Urls.ToLower().Split(',').ToList().Exists(a => samplingContext.OperationName.ToLower().Contains(a));
                        if (isContains)
                        {
                            return false;
                        }
                    }
                }

                var result = next(samplingContext);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"SkyWalking链路追踪异常 OperationName:{samplingContext?.OperationName}");
                return false;
            }

        }
    }
    public class IgnoresUrl
    {
        public string Urls { get; set; }
    }
}
