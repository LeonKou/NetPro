using CSRedis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.CsRedis
{
    public class NullCache : IRedisManager
    {
        public NullCache()
        {
            Console.WriteLine("redis drive is null");
        }
        public T Get<T>(string key, string dbKey = default)
        {
            return default(T);
        }



        public Task<T> GetAsync<T>(string key, string dbKey = default)
        {
            return Task.FromResult(default(T));
        }

        public T GetOrSet<T>(string key, Func<T> func = null, int expiredTime = -1, int localExpiredTime = 0, string dbKey = default)
        {
            if (func == null)
                return default(T);
            var executeResult = func.Invoke();
            return executeResult;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, int expiredTime = -1, int localExpiredTime = 0, string dbKey = default)
        {
            if (func == null)
                return default(T);
            var executeResult = await func.Invoke();
            return executeResult;
        }

        public object GetByLuaScript(string script, object obj, string dbKey = default)
        {
            return default;
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait, string dbKey = default)
        {
            return default(T);
        }

        public T HGet<T>(string key, string field, string dbKey = default)
        {
            return default;
        }

        public Task<T> HGetAsync<T>(string key, string field, string dbKey = default)
        {
            return default;
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1, string dbKey = default)
        {
            return false;
        }

        public Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public bool Exists(string key, string dbKey = default)
        {
            return false;
        }

        public Task<bool> ExistsAsync(string key, string dbKey = default)
        {
            return default;
        }

        public long Remove(string key, string dbKey = default)
        {
            return 0;
        }

        public long Remove(string[] keys, string dbKey = default)
        {
            return 0;
        }

        public bool Set(string key, object data, int cacheTime = -1, string dbKey = default)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, int cacheTime = -1, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public long SortedSetAdd<T>(string key, T obj, decimal score, string dbKey = default)
        {
            return 0;
        }
        public Task<long> SortedSetAddAsync<T>(string key, T obj, decimal score, string dbKey = default)
        {
            return default;
        }
        public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, string dbKey = default)
        {
            return null;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public long Publish(string channel, string input, string dbKey = default)
        {
            return 0;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public Task<long> PublishAsync(string channel, string input, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }


        public Task<long> RemoveAsync(string key, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }

        public Task<long> RemoveAsync(string[] keys, string dbKey = default)
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
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
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
        public Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            return default;
        }

        public void SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage, string dbKey = default)
        {
        }

        public bool Set(string key, object data, TimeSpan expiredTime, string dbKey = default)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, TimeSpan expiredTime, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HSetAsync<T>(string key, string field, T value, TimeSpan expiration, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, string dbKey = default)
        {
            return null;
        }

        public Task<long> KeyTimeToLiveAsync(string key, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }

        public long KeyTimeToLive(string key, string dbKey = default)
        {
            return 0;
        }

        public Task<bool> KeyExpireAsync(string key, TimeSpan expiry, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public bool KeyExpire(string key, TimeSpan expiration, string dbKey = default)
        {
            return false;
        }

        public T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default)
        {
            if (func == null)
                return default(T);
            var executeResult = func.Invoke();
            return executeResult;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0, string dbKey = default)
        {
            if (func == null)
                return default(T);
            var executeResult = await func.Invoke();
            return executeResult;
        }

        public bool Set(string key, object data, TimeSpan? expiredTime, string dbKey = default)
        {
            return false;
        }

        public Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public void Subscribe(string dbKey = default, params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {

        }

        public long HashDelete(string key, IEnumerable<string> field, string dbKey = default)
        {
            return 0;
        }

        public long HashDelete(string key, string[] field, string dbKey = default)
        {
            return 0;
        }

        public Task<long> HashDeleteAsync(string key, IEnumerable<string> field, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }

        public Task<long> HashDeleteAsync(string key, string[] field, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }

        public Task<bool> HashExistsAsync(string key, string hashField, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public bool HashExists(string key, string hashField, string dbKey = default)
        {
            return false;
        }

        public bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default)
        {
            return false;
        }

        public T HashGet<T>(string key, string field, string dbKey = default)
        {
            return default;
        }

        public Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null, string dbKey = default)
        {
            return Task.FromResult(false);
        }

        public Task<T> HashGetAsync<T>(string key, string field, string dbKey = default)
        {
            return Task.FromResult(default(T));
        }

        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            return default;
        }

        public Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null, string dbKey = default)
        {
            return Task.FromResult((long)0);
        }

        public Dictionary<string, T> HashGetAll<T>(string key, string dbKey = default)
        {
            return default;
        }

        public Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, string dbKey = default)
        {
            return default;
        }

        public long HashIncrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            return default;
        }

        public Task<long> HashIncrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            return default;
        }

        public long HashDecrement(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            return default;
        }

        public Task<long> HashDecrementAsync(string key, string field, long value = 1, TimeSpan? expiry = null, string dbKey = null)
        {
            return default;
        }
    }
}
