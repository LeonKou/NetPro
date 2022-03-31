using NetPro.CsRedis;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Redis全局配置的Builder接口
    /// </summary>
    public interface ICSRedisBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }

    }

    /// <summary>
    /// CSRedisBuilder全局配置的Builder
    /// </summary>
    internal class CSRedisBuilder : ICSRedisBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        public IServiceCollection Services { get; }

        public CSRedisBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }

    /// <summary>
    /// 获取连接串定义
    /// </summary>
    public interface IConnectionsFactory
    {
        public IList<ConnectionString> GetConnectionStrings();
    }

    internal class DefaultConnections : IConnectionsFactory
    {
        private readonly RedisCacheOption _option;

        public DefaultConnections(RedisCacheOption option)
        {
            _option = option;
        }
        public IList<ConnectionString> GetConnectionStrings()
        {
            return _option.ConnectionString;
        }
    }
}