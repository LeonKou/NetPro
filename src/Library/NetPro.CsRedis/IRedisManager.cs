using CSRedis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.CsRedis
{
    /// <summary>
    /// 默认只支持单个Redis
    /// </summary>
    public interface IRedisManager
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key, string dbKey = default);

        /// <summary>
        /// 异步获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key, string dbKey = default);

        /// <summary>
        ///获取或者创建缓存 
        /// localExpiredTime参数大于0并且小于expiredTime数据将缓存到本地内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="localExpiredTime">本地过期时间</param>
        /// <returns></returns>
        T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default);

        /// <summary>
        ///获取或者创建缓存 
        /// localExpiredTime参数大于0并且小于expiredTime数据将缓存到本地内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="localExpiredTime">本地过期时间</param>
        /// <returns></returns>
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default);

        /// <summary>
        ///新增缓存
        /// </summary>
        /// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
        /// <param name="data">Value for caching</param>
        /// <param name="expiredTime">Cache time in minutes</param>
        bool Set(string key, object data, TimeSpan? expiredTime = null, string dbKey = default);

        /// <summary>
        ///新增缓存
        /// </summary>
        /// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
        /// <param name="data">Value for caching</param>
        /// <param name="expiredTime">Cache time in minutes</param>
        Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime = null, string dbKey = default);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key, string dbKey = default);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key, string dbKey = default);

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long Remove(string key, string dbKey = default);

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>删除的个数</returns>
        Task<long> RemoveAsync(string key, string dbKey = default);

        /// <summary>
        /// 批量移除key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>删除的个数</returns>
        long Remove(string[] keys, string dbKey = default);

        /// <summary>
        /// 批量移除key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<long> RemoveAsync(string[] keys, string dbKey = default);

        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        long SortedSetAdd<T>(string key, T obj, decimal score, string dbKey = default);

        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        Task<long> SortedSetAddAsync<T>(string key, T obj, decimal score, string dbKey = default);

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, string dbKey = default);

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, string dbKey = default);

        /// <summary>
        /// 获取一个分布式锁,不支持嵌套锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="expiredTime">超时过期事件，单位秒</param>
        /// <param name="func"></param>
        /// <param name="isAwait">是否等待</param>
        /// <returns></returns>
        T GetDistributedLock<T>(string resource, int expiredTime, Func<T> func, bool isAwait = true, string dbKey = default);

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Dictionary<string, T> HashGetAll<T>(string key, string dbKey = default);

        /// <summary>
        ///   获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, string dbKey = default);

        /// <summary>
        /// 删除hash中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        long HashDelete(string key, IEnumerable<string> field, string dbKey = default);

        /// <summary>
        /// 删除hash中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        long HashDelete(string key, string[] field, string dbKey = default);

        /// <summary>
        /// 删除hash中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync(string key, IEnumerable<string> field, string dbKey = default);

        /// <summary>
        /// 删除hash中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync(string key, string[] field, string dbKey = default);

        /// <summary>
        /// 检查 hash条目中的 key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string key, string hashField, string dbKey = default);

        /// <summary>
        /// 检查 hash条目中的 key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        bool HashExists(string key, string hashField, string dbKey = default);

        /// <summary>
        /// 设置或更新Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default);

        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string field, string dbKey = default);

        /// <summary>
        /// 设置或更新Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime">过期时间</param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default);

        /// <summary>
        /// 获取Hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<T> HashGetAsync<T>(string key, string field, string dbKey = default);

        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        long HashIncrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        Task<long> HashIncrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值递减 Decrement
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        long HashDecrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        Task<long> HashDecrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// lua脚本
        /// obj :new {key=key}
        /// </summary>
        /// <param name="script"></param>
        /// <param name="obj"></param>
        object GetByLuaScript(string script, object obj, string dbKey = default);

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">递增值</param>
        /// <returns></returns>
        long StringIncrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        long StringDecrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default);

        /// <summary>
        /// 返回具有超时的键的剩余生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> KeyTimeToLiveAsync(string key, string dbKey = default);

        /// <summary>
        /// 返回具有超时的键的剩余生存时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long KeyTimeToLive(string key, string dbKey = default);

        /// <summary>
        /// 设置一个超时键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns>true:设置成功，false：设置失败</returns>
        Task<bool> KeyExpireAsync(string key, TimeSpan expiry, string dbKey = default);

        /// <summary>
        /// 设置一个超时键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiration">过期时间</param>
        /// <returns>true:设置成功，false：设置失败</returns>
        bool KeyExpire(string key, TimeSpan expiration, string dbKey = default);

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        long Publish(string key, string message, string dbKey = default);
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="input"></param>
        Task<long> PublishAsync(string channel, string input, string dbKey = default);

        /// <summary>
        /// 订阅消息,多端非争抢模式;数据会丢失
        /// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)),
        /// ("chan2", msg => Console.WriteLine(msg.Body)))
        /// </summary>
        /// <param name="channels">管道</param>
        /// <returns>收到的消息</returns>
        void Subscribe(string dbKey = default, params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels);

        //
        // Summary:
        //     使用lpush + blpop订阅端（多端非争抢模式），都可以收到消息
        //
        // Parameters:
        //   listKey:
        //     list key（不含prefix前辍）
        //
        //   clientId:
        //     订阅端标识，若重复则争抢，若唯一必然收到消息
        //
        //   onMessage:
        //     接收消息委托
        void SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage, string dbKey = default);
    }
}