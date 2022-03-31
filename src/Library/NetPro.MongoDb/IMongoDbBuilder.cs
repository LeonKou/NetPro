using NetPro.MongoDb;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 全局配置的Builder接口
    /// </summary>
    public interface IMongoDbBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// 全局配置的Builder
    /// </summary>
    internal class MongoDbBuilder : IMongoDbBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        public IServiceCollection Services { get; }

        public MongoDbBuilder(IServiceCollection services)
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
        private readonly MongoDbOption _option;

        public DefaultConnections(MongoDbOption option)
        {
            _option = option;
        }
        public IList<ConnectionString> GetConnectionStrings()
        {
            return _option.ConnectionString;
        }
    }
}