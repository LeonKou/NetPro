using NetPro.EasyNetQ;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 全局配置的Builder接口
    /// </summary>
    public interface IEasyNetQBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// 全局配置的Builder
    /// </summary>
    internal class EasyNetQBuilder : IEasyNetQBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        public IServiceCollection Services { get; }

        public EasyNetQBuilder(IServiceCollection services)
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
        private readonly EasyNetQOption _option;

        public DefaultConnections(EasyNetQOption option)
        {
            _option = option;
        }
        public IList<ConnectionString> GetConnectionStrings()
        {
            return _option.ConnectionString;
        }
    }
}