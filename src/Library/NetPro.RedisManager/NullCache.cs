using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
    internal class NullCache : IRedisManager
    {
        public NullCache()
        {
            Console.WriteLine("redis drive is null");
        }
        public T Get<T>(string key)
        {
            return default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return default(T);
        }

        public T GetOrSet<T>(string key, Func<T> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);

            if (func == null)
                return default(T);
            var executeResult = func.Invoke();
            return executeResult;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
            Common.CheckAndProcess(ref key, ref expiredTime);
            if (func == null)
                return default(T);
            var executeResult = await func.Invoke();
            return executeResult;
        }

        public object GetByLuaScript(string script, object obj)
        {
            return default;
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait)
        {
            return default(T);
        }

        public IDatabase GetIDatabase()
        {
            return default;
        }

        public T HGet<T>(string key, string field)
        {
            return default;
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
            return default(T);
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
        {
            return false;
        }

        public Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
        {
            return Task.FromResult(false);
        }

        public bool Exists(string key)
        {
            return false;
        }

        public bool Remove(string key)
        {
            return false;
        }

        public bool Remove(string[] keys)
        {
            return false;
        }

        public bool Set(string key, object data, int cacheTime = -1)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, int cacheTime = -1)
        {
            return Task.FromResult(false);
        }

        public bool ZAdd<T>(string key, T obj, decimal score)
        {
            return false;
        }

        public List<T> ZRange<T>(string key)
        {
            return null;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public long Publish(string channel, string input)
        {
            return 0;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public Task<long> PublishAsync(string channel, string input)
        {
            return Task.FromResult((long)0);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public string Subscriber(string channel)
        {
            return null;
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public Task<string> SubscriberAsync(string channel)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Task.FromResult(false);
        }

        public Task<bool> RemoveAsync(string[] keys)
        {
            return Task.FromResult(false);
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
            return 0;
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
            return 0;
        }
    }
}
