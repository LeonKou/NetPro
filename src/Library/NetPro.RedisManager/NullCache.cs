using NetPro.RedisManager;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
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
            return default;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return default;
        }

        public T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1, bool isLocalCache = false)
        {
            return default;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, bool isLocalCache = false)
        {
            return default;
        }

        public object GetByLuaScript(string script, object obj)
        {
            return default;
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func,bool isAwait )
        {
            return default;
        }

        public IDatabase GetIDatabase()
        {
            return default;
        }

        public T HGet<T>(string key, string field)
        {
            return default;
        }

        public Task<T> HGetAsync<T>(string key, string field)
        {
            return default;
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
        {
            return default;
        }

        public Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
        {
            return default;
        }

        public bool Exists(string key)
        {
            return default;
        }

        public bool Remove(string key)
        {
            return default;
        }

        public bool Remove(string[] keys)
        {
            return default;
        }

        public bool Set(string key, object data, int cacheTime = -1)
        {
            return default;
        }

        public Task<bool> SetAsync(string key, object data, int cacheTime = -1)
        {
            return default;
        }

        public bool ZAdd<T>(string key, T obj, decimal score)
        {
            return default;
        }

        public List<T> ZRange<T>(string key)
        {
            return default;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public long Publish(string channel, string input)
        {
            return default;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public Task<long> PublishAsync(string channel, string input)
        {
            return default;
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public string Subscriber(string channel)
        {
            return default;
        }


        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <returns>收到的消息</returns>
        public Task<string> SubscriberAsync(string channel)
        {
            return default;
        }
    }
}
