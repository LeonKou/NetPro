using System;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <summary>
    /// The Redis Database
    /// </summary>
    public partial interface IRedisDatabase
    {
        /// <summary>
        /// 将消息发布到管道
        /// </summary>
        /// <typeparam name="T">期望的值</typeparam>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="message">要发布的消息</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 将消息发布到管道
        /// </summary>
        /// <typeparam name="T">期望的值</typeparam>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="message">要发布的消息</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        long Publish<T>(RedisChannel channel, T message, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  注册一个回调处理程序来处理发布到通道的消息。
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="handler">当收到消息时要运行的函数</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task SubscribeAsync<T>(string channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  注册一个回调处理程序来处理发布到通道的消息。
        /// </summary>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="handler">当收到消息时要运行的函数</param>
        /// <param name="flags">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        void Subscribe(string channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 退订回调处理程序来处理发布到通道的消息。
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="handler">当收到消息时要运行的函数.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 退订回调处理程序来处理发布到通道的消息。
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="channel">发布订阅的管道名称</param>
        /// <param name="handler">当收到消息时要运行的函数.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        void Unsubscribe<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  注销管道上的所有回调处理程序。
        /// </summary>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task UnsubscribeAllAsync(CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  注销管道上的所有回调处理程序。
        /// </summary>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        void UnsubscribeAll(CommandFlags flag = CommandFlags.None);
    }
}
