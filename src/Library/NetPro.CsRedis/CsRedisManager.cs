using CSRedis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.CsRedis
{
    /// <summary>
    /// 
    /// </summary>
    public class CsRedisManager : IRedisManager
    {
        private readonly RedisCacheOption _option;
        private readonly ISerializer _serializer;
        private IMemoryCache _memorycache;
        private ILogger _logger;
        private readonly IdleBus<CSRedisClient> _cSRedisClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="serializer"></param>
        /// <param name="cSRedisClient"></param>
        /// <param name="memorycache"></param>
        /// <param name="logger"></param>
        public CsRedisManager(RedisCacheOption option,
            ISerializer serializer
            , IdleBus<CSRedisClient> cSRedisClient
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

        public T Get<T>(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).Get<T>(key);
            return result;
        }

        public async Task<T> GetAsync<T>(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = await _cSRedisClient.Get(dbKey).GetAsync<T>(key);
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
        public T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            if (localExpiredTime > 0 && TimeSpan.FromSeconds(localExpiredTime) <= expiredTime && _memorycache != null)
            {
                var memoryResult = _memorycache.GetOrCreate<T>(key, s =>
                {
                    if (func == null)
                    {
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                        return default(T);
                    }

                    var resultTemp = _(key, func, expiredTime, dbKey);

                    if (resultTemp == null)
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    else
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(localExpiredTime);

                    return resultTemp;
                });
                return memoryResult;
            }
            return _(key, func, expiredTime, dbKey);
        }

        private T _<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, string dbKey = default)
        {
            var result = _cSRedisClient.Get(dbKey).Get<T>(key);

            //引用类型
            if (typeof(T).IsClass && result == null)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);

                if (expiredTime.HasValue)
                    _cSRedisClient.Get(dbKey).Set(key, executeResult, expiredTime.Value);
                else
                    _cSRedisClient.Get(dbKey).Set(key, executeResult);

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
                            _cSRedisClient.Get(dbKey).Set(key, executeResult, expiredTime.Value);
                        else
                            _cSRedisClient.Get(dbKey).Set(key, executeResult);
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
                            _cSRedisClient.Get(dbKey).Set(key, executeResult, expiredTime.Value);
                        else
                            _cSRedisClient.Get(dbKey).Set(key, executeResult);
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

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
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

        private async Task<T> _Async<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, string dbKey = default)
        {
            var result = await _cSRedisClient.Get(dbKey).GetAsync<T>(key);

            //引用类型
            if (typeof(T).IsClass && result == null)
            {
                if (func == null) return default(T);
                var executeResult = await func.Invoke();
                if (executeResult == null) return default(T);

                if (expiredTime.HasValue)
                    await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult, expiredTime.Value);
                else
                    await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult);
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
                            await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult, expiredTime.Value);
                        else
                            await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult);
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
                            await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult, expiredTime.Value);
                        else
                            await _cSRedisClient.Get(dbKey).SetAsync(key, executeResult);
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

        public object GetByLuaScript(string script, object obj, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).Eval(script, key: "lock_name", args: obj);
            return result;
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            //resource = AddDefaultPrefixKey(resource);

            if (isAwait)
                goto gotoNext;
            using (var _lockObject = _cSRedisClient.Get(dbKey).Lock(resource, 1))
            {
                if (_lockObject == null)
                {
                    return default;
                }
                goto gotoNext;
            }

        gotoNext:
            {
                using (var lockObject = _cSRedisClient.Get(dbKey).Lock(resource, timeoutSeconds))
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

        public long HashDelete(string key, IEnumerable<string> field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).HDel(key, field.ToArray());
        }
        public async Task<long> HashDeleteAsync(string key, IEnumerable<string> field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).HDelAsync(key, field.ToArray());
        }

        public long HashDelete(string key, string[] field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).HDel(key, field);
        }

        public async Task<long> HashDeleteAsync(string key, string[] field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).HDelAsync(key, field);
        }

        public async Task<bool> HashExistsAsync(string key, string hashField, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).HExistsAsync(key, hashField);
        }

        public bool HashExists(string key, string hashField, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).HExists(key, hashField);
        }

        public T HashGet<T>(string key, string field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).HGet<T>(key, field);
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).HGetAll<T>(key);
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).HGetAllAsync<T>(key);
        }

        public async Task<T> HashGetAsync<T>(string key, string field, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).HGetAsync<T>(key, field);
        }

        public bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            bool isSet = _cSRedisClient.Get(dbKey).HSet(key, field, value);
            if (isSet && expiredTime.HasValue)
                _cSRedisClient.Get(dbKey).Expire(key, expiredTime.Value);
            return isSet;
        }

        public async Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            bool isSet = await _cSRedisClient.Get(dbKey).HSetAsync(key, field, value);
            if (isSet && expiredTime.HasValue)
                await _cSRedisClient.Get(dbKey).ExpireAsync(key, expiredTime.Value);
            return isSet;
        }

        public bool KeyExpire(string key, TimeSpan expiration, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).Expire(key, expiration);
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan expiration, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).ExpireAsync(key, expiration);
        }

        public bool Exists(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).Exists(key);
        }

        public async Task<bool> ExistsAsync(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).ExistsAsync(key);
        }

        public long Remove(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).Del(key);
        }

        public async Task<long> RemoveAsync(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).DelAsync(key);
        }

        public long Remove(string[] keys, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).Del(keys);
        }

        public async Task<long> RemoveAsync(string[] keys, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).DelAsync(keys);
        }

        public bool Set(string key, object data, TimeSpan? expiredTime, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            if (expiredTime.HasValue)
                return _cSRedisClient.Get(dbKey).Set(key, data, expiredTime.Value);
            else
                return _cSRedisClient.Get(dbKey).Set(key, data);
        }

        public async Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            if (expiredTime.HasValue)
                return await _cSRedisClient.Get(dbKey).SetAsync(key, data, expiredTime.Value);
            else
                return await _cSRedisClient.Get(dbKey).SetAsync(key, data);
        }

        public long SortedSetAdd<T>(string key, T obj, decimal score, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).ZAdd(key, (score, obj));
        }

        public async Task<long> SortedSetAddAsync<T>(string key, T obj, decimal score, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return await _cSRedisClient.Get(dbKey).ZAddAsync(key, (score, obj));
        }

        public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).ZRange<T>(key, start, stop)?.ToList();
        }

        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = await _cSRedisClient.Get(dbKey).ZRangeAsync<T>(key, start, stop);
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
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).IncrBy(key, value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).Expire(key, expiry.Value);
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
        public async Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = await _cSRedisClient.Get(dbKey).IncrByAsync(key, value);
            if (expiry.HasValue)
                await _cSRedisClient.Get(dbKey).ExpireAsync(key, expiry.Value);
            return result;
        }

        /// <summary>
        /// value递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).IncrBy(key, -value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).Expire(key, expiry.Value);
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
        public async Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = await _cSRedisClient.Get(dbKey).IncrByAsync(key, -value);
            if (expiry.HasValue)
                await _cSRedisClient.Get(dbKey).ExpireAsync(key, expiry.Value);
            return result;
        }

        public long KeyTimeToLive(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var time = _cSRedisClient.Get(dbKey).Ttl(key);
            return time;
        }

        public async Task<long> KeyTimeToLiveAsync(string key, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var time = await _cSRedisClient.Get(dbKey).TtlAsync(key);
            return time;
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public long Publish(string key, string message, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            return _cSRedisClient.Get(dbKey).PublishNoneMessageId(key, message);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync(string channel, string input, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = await _cSRedisClient.Get(dbKey).PublishNoneMessageIdAsync(channel, input);
            return result;
        }

        /// <summary>
        /// 订阅消息,多端非争抢模式;数据会丢失
        /// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)),
        /// ("chan2", msg => Console.WriteLine(msg.Body)))
        /// </summary>
        /// <param name="channels">管道</param>
        /// <returns>收到的消息</returns>
        public void Subscribe(string dbKey = default, params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).Subscribe(channels);
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
        public void SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage, string dbKey = default)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).SubscribeListBroadcast(listKey, clientId, onMessage);
            result.Dispose();
        }

        public long HashIncrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).HIncrBy(key, field, value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).Expire(key, expiry.Value);
            return result;
        }

        public Task<long> HashIncrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).HIncrByAsync(key, field, value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).ExpireAsync(key, expiry.Value);
            return result;
        }

        public long HashDecrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).HIncrBy(key, field, -value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).Expire(key, expiry.Value);
            return result;
        }

        public Task<long> HashDecrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            if (string.IsNullOrWhiteSpace(dbKey))
            {
                dbKey = $"{_option.ConnectionString.FirstOrDefault()?.Key}";
            }
            var result = _cSRedisClient.Get(dbKey).HIncrByAsync(key, field, -value);
            if (expiry.HasValue)
                _cSRedisClient.Get(dbKey).ExpireAsync(key, expiry.Value);
            return result;
        }

        //private string AddDefaultPrefixKey(string key)
        //{
        //    var build = new StringBuilder(_option?.DefaultCustomKey ?? string.Empty);
        //    return build.Append(key).ToString();
        //}
    }
}
