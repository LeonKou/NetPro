using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
    /// <summary>
    /// Cache manager interface
    /// </summary>
    public interface IRedisManager
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 异步获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 获取缓存，没有则新增缓存
        /// 不过期或者过期时间时间大于一小时，数据将缓存到本地内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="isLocalCache">是否缓存本地</param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1, bool isLocalCache = false);

        /// <summary>
        /// 获取缓存没有则新增缓存
        /// 不过期或者过期时间时间大于一小时，数据将缓存到本地内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="isLocalCache">是否缓存本地</param>
        /// <returns></returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, bool isLocalCache = false);

        /// <summary>
        ///新增缓存
        /// </summary>
        /// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        bool Set(string key, object data, int cacheTime = -1);

        /// <summary>
        ///新增缓存
        /// </summary>
        /// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        Task<bool> SetAsync(string key, object data, int cacheTime);

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 移除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// 批量移除key
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        bool Remove(string[] keys);

        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        bool ZAdd<T>(string key, T obj, decimal score);

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> ZRange<T>(string key);

        /// <summary>
        /// 获取一个分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="func"></param>
        /// <param name="isAwait">是否等待</param>
        /// <returns></returns>
        T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait = true);

        bool HSet<T>(string key, string field, T value, int expirationMinute = 1);

        T HGet<T>(string key, string field);

        Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1);

        Task<T> HGetAsync<T>(string key, string field);

        /// <summary>
        /// lua脚本
        /// obj :new {key=key}
        /// </summary>
        /// <param name="script"></param>
        /// <param name="obj"></param>
        object GetByLuaScript(string script, object obj);

        /// <summary>
        /// 获得使用原生stackexchange.redis的能力，用于pipeline (stackExchange.redis专用，Csredis驱动使用此方法会报异常)
        /// </summary>
        /// <returns></returns>
        IDatabase GetIDatabase();

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        long Publish(string channel, string input);

        Task<long> PublishAsync(string channel, string input);

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        string Subscriber(string channel);

        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        Task<string> SubscriberAsync(string channel);
    }
}
