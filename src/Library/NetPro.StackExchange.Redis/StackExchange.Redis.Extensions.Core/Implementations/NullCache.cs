using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.StackExchange.Redis.StackExchange.Redis.Extensions.Core.Implementations
{
    public class NullCache : IRedisDatabase
    {
        public NullCache()
        {

        }

        public IDatabase Database => throw new PlatformNotSupportedException();

        public ISerializer Serializer => throw new PlatformNotSupportedException();

        public bool Exists(string key, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> ExistsAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public T Get<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public T Get<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return default;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys, TimeSpan expiresIn)
        {
            return default;
        }

        public Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            return null;
        }

        public Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, TimeSpan expiresIn)
        {
            return null;
        }

        public Task<T> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public Task<T> GetAsync<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public object GetByLuaScript(string script, object obj)
        {
            return default;
        }

        public bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null)
        {
            return false;
        }

        public bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds)
        {
            return false;
        }

        public Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null)
        {
            return Task.FromResult(false);
        }

        public Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds)
        {
            return Task.FromResult(false);
        }

        public Task<Dictionary<string, string>> GetInfoAsync()
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        public Task<List<InfoDetail>> GetInfoCategorizedAsync()
        {
            return Task.FromResult(new List<InfoDetail>());
        }

        public T GetOrSet<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            if (func == null)
                return default(T);
            var executeResult = func.Invoke();
            return executeResult;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null, int localExpiredTime = 0)
        {
            if (func == null)
                return default(T);
            var executeResult = await func.Invoke();
            return executeResult;
        }

        public bool HashDelete(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public long HashDelete(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<long> HashDeleteAsync(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public bool HashExists(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> HashExistsAsync(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public T HashGet<T>(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Dictionary<string, T> HashGet<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Dictionary<string, T> HashGetAll<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<T> HashGetAsync<T>(string key, string hashField, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<Dictionary<string, T>> HashGetAsync<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public long HashIncerement(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public double HashIncerement(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<long> HashIncerementByAsync(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public Task<double> HashIncerementByAsync(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((double)0);
        }

        public IEnumerable<string> HashKeys(string key, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public Task<IEnumerable<string>> HashKeysAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public long HashLength(string key, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<long> HashLengthAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public Dictionary<string, T> HashScan<T>(string key, string pattern, int pageSize = 10, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public bool HashSet<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public void HashSet<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None)
        {

        }

        public Task<bool> HashSetAsync<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task HashSetAsync<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public IEnumerable<T> HashValues<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public Task<IEnumerable<T>> HashValuesAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return null;
        }

        public bool KeyExpire(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> KeyExpireAsync(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None)
        {
            throw null;
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            throw null;
        }

        public long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None) where T : class
        {
            return 0;
        }

        public long ListAddToLeft<T>(string key, T[] items, CommandFlags flag = CommandFlags.None) where T : class
        {
            return 0;
        }

        public Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flag = CommandFlags.None) where T : class
        {
            return Task.FromResult((long)0);
        }

        public Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flag = CommandFlags.None) where T : class
        {
            return Task.FromResult((long)0);
        }

        public T ListGetFromRight<T>(string key, CommandFlags flag = CommandFlags.None) where T : class
        {
            return default;
        }

        public Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flag = CommandFlags.None) where T : class
        {
            return null;
        }

        public long Publish<T>(RedisChannel channel, T message, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public bool Remove(string key, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public long RemoveAll(IEnumerable<string> keys, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public Task<bool> RemoveAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task SaveAsync(SaveType saveType, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((long)0);
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            return default;
        }

        public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            return default;
        }

        public bool Set<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public bool SetAdd<T>(string key, T item, CommandFlags flag = CommandFlags.None) where T : class
        {
            return false;
        }

        public long SetAddAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items) where T : class
        {
            return 0;
        }

        public Task<long> SetAddAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items) where T : class
        {
            return Task.FromResult((long)0);
        }

        public Task<bool> SetAddAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None) where T : class
        {
            return Task.FromResult(false);
        }

        public bool SetAll<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public bool SetAll<T>(IList<Tuple<string, T>> items, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<bool> SetAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public string[] SetMember(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<string[]> SetMemberAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public IEnumerable<T> SetMembers<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<IEnumerable<T>> SetMembersAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public T SetPop<T>(string key, CommandFlags flag = CommandFlags.None) where T : class
        {
            return default;
        }

        public IEnumerable<T> SetPop<T>(string key, long count, CommandFlags flag = CommandFlags.None) where T : class
        {
            return default;
        }

        public Task<T> SetPopAsync<T>(string key, CommandFlags flag = CommandFlags.None) where T : class
        {
            return default;
        }

        public Task<IEnumerable<T>> SetPopAsync<T>(string key, long count, CommandFlags flag = CommandFlags.None) where T : class
        {
            return default;
        }

        public bool SetRemove<T>(string key, T item, CommandFlags flag = CommandFlags.None) where T : class
        {
            return false;
        }

        public long SetRemoveAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items) where T : class
        {
            return 0;
        }

        public Task<long> SetRemoveAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items) where T : class
        {
            return Task.FromResult((long)0);
        }

        public Task<bool> SetRemoveAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None) where T : class
        {
            return Task.FromResult(false);
        }

        public bool SortedSetAdd<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public double SortedSetAddIncrement<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None)
        {
            return 0;
        }

        public Task<double> SortedSetAddIncrementAsync<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult((double)0);
        }

        public IEnumerable<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return default;
        }

        public Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return default;
        }

        public IEnumerable<T> SortedSetRangeByScore<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public bool SortedSetRemove<T>(string key, T value, CommandFlags flag = CommandFlags.None)
        {
            return false;
        }

        public Task<bool> SortedSetRemoveAsync<T>(string key, T value, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            return default;
        }

        public Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            return Task.FromResult((long)0);
        }

        public long StringIncrement(string key, long value = 1)
        {
            return 0;
        }

        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            return 0;
        }

        public Task<long> StringIncrementAsync(string key, long value = 1)
        {
            return Task.FromResult((long)0);
        }

        public Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            return Task.FromResult((long)0);
        }

        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {

        }

        public Task SubscribeAsync<T>(string channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None)
        {
            return Task.CompletedTask;
        }

        public void Unsubscribe<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None)
        {

        }

        public void UnsubscribeAll(CommandFlags flag = CommandFlags.None)
        {

        }

        public Task UnsubscribeAllAsync(CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            return default;
        }

        public Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }

        public Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            return Task.FromResult(false);
        }
    }
}
