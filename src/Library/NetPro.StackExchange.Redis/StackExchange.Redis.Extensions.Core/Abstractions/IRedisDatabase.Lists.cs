using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <summary>
    /// The Redis Database
    /// </summary>
    public partial interface IRedisDatabase
    {
        /// <summary>
        /// 添加item单个对象到列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="item">要添加的项</param>
        /// <param name="when">添加项的条件,默认不检查是否存在始终添加</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 添加item单个对象到列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="item">要添加的项</param>
        /// <param name="when">添加项的条件,默认不检查是否存在始终添加</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 添加items 数组集合到列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="items">items 数组集合</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 添加items 数组集合到列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="items">items 数组集合</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        long ListAddToLeft<T>(string key, T[] items, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 删除并返回存储在键处的列表的最后一个元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 删除并返回存储在键处的列表的最后一个元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">redsi key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        T ListGetFromRight<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;
    }
}
