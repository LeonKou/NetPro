using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace NetPro.RedisManager
{
    internal class CsRedisManager : IRedisManager
    {
        private readonly RedisCacheOption _option;
        private readonly ConnectionMultiplexer _connection;
        private IMemoryCache _memorycache;
        public CsRedisManager(RedisCacheOption option,
            ConnectionMultiplexer connection,
               IMemoryCache memorycache)
        {
            _connection = connection;
            _option = option;
            _memorycache = memorycache;
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
        ///不过期或者过期时间时间大于一小时，数据将缓存到本地内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiredTime"></param>
        /// <param name="isLocalCache"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1, bool isLocalCache = false)
        {
            if (isLocalCache && (expiredTime == -1 || expiredTime > 3600))
            {
                var memoryResult = _memorycache.GetOrCreate<T>(key, s =>
                {
                    var resultTemp = _(key, func, expiredTime);
                    if (resultTemp == null)
                    {
                        s.AbsoluteExpirationRelativeToNow = new TimeSpan(1);
                        return resultTemp;
                    }
                    if (expiredTime > 3600)
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiredTime / 3);
                    return resultTemp;
                });
                return memoryResult;
            }
            return _(key, func, expiredTime);
        }

        private T _<T>(string key, Func<T> func = null, int expiredTime = -1)
        {
            Common.CheckKey(key);
            var result = RedisHelper.Get<T>(key);

            if (result == null)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);

                RedisHelper.Set(key, executeResult, expiredTime);

                return executeResult;
            }

            return result;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, bool isLocalCache = false)
        {
            if (isLocalCache && (expiredTime == -1 || expiredTime > 3600))
            {
                var memoryResult = await _memorycache.GetOrCreateAsync<T>(key, async s =>
                {
                    var resultTemp = await _Async(key, func, expiredTime);
                    if (resultTemp == null)
                    {
                        s.AbsoluteExpirationRelativeToNow = new TimeSpan(1);
                        return resultTemp;
                    }
                    if (expiredTime > 3600)
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiredTime);
                    return resultTemp;
                });
                return memoryResult;
            }
            return await _Async(key, func, expiredTime);
        }

        private async Task<T> _Async<T>(string key, Func<Task<T>> func = null, int expiredTime = -1)
        {
            Common.CheckKey(key);
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

            if (isAwait)
                goto gotoNext;
            using (var _lockObject = RedisHelper.Lock($"Lock:{resource?.Trim()}", 1))
            {
                if (_lockObject == null)
                {
                    return default;
                }
                goto gotoNext;
            }

            gotoNext:
            {
                using (var lockObject = RedisHelper.Lock($"Lock:{resource?.Trim()}", timeoutSeconds))
                {
                    if (lockObject == null)
                    {
                        Console.WriteLine($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
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
            Common.CheckKey(key);
            return RedisHelper.HGet<T>(key, field);
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
            Common.CheckKey(key);

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

        public bool Remove(string key)
        {
            return RedisHelper.Del(key) > 0 ? true : false;
        }

        public bool Remove(string[] keys)
        {
            return RedisHelper.Del(keys) > 0 ? true : false;
        }

        public bool Set(string key, object data, int cacheTime = -1)
        {
            return RedisHelper.Set(key, data, cacheTime);
        }

        public async Task<bool> SetAsync(string key, object data, int cacheTime)
        {
            return await RedisHelper.SetAsync(key, data, cacheTime);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
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
    }
}
