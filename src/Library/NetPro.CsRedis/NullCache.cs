using CSRedis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.CsRedis
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


        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(default(T));
        }

        public T GetOrSet<T>(string key, Func<T> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
            if (func == null)
                return default(T);
            var executeResult = func.Invoke();
            return executeResult;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, int localExpiredTime = 0)
        {
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

        public T HGet<T>(string key, string field)
        {
            return default;
        }

        public  Task<T> HGetAsync<T>(string key, string field)
        {
            return default;
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

        public Task<bool> ExistsAsync(string key)
        {
            return default;
        }

        public long Remove(string key)
        {
            return 0;
        }

        public long Remove(string[] keys)
        {
            return 0;
        }

        public bool Set(string key, object data, int cacheTime = -1)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, int cacheTime = -1)
        {
            return Task.FromResult(false);
        }

        public long SortedSetAdd<T>(string key, T obj, decimal score)
        {
            return 0;
        }
        public  Task<long> SortedSetAddAsync<T>(string key, T obj, decimal score)
        {
            return default;
        }
        public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1)
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


        public Task<long> RemoveAsync(string key)
        {
            return Task.FromResult((long)0);
        }

        public Task<long> RemoveAsync(string[] keys)
        {
            return Task.FromResult((long)0);
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>TODO 待优化为脚本批量操作</remarks>
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null)
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
        public  Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            return default;
        }

        public void SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage)
        {
        }

        public bool Set(string key, object data, TimeSpan expiredTime)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, TimeSpan expiredTime)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HSetAsync<T>(string key, string field, T value, TimeSpan expiration)
        {
            return Task.FromResult(false);
        }

        public Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1)
        {
            return null;
        }

        public Task<long> KeyTimeToLiveAsync(string key)
        {
            return Task.FromResult((long)0);
        }

        public long KeyTimeToLive(string key)
        {
            return 0;
        }

        public Task<bool> KeyExpireAsync(string key, TimeSpan expiry)
        {
            return Task.FromResult(false);
        }

        public bool KeyExpire(string key, TimeSpan expiration)
        {
            return false;
        }

        public T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            return default(T);
        }

        public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            return Task.FromResult(default(T));
        }

        public bool Set(string key, object data, TimeSpan? expiredTime)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime)
        {
            return Task.FromResult(false);
        }

        public void Subscribe(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {

        }

        public long HashDelete(string key, IEnumerable<string> field)
        {
            return 0;
        }

        public long HashDelete(string key, string[] field)
        {
            return 0;
        }

        public Task<long> HashDeleteAsync(string key, IEnumerable<string> field)
        {
            return Task.FromResult((long)0);
        }

        public Task<long> HashDeleteAsync(string key, string[] field)
        {
            return Task.FromResult((long)0);
        }

        public Task<bool> HashExistsAsync(string key, string hashField)
        {
            return Task.FromResult(false);
        }

        public bool HashExists(string key, string hashField)
        {
            return false;
        }

        public bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            return false;
        }

        public T HashGet<T>(string key, string field)
        {
            return default;
        }

        public Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            return Task.FromResult(false);
        }

        public Task<T> HashGetAsync<T>(string key, string field)
        {
            return Task.FromResult(default(T));
        }

        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            return default;
        }

        public Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            return Task.FromResult((long)0);
        }
    }
}
