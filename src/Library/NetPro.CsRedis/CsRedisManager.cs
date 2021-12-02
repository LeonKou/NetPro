using CSRedis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetPro.CsRedis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.CsRedis
{
    public class CsRedisManager : IRedisManager
    {
        private readonly RedisCacheOption _option;
        private readonly ISerializer _serializer;
        private IMemoryCache _memorycache;
        private ILogger _logger;
        private readonly CSRedisClient _cSRedisClient ; 
        public CsRedisManager(RedisCacheOption option,
            ISerializer serializer
            , CSRedisClient cSRedisClient
            , IMemoryCache memorycache = null
            , ILogger<CsRedisManager> logger = null
            )
        {
            _serializer = serializer;
            _cSRedisClient = cSRedisClient;
            _option = option;
            _memorycache = memorycache;
            _logger = logger ?? NullLogger<CsRedisManager>.Instance; ;
        }

        public T Get<T>(string key)
        {
            var result = _cSRedisClient.Get<T>(key);
            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await _cSRedisClient.GetAsync<T>(key);
            return result;
        }

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
        public T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            if (localExpiredTime > 0 && TimeSpan.FromSeconds(localExpiredTime) <= expiredTime && _memorycache != null)
            {
                var memoryResult = _memorycache.GetOrCreate<T>(key, s =>
                {
                    if (func == null)
                    {
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                        return default(T);
                    }

                    var resultTemp = _(key, func, expiredTime);

                    if (resultTemp == null)
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    else
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(localExpiredTime);

                    return resultTemp;
                });
                return memoryResult;
            }
            return _(key, func, expiredTime);
        }

        private T _<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null)
        {
            var result = _cSRedisClient.Get<T>(key);

            //引用类型
            if (typeof(T).IsClass && result == null)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);

                if (expiredTime.HasValue)
                    _cSRedisClient.Set(key, executeResult, expiredTime.Value);
                else
                    _cSRedisClient.Set(key, executeResult);

                return executeResult;
            }
            //值类型
            else if (typeof(T).IsValueType)
            {
                if (typeof(T).Name.StartsWith("Nullable`1"))//可空类型
                {
                    if (result == null)
                    {
                        if (func == null) return default(T);
                        var executeResult = func.Invoke();
                        if (executeResult == null) return default(T);

                        if (expiredTime.HasValue)
                            _cSRedisClient.Set(key, executeResult, expiredTime.Value);
                        else
                            _cSRedisClient.Set(key, executeResult);
                        return executeResult;
                    }
                    else
                    {
                        return result;
                    }
                }
                else//不可空
                {
                    if (result.ToString() == typeof(T).GetDefault().ToString())
                    {
                        if (func == null) return default(T);
                        var executeResult = func.Invoke();
                        if (executeResult == null) return default(T);
                        if (expiredTime.HasValue)
                            _cSRedisClient.Set(key, executeResult, expiredTime.Value);
                        else
                            _cSRedisClient.Set(key, executeResult);
                        return executeResult;
                    }
                    else
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            if (localExpiredTime > 0 && TimeSpan.FromSeconds(localExpiredTime) <= expiredTime && _memorycache != null)
            {
                var memoryResult = await _memorycache.GetOrCreateAsync<T>(key, async s =>
                {
                    if (func == null)
                    {
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                        return default(T);
                    }
                    var executeResult = await _Async(key, func, expiredTime);

                    if (executeResult == null)
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    else
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(localExpiredTime);

                    return executeResult;
                });
                return memoryResult;
            }
            return await _Async(key, func, expiredTime);
        }

        private async Task<T> _Async<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null)
        {
            var result = await _cSRedisClient.GetAsync<T>(key);

            //引用类型
            if (typeof(T).IsClass && result == null)
            {
                if (func == null) return default(T);
                var executeResult = await func.Invoke();
                if (executeResult == null) return default(T);

                if (expiredTime.HasValue)
                    await _cSRedisClient.SetAsync(key, executeResult, expiredTime.Value);
                else
                    await _cSRedisClient.SetAsync(key, executeResult);
                return executeResult;
            }
            //值类型
            else if (typeof(T).IsValueType)
            {
                if (typeof(T).Name.StartsWith("Nullable`1"))//可空类型
                {
                    if (result == null)
                    {
                        if (func == null) return default(T);
                        var executeResult = await func.Invoke();
                        if (executeResult == null) return default(T);

                        if (expiredTime.HasValue)
                            await _cSRedisClient.SetAsync(key, executeResult, expiredTime.Value);
                        else
                            await _cSRedisClient.SetAsync(key, executeResult);
                        return executeResult;
                    }
                    else
                    {
                        return result;
                    }
                }
                else//不可空
                {
                    if (result.ToString() == typeof(T).GetDefault().ToString())
                    {
                        if (func == null) return default(T);
                        var executeResult = await func.Invoke();
                        if (executeResult == null) return default(T);
                        if (expiredTime.HasValue)
                            await _cSRedisClient.SetAsync(key, executeResult, expiredTime.Value);
                        else
                            await _cSRedisClient.SetAsync(key, executeResult);
                        return executeResult;
                    }
                    else
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        public object GetByLuaScript(string script, object obj)
        {
            var result = _cSRedisClient.Eval(script, key: "lock_name", args: obj);
            return result;
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait)
        {
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            resource = AddDefaultPrefixKey(resource);

            if (isAwait)
                goto gotoNext;
            using (var _lockObject = _cSRedisClient.Lock(resource, 1))
            {
                if (_lockObject == null)
                {
                    return default;
                }
                goto gotoNext;
            }

            gotoNext:
            {
                using (var lockObject = _cSRedisClient.Lock(resource, timeoutSeconds))
                {
                    if (lockObject == null)
                    {
                        _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
                        return default;
                    }
                    var result = func();
                    if (lockObject.Unlock())
                        return result;
                    return default;
                }
            }
        }

        public long HashDelete(string key, IEnumerable<string> field)
        {
            return _cSRedisClient.HDel(key, field.ToArray());
        }
        public async Task<long> HashDeleteAsync(string key, IEnumerable<string> field)
        {
            return await _cSRedisClient.HDelAsync(key, field.ToArray());
        }

        public long HashDelete(string key, string[] field)
        {
            return _cSRedisClient.HDel(key, field);
        }

        public async Task<long> HashDeleteAsync(string key, string[] field)
        {
            return await _cSRedisClient.HDelAsync(key, field);
        }

        public async Task<bool> HashExistsAsync(string key, string hashField)
        {
            return await _cSRedisClient.HExistsAsync(key, hashField);
        }

        public bool HashExists(string key, string hashField)
        {
            return _cSRedisClient.HExists(key, hashField);
        }

        public T HashGet<T>(string key, string field)
        {
            return _cSRedisClient.HGet<T>(key, field);
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            return _cSRedisClient.HGetAll<T>(key);
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
        {
            return await _cSRedisClient.HGetAllAsync<T>(key);
        }

        public async Task<T> HashGetAsync<T>(string key, string field)
        {
            return await _cSRedisClient.HGetAsync<T>(key, field);
        }

        public bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            bool isSet = _cSRedisClient.HSet(key, field, value);
            if (isSet && expiredTime.HasValue)
                _cSRedisClient.Expire(key, expiredTime.Value);
            return isSet;
        }

        public async Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            bool isSet = await _cSRedisClient.HSetAsync(key, field, value);
            if (isSet && expiredTime.HasValue)
                await _cSRedisClient.ExpireAsync(key, expiredTime.Value);
            return isSet;
        }

        public bool KeyExpire(string key, TimeSpan expiration)
        {
            return _cSRedisClient.Expire(key, expiration);
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan expiration)
        {
            return await _cSRedisClient.ExpireAsync(key, expiration);
        }

        public bool Exists(string key)
        {
            return _cSRedisClient.Exists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _cSRedisClient.ExistsAsync(key);
        }

        public long Remove(string key)
        {
            return _cSRedisClient.Del(key);
        }

        public async Task<long> RemoveAsync(string key)
        {
            return await _cSRedisClient.DelAsync(key);
        }

        public long Remove(string[] keys)
        {
            return _cSRedisClient.Del(keys);
        }

        public async Task<long> RemoveAsync(string[] keys)
        {
            return await _cSRedisClient.DelAsync(keys);
        }

        public bool Set(string key, object data, TimeSpan? expiredTime)
        {
            if (expiredTime.HasValue)
                return _cSRedisClient.Set(key, data, expiredTime.Value);
            else
                return _cSRedisClient.Set(key, data);
        }

        public async Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime)
        {
            if (expiredTime.HasValue)
                return await _cSRedisClient.SetAsync(key, data, expiredTime.Value);
            else
                return await _cSRedisClient.SetAsync(key, data);
        }

        public long SortedSetAdd<T>(string key, T obj, decimal score)
        {
            return _cSRedisClient.ZAdd(key, (score, obj));
        }

        public async Task<long> SortedSetAddAsync<T>(string key, T obj, decimal score)
        {
            return await _cSRedisClient.ZAddAsync(key, (score, obj));
        }

        public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1)
        {
            return _cSRedisClient.ZRange<T>(key, start, stop)?.ToList();
        }

        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1)
        {
            var result = await _cSRedisClient.ZRangeAsync<T>(key, start, stop);
            return result.ToList();
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = _cSRedisClient.IncrBy(key, value);
            if (expiry.HasValue)
                _cSRedisClient.Expire(key, expiry.Value);
            return result;
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public async Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = await _cSRedisClient.IncrByAsync(key, value);
            if (expiry.HasValue)
                await _cSRedisClient.ExpireAsync(key, expiry.Value);
            return result;
        }

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = _cSRedisClient.IncrBy(key, -value);
            if (expiry.HasValue)
                _cSRedisClient.Expire(key, expiry.Value);
            return result;
        }

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public async Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = await _cSRedisClient.IncrByAsync(key, -value);
            if (expiry.HasValue)
                await _cSRedisClient.ExpireAsync(key, expiry.Value);
            return result;
        }

        public long KeyTimeToLive(string key)
        {
            var time = _cSRedisClient.Ttl(key);
            return time;
        }

        public async Task<long> KeyTimeToLiveAsync(string key)
        {
            var time = await _cSRedisClient.TtlAsync(key);
            return time;
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public long Publish(string key, string message)
        {
            return _cSRedisClient.PublishNoneMessageId(key, message);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync(string channel, string input)
        {
            var result = await _cSRedisClient.PublishNoneMessageIdAsync(channel, input);
            return result;
        }

        /// <summary>
        /// 订阅消息,多端非争抢模式;数据会丢失
        /// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)),
        /// ("chan2", msg => Console.WriteLine(msg.Body)))
        /// </summary>
        /// <param name="channels">管道</param>
        /// <returns>收到的消息</returns>
        public void Subscribe(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {
            var result = _cSRedisClient.Subscribe(channels);
            result.Dispose();
        }

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
        public void SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage)
        {
            var result = _cSRedisClient.SubscribeListBroadcast(listKey, clientId, onMessage);
            result.Dispose();
        }

        private string AddDefaultPrefixKey(string key)
        {
            var build = new StringBuilder(_option?.DefaultCustomKey ?? string.Empty);
            return build.Append(key).ToString();
        }
    }
}
