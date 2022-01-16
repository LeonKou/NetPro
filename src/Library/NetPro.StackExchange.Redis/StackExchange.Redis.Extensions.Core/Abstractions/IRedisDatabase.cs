using StackExchange.Redis.Extensions.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <summary>
    /// The Redis Database
    /// </summary>
    public partial interface IRedisDatabase
    {
        /// <summary>
        ///     Gets the instance of <see cref="IDatabase" /> used be ICacheClient implementation
        /// </summary>
        IDatabase Database { get; }

        /// <summary>
        ///     Gets the instance of <see cref="ISerializer" />
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// 检查key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回具有超时的键的剩余生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 返回具有超时的键的剩余生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 设置一个超时键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <returns>true:设置成功，false：设置失败</returns>
        Task<bool> KeyExpireAsync(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 设置一个超时键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry">expiry为空，设置为永不失效</param>
        /// <param name="flags"></param>
        /// <returns>true:设置成功，false：设置失败</returns>
        bool KeyExpire(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 检查key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        bool Exists(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 移除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 移除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        bool Remove(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  移除指定的key集合
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  移除指定的key集合
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        long RemoveAll(IEnumerable<string> keys, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///获取或者创建缓存 
        /// localExpiredTime参数大于0并且小于expiredTime数据将缓存到本地内存
        /// 设置了本地缓存过期时间大于0并委托也为空，本地将保持5秒空值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="localExpiredTime">本地过期时间</param>
        /// <returns></returns>
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0);

        /// <summary>
        ///获取或者创建缓存 
        /// localExpiredTime参数大于0并且小于expiredTime数据将缓存到本地内存
        /// 设置了本地缓存过期时间大于0并委托也为空，本地将保持5秒空值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="localExpiredTime">本地过期时间</param>
        /// <returns></returns>
        T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0);

        /// <summary>
        /// 从Redis数据库中获取指定key的对象
        /// </summary>
        /// <typeparam name="T">期望的返回数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>如果不存在，则为空，否则为T的实例。</returns>
        Task<T> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从Redis数据库中获取指定key的对象
        /// </summary>
        /// <typeparam name="T">期望的返回数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>如果不存在，则为空，否则为T的实例。</returns>
        T Get<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  从Redis数据库中获取指定key的对象并更新过期时间
        /// </summary>
        /// <typeparam name="T">期望的返回数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="expiresIn">过期时间</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        ///  >如果不存在，则为空，否则为T的实例。
        /// </returns>
        Task<T> GetAsync<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  从Redis数据库中获取指定key的对象并更新过期时间
        /// </summary>
        /// <typeparam name="T">期望的返回数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="expiresIn">过期时间</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        ///  >如果不存在，则为空，否则为T的实例。
        /// </returns>
        T Get<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 添加一个指定的不过期的对象到redis数据库
        /// </summary>
        /// <typeparam name="T">添加到redis的数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">指定T类型的实例</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>T如果已添加对象，则为true。否则false</returns>
        Task<bool> SetAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 添加一个指定的不过期的对象到redis数据库
        /// </summary>
        /// <typeparam name="T">添加到redis的数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">指定T类型的实例</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>T如果已添加对象，则为true。否则false</returns>
        bool Set<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 添加一个有过期时间的对象到redis数据库
        /// </summary>
        /// <typeparam name="T">添加到redis的数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">指定T类型的实例</param>
        /// <param name="expiresIn">缓存有效持续的时长TimeSpan类型;如TimeSpan.FromSecond(10)</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>T如果已添加对象，则为true。否则false</returns>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 添加一个有过期时间的对象到redis数据库
        /// </summary>
        /// <typeparam name="T">添加到redis的数据类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="value">指定T类型的实例</param>
        /// <param name="expiresIn">缓存有效持续的时长TimeSpan类型;如TimeSpan.FromSecond(10)</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>T如果已添加对象，则为true。否则false</returns>
        bool Set<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">递增值,默认增长1</param>
        /// <returns></returns>
        Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null);

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">递增值,默认增长1</param>
        /// <returns></returns>
        long StringIncrement(string key, long value = 1, TimeSpan? expiry = null);

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        long StringDecrement(string key, long value = 1, TimeSpan? expiry = null);

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null);

        /// <summary>
        ///  查询指定缓存key集合的对象
        /// </summary>
        /// <typeparam name="T">key对应的对象的类型</typeparam>
        /// <param name="keys">缓存key</param>
        /// <returns>
        /// 如果没有结果，则为空列表，否则为实例T。
        /// 如果Redis上没有缓存键，指定的对象返回的字典将为空
        /// </returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        /// <summary>
        ///  查询指定缓存key集合的对象
        /// </summary>
        /// <typeparam name="T">key对应的对象的类型</typeparam>
        /// <param name="keys">缓存key</param>
        /// <returns>
        /// 如果没有结果，则为空列表，否则为实例T。
        /// 如果Redis上没有缓存键，指定的对象返回的字典将为空
        /// </returns>
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        /// <summary>
        ///  查询指定缓存key集合的对象并更新key的过期时间
        /// </summary>
        /// <typeparam name="T">key对应的对象的类型</typeparam>
        /// <param name="keys">缓存key</param>
        /// <param name="expiresIn">更新key的过期时间</param>
        /// <returns>
        /// 如果没有结果，则为空列表，否则为实例T。
        /// 如果Redis上没有缓存键，指定的对象返回的字典将为空
        /// </returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, TimeSpan expiresIn);

        /// <summary>
        ///  查询指定缓存key集合的对象并更新key的过期时间
        /// </summary>
        /// <typeparam name="T">key对应的对象的类型</typeparam>
        /// <param name="keys">缓存key</param>
        /// <param name="expiresIn">更新key的过期时间</param>
        /// <returns>
        /// 如果没有结果，则为空列表，否则为实例T。
        /// 如果Redis上没有缓存键，指定的对象返回的字典将为空
        /// </returns>
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys, TimeSpan expiresIn);

        /// <summary>
        /// 通过一次往返将指定键值的对象添加到Redis数据库中
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="items">项.</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 通过一次往返将指定键值的对象添加到Redis数据库中
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="items">项.</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        bool SetAll<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 通过一次往返将指定键值的对象添加到Redis数据库中
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="items">项 string:key;T:key对应的缓存对象</param>
        /// <param name="expiresIn">过期时间</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 通过一次往返将指定键值的对象添加到Redis数据库中
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="items">项 string:key;T:key对应的缓存对象</param>
        /// <param name="expiresIn">过期时间</param>
        /// <param name="when">插入条件(默认 Always,无论是否存在值都执行插入操作).</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        bool SetAll<T>(IList<Tuple<string, T>> items, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 将指定的成员添加到存储在键处的集合中。指定的成员为已经忽略该集合的一个成员。如果键不存在，则有一个新的键集/在添加指定成员之前创建。 
        /// 执行SADD命令 http://redis.io/commands/sadd
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="item">成员名称</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<bool> SetAddAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 将指定的成员添加到存储在键处的集合中。指定的成员为已经忽略该集合的一个成员。如果键不存在，则有一个新的键集/在添加指定成员之前创建。 
        /// 执行SADD命令 http://redis.io/commands/sadd
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="item">成员名称</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        bool SetAdd<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 将指定的成员添加到存储在键处的集合中。指定的成员为已经忽略该集合的一个成员。如果键不存在，则有一个新的键集/在添加指定成员之前创建
        /// 执行SADD命令 http://redis.io/commands/sadd
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="items">成员名称</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<long> SetAddAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class;

        /// <summary>
        /// 将指定的成员添加到存储在键处的集合中。指定的成员为已经忽略该集合的一个成员。如果键不存在，则有一个新的键集/在添加指定成员之前创建
        /// 执行SADD命令 http://redis.io/commands/sadd
        /// </summary>
        /// <typeparam name="T">期望对象的类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="items">成员名称</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        long SetAddAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class;

        /// <summary>
        ///  移除指定key并返回一个随机元素
        ///  Run SPOP command https://redis.io/commands/spop
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<T> SetPopAsync<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        ///  移除指定key并返回一个随机元素
        ///  Run SPOP command https://redis.io/commands/spop
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        T SetPop<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 从设置的值中移除并返回指定数量的随机元素    Run SPOP command https://redis.io/commands/spop
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="count">返回随机数的数量</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<IEnumerable<T>> SetPopAsync<T>(string key, long count, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        /// 从设置的值中移除并返回指定数量的随机元素    Run SPOP command https://redis.io/commands/spop
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">缓存key</param>
        /// <param name="count">返回随机数的数量</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        IEnumerable<T> SetPop<T>(string key, long count, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        ///  删除指定key所在集合中指定的item对象
        ///  Run SREM command http://redis.io/commands/srem"
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">集合所在的key</param>
        /// <param name="item">存储到redis中的对象</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<bool> SetRemoveAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        ///  删除指定key所在集合中指定的item对象
        ///  Run SREM command http://redis.io/commands/srem"
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">集合所在的key</param>
        /// <param name="item">存储到redis中的对象</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        bool SetRemove<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class;

        /// <summary>
        ///  删除指定key所在集合中指定的item对象集合
        ///  Run SREM command http://redis.io/commands/srem
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">集合所在的key</param>
        /// <param name="items">存储到redis中的对象</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<long> SetRemoveAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class;

        /// <summary>
        ///  删除指定key所在集合中指定的item对象集合
        ///  Run SREM command http://redis.io/commands/srem
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">集合所在的key</param>
        /// <param name="items">存储到redis中的对象</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        long SetRemoveAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class;

        /// <summary>
        /// 返回存储在键上的集合的所有成员。
        /// Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="key">成员的key名称.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<string[]> SetMemberAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回存储在键上的集合的所有成员。
        /// Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="key">成员的key名称.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        string[] SetMember(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 查看命令  Run SMEMBERS http://redis.io/commands/SMEMBERS
        /// 返回存储在键上的集合的所有成员并序列化为指定类型T
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">成员的key名称.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>集合中的对象数组</returns>
        Task<IEnumerable<T>> SetMembersAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 查看命令  Run SMEMBERS http://redis.io/commands/SMEMBERS
        /// 返回存储在键上的集合的所有成员并序列化为指定类型T
        /// </summary>
        /// <typeparam name="T">期望对象的类型.</typeparam>
        /// <param name="key">成员的key名称.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>集合中的对象数组</returns>
        IEnumerable<T> SetMembers<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 搜索匹配模式的所有key
        /// </summary>
        /// <remarks>
        /// 谨慎使用此命令,匹配模式搜索可能会损耗性能
        /// </remarks>
        /// <param name="pattern">模式</param>
        /// <example>
        /// 如果你想返回所有以"leon"开头的所有key，pattern使用"leon*"
        /// 如果你想返回所有以"leon"结束的所有key，pattern使用"*leon"
        /// 如果你向返回所有包含"leon"的所有key，pattern使用"*leon*"
        /// </example>
        /// <returns>从Redis数据库通过pattern查询到的的缓存键列表</returns>
        Task<IEnumerable<string>> SearchKeysAsync(string pattern);

        /// <summary>
        /// 搜索匹配模式的所有key
        /// </summary>
        /// <remarks>
        /// 谨慎使用此命令,匹配模式搜索可能会损耗性能
        /// </remarks>
        /// <param name="pattern">模式</param>
        /// <example>
        /// 如果你想返回所有以"leon"开头的所有key，pattern使用"leon*"
        /// 如果你想返回所有以"leon"结束的所有key，pattern使用"*leon"
        /// 如果你向返回所有包含"leon"的所有key，pattern使用"*leon*"
        /// </example>
        /// <returns>从Redis数据库通过pattern查询到的的缓存键列表</returns>
        IEnumerable<string> SearchKeys(string pattern);

        /// <summary>
        /// 在后台异步保存数据库。
        /// </summary>
        Task SaveAsync(SaveType saveType, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 获取关于redis的信息。
        /// More info see http://redis.io/commands/INFO
        /// </summary>
        Task<Dictionary<string, string>> GetInfoAsync();

        /// <summary>
        /// 获取与类别有关的redis信息。
        /// More info see http://redis.io/commands/INFO
        /// </summary>
        Task<List<InfoDetail>> GetInfoCategorizedAsync();

        /// <summary>
        ///     Updates the expiry time of a redis cache object
        /// </summary>
        /// <param name="key">The key of the object</param>
        /// <param name="expiresAt">The new expiry time of the object</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>True if the object is updated, false if the object does not exist</returns>
        Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///     Updates the expiry time of a redis cache object
        /// </summary>
        /// <param name="key">The key of the object</param>
        /// <param name="expiresIn">Time until the object will expire</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>True if the object is updated, false if the object does not exist</returns>
        Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///     Updates the expiry time of a redis cache object
        /// </summary>
        /// <param name="keys">An array of keys to be updated</param>
        /// <param name="expiresAt">The new expiry time of the object</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>An array of type bool, where true if the object is updated and false if the object does not exist at the same index as the input keys</returns>
        Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///     Updates the expiry time of a redis cache object
        /// </summary>
        /// <param name="keys">An array of keys to be updated</param>
        /// <param name="expiresIn">Time until the object will expire</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>An IDictionary object that contains the origional key and the result of the operation</returns>
        Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// lua脚本
        /// obj :new {key=key}
        /// </summary>
        /// <param name="script"></param>
        /// <param name="obj"></param>
        object GetByLuaScript(string script, object obj);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">被锁的资源</param>
        /// <param name="timeoutSeconds">超时时间</param>
        /// <param name="func">待执行的操作</param>
        /// <param name="waitTime">等待时间</param>
        /// <param name="retryTime">重试时间</param>
        /// <returns>是否拿到锁并执行成功
        /// true:拿到锁，并成功执行委托
        /// false:未拿到锁</returns>
        bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">被锁的资源</param>
        /// <param name="timeoutSeconds">超时时间</param>
        /// <param name="func">待执行的操作</param>
        /// <param name="waitTime">等待时间</param>
        /// <param name="retryTime">重试时间</param>
        /// <returns>是否拿到锁并执行成功
        /// true:拿到锁，并成功执行委托
        /// false:未拿到锁</returns>
        Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">被锁的资源</param>
        /// <param name="timeoutSeconds">超时时间</param>
        /// <param name="func">待执行的操作</param>
        /// <returns>是否拿到锁并执行成功
        /// true:拿到锁，并成功执行委托
        /// false:未拿到锁</returns>
        bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource">被锁的资源</param>
        /// <param name="timeoutSeconds">超时时间</param>
        /// <param name="func">待执行的操作</param>
        /// <returns>是否拿到锁并执行成功
        /// true:拿到锁，并成功执行委托
        /// false:未拿到锁</returns>
        Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds);
    }
}
