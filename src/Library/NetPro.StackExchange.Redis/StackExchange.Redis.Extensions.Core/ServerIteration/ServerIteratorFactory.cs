using StackExchange.Redis.Extensions.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackExchange.Redis.Extensions.Core.ServerIteration
{
    /// <summary>
    /// redis服务工厂
    /// </summary>
    public class ServerIteratorFactory
    {
        /// <summary>
        /// 返回redis服务,返回所有或者一个
        /// </summary>
        /// <param name="multiplexer"></param>
        /// <param name="serverEnumerationStrategy">redis迭代器</param>
        public static IEnumerable<IServer> GetServers(
            IConnectionMultiplexer multiplexer,
            ServerEnumerationStrategy serverEnumerationStrategy)
        {
            switch (serverEnumerationStrategy.Mode)
            {
                case ServerEnumerationStrategy.ModeOptions.All:
                    var serversAll = new ServerEnumerable(
                                            multiplexer,
                                            serverEnumerationStrategy.TargetRole,
                                            serverEnumerationStrategy.UnreachableServerAction);
                    return serversAll;

                case ServerEnumerationStrategy.ModeOptions.Single:
                    var serversSingle = new ServerEnumerable(
                                                multiplexer,
                                                serverEnumerationStrategy.TargetRole,
                                                serverEnumerationStrategy.UnreachableServerAction);
                    return serversSingle.Take(1);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
