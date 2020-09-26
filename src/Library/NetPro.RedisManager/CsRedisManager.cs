using CSRedis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
    internal class CsRedisManager : IRedisManager
    {
        private readonly RedisCacheOption _option;
        private readonly ConnectionMultiplexer _connection;
        private IMemoryCache _memorycache;
        private ILogger _logger;
        public CsRedisManager(RedisCacheOption option,
            ConnectionMultiplexer connection,
               IMemoryCache memorycache, ILogger<CsRedisManager> logger)
        {
            _connection = connection;
            _option = option;
            _memorycache = memorycache;
            _logger = logger;
        }
        public T Get<T>(string key)
        {
            var result = _<T>(key);
            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await _Async<T>(key);
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
        public T GetOrSet<T>(string key, Func<T> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
            if (localExpiredTime > 0 && localExpiredTime <= expiredTime)
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

        private T _<T>(string key, Func<T> func = null, int expiredTime = -1)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);
            var result = RedisHelper.Get<T>(key);

            if (result == null)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);

                RedisHelper.SetAsync(key, executeResult, expiredTime);

                return executeResult;
            }

            return result;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
            if (localExpiredTime > 0 && localExpiredTime <= expiredTime)
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

        private async Task<T> _Async<T>(string key, Func<Task<T>> func = null, int expiredTime = -1)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);
            var result = await RedisHelper.GetAsync<T>(key);

            if (result == null)
            {
                if (func == null) return default(T);
                var executeResult = await func.Invoke();
                if (executeResult == null) return default(T);

                await RedisHelper.SetAsync(key, executeResult, expiredTime);

                return executeResult;
            }

            return result;
        }

        public object GetByLuaScript(string script, object obj)
        {
            var result = RedisHelper.Eval(script, key: "lock_name", args: obj);
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
            using (var _lockObject = RedisHelper.Lock(resource, 1))
            {
                if (_lockObject == null)
                {
                    return default;
                }
                goto gotoNext;
            }

            gotoNext:
            {
                using (var lockObject = RedisHelper.Lock(resource, timeoutSeconds))
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

        /// <summary>
        /// 获得使用原生stackexchange.redis的能力，用于pipeline (stackExchange.redis专用)
        /// </summary>
        /// <returns></returns>
        public IDatabase GetIDatabase()
        {
            return _connection.GetDatabase();
        }

        public T HGet<T>(string key, string field)
        {
            Common.CheckAndProcess(ref key);
            return RedisHelper.HGet<T>(key, field);
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
            Common.CheckAndProcess(ref key);

            return await RedisHelper.HGetAsync<T>(key, field);
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
        {
            bool isSet = RedisHelper.HSet(key, field, value);
            if (isSet && expirationMinute > 0)
                RedisHelper.Expire(key, TimeSpan.FromMinutes(expirationMinute));
            return isSet;
        }

        public async Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
        {
            bool isSet = await RedisHelper.HSetAsync(key, field, value);
            if (isSet && expirationMinute > 0)
                await RedisHelper.ExpireAsync(key, TimeSpan.FromMinutes(expirationMinute));
            return isSet;
        }

        public bool Exists(string key)
        {
            return RedisHelper.Exists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await RedisHelper.ExistsAsync(key);
        }

        public bool Remove(string key)
        {
            return RedisHelper.Del(key) > 0 ? true : false;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await RedisHelper.DelAsync(key) > 0 ? true : false;
        }

        public bool Remove(string[] keys)
        {
            return RedisHelper.Del(keys) > 0 ? true : false;
        }

        public async Task<bool> RemoveAsync(string[] keys)
        {
            return await RedisHelper.DelAsync(keys) > 0 ? true : false;
        }

        public bool Set(string key, object data, int expiredTime = -1)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);
            return RedisHelper.Set(key, data, expiredTime);
        }

        public async Task<bool> SetAsync(string key, object data, int expiredTime)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);
            return await RedisHelper.SetAsync(key, data, expiredTime);
        }

        public bool ZAdd<T>(string key, T obj, decimal score)
        {
            return RedisHelper.ZAdd(key, (score, obj)) > 0 ? true : false;
        }

        public List<T> ZRange<T>(string key)
        {
            return RedisHelper.ZRange<T>(key, 0, -1)?.ToList();
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public long StringIncrement(string key, long value = 1)
        {
            Common.CheckAndProcess(ref key);
            key = AddDefaultPrefixKey(key);
            return RedisHelper.IncrBy(key, value);
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public async Task<long> StringIncrementAsync(string key, long value = 1)
        {
            Common.CheckAndProcess(ref key);
            key = AddDefaultPrefixKey(key);
            return await RedisHelper.IncrByAsync(key, value);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Subscribe<T>(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {
            RedisHelper.Subscribe(channels);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public long Publish(string key, string message)
        {
            return RedisHelper.PublishNoneMessageId(key, message);
        }

        public async Task<long> PublishAsync(string channel, string input)
        {
            var result = await RedisHelper.PublishNoneMessageIdAsync(channel, input);
            return result;
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public string Subscriber(string channel)
        {
            string result = null;
            ISubscriber sub = _connection.GetSubscriber();
            //订阅名为 messages 的通道
            sub.Subscribe(channel, (channel, message) =>
            {
                result = message;
            });
            return result;
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public async Task<string> SubscriberAsync(string channel)
        {
            string result = null;
            ISubscriber sub = _connection.GetSubscriber();
            //订阅名为 messages 的通道
            await sub.SubscribeAsync(channel, (channel, message) =>
            {
                result = message;
            });
            return result;
        }

        private string AddDefaultPrefixKey(string key)
        {
            var build = new StringBuilder(_option?.DefaultCustomKey ?? string.Empty);
            return build.Append(key).ToString();
        }
    }
}
