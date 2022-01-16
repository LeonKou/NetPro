using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Extensions;
using StackExchange.Redis.Extensions.Core.Models;
using StackExchange.Redis.Extensions.Core.ServerIteration;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Implementations
{
    public partial class RedisDatabase : IRedisDatabase
    {
        private readonly ILogger _logger;
        private readonly IRedisCacheConnectionPoolManager connectionPoolManager;
        private readonly ServerEnumerationStrategy serverEnumerationStrategy = new ServerEnumerationStrategy();
        private readonly string keyPrefix;
        private readonly uint maxValueLength;
        private readonly int dbNumber;
        private IMemoryCache _memorycache;

        /// <summary>
        /// 对象的新实例初始化 <see cref="RedisDatabase"/> class.
        /// </summary>
        /// <param name="connectionPoolManager">连接池管理器。</param>
        /// <param name="serializer">序列化</param>
        /// <param name="serverEnumerationStrategy">服务器枚举策略</param>
        /// <param name="dbNumber">要使用的数据库序号.</param>
        /// <param name="maxvalueLength">缓存对象的最大长度</param>
        /// <param name="keyPrefix">key前缀</param>
        public RedisDatabase(
                IRedisCacheConnectionPoolManager connectionPoolManager,
                ISerializer serializer,
                ServerEnumerationStrategy serverEnumerationStrategy,
                int dbNumber,
                uint maxvalueLength,
                string keyPrefix = null,
                ILogger logger = null,
                IMemoryCache memorycache = null)
        {
            _logger = logger ?? NullLogger<RedisDatabase>.Instance;
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.serverEnumerationStrategy = serverEnumerationStrategy ?? new ServerEnumerationStrategy();
            this.connectionPoolManager = connectionPoolManager ?? throw new ArgumentNullException(nameof(connectionPoolManager));
            this.dbNumber = dbNumber;
            this.keyPrefix = keyPrefix;
            maxValueLength = maxvalueLength;
            _memorycache = memorycache;
        }

        public IDatabase Database
        {
            get
            {
                var db = connectionPoolManager.GetConnection().GetDatabase(dbNumber);

                if (!string.IsNullOrWhiteSpace(keyPrefix))
                    return db.WithKeyPrefix(keyPrefix);

                return db;
            }
        }

        public RedLockFactory RedLockFactory
        {
            get
            {
                var multiplexers = new List<RedLockMultiplexer>();
                multiplexers.Add(new RedLockMultiplexer(connectionPoolManager.GetConnection()));
                var redlockFactory = RedLockFactory.Create(multiplexers);
                return redlockFactory;
            }
        }

        public ISerializer Serializer { get; }

        public Task<bool> ExistsAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyExistsAsync(key, flags);
        }

        public TimeSpan? KeyTimeToLive(string key, CommandFlags flags = CommandFlags.None)
        {
            var time = Database.KeyTimeToLive(key, flags); return time;
        }

        public async Task<TimeSpan?> KeyTimeToLiveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            var time = await Database.KeyTimeToLiveAsync(key, flags); return time;
        }

        public bool KeyExpire(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            var issucceed = Database.KeyExpire(key, expiry, flags);
            return issucceed;
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            var issucceed = await Database.KeyExpireAsync(key, expiry, flags);
            return issucceed;
        }

        public bool Exists(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyExists(key, flags);
        }

        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyDeleteAsync(key, flags);
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None)
        {
            return Database.KeyDelete(key, flags);
        }

        public Task<long> RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDeleteAsync(redisKeys, flags);
        }

        public long RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDelete(redisKeys, flags);
        }

        #region
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

        private T _<T>(string key, Func<T> func = null, TimeSpan? expiredTime = null)
        {
            var rValue = Database.StringGet(key);
            if (!rValue.HasValue)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);
                var entryBytes = Serializer.Serialize(executeResult);
                if (!expiredTime.HasValue)
                    Database.StringSetAsync(key, entryBytes);
                else Database.StringSetAsync(key, entryBytes, expiredTime.Value);

                return executeResult;
            }
            var result = Serializer.Deserialize<T>(rValue);
            return result;
        }

        private async Task<T> _Async<T>(string key, Func<Task<T>> func = null, TimeSpan? expiredTime = null)
        {
            var rValue = await Database.StringGetAsync(key);
            if (!rValue.HasValue)
            {
                if (func == null) return default(T);
                var executeResult = await func.Invoke();
                if (executeResult == null) return default(T);
                var entryBytes = Serializer.Serialize(executeResult);
                if (!expiredTime.HasValue)
                    await Database.StringSetAsync(key, entryBytes);
                else await Database.StringSetAsync(key, entryBytes, expiredTime.Value);

                return executeResult;
            }
            var result = Serializer.Deserialize<T>(rValue);
            return result;
        }
        #endregion

        public async Task<T> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            var valueBytes = await Database.StringGetAsync(key, flag).ConfigureAwait(false);

            if (!valueBytes.HasValue)
                return default;

            return Serializer.Deserialize<T>(valueBytes);
        }

        public T Get<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            var valueBytes = Database.StringGet(key, flag);

            if (!valueBytes.HasValue)
                return default;

            return Serializer.Deserialize<T>(valueBytes);
        }

        public async Task<T> GetAsync<T>(string key, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None)
        {
            var result = await GetAsync<T>(key, flag).ConfigureAwait(false);

            if (!Equals(result, default(T)))
                await Database.KeyExpireAsync(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow)).ConfigureAwait(false);

            return result;
        }

        public T Get<T>(string key, DateTimeOffset expiresAt, CommandFlags flag = CommandFlags.None)
        {
            var result = Get<T>(key, flag);

            if (!Equals(result, default(T)))
                Database.KeyExpire(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow));

            return result;
        }

        public async Task<T> GetAsync<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            var result = await GetAsync<T>(key, flag).ConfigureAwait(false);

            if (!Equals(result, default(T)))
                await Database.KeyExpireAsync(key, expiresIn).ConfigureAwait(false);

            return result;
        }

        public T Get<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None)
        {
            var result = Get<T>(key, flag);

            if (!Equals(result, default(T)))
                Database.KeyExpire(key, expiresIn);

            return result;
        }

        public Task<bool> SetAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var entryBytes = value.OfValueSize(Serializer, maxValueLength, key);

            return Database.StringSetAsync(key, entryBytes, null, when, flag);
        }

        public bool Set<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var entryBytes = value.OfValueSize(Serializer, maxValueLength, key);

            return Database.StringSet(key, entryBytes, null, when, flag);
        }

        public Task<bool> SetAsync<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var entryBytes = value.OfValueSize(Serializer, maxValueLength, key);

            return Database.StringSetAsync(key, entryBytes, expiresIn, when, flag);
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var entryBytes = value.OfValueSize(Serializer, maxValueLength, key);

            return Database.StringSet(key, entryBytes, expiresIn, when, flag);
        }

        public Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var entryBytes = value.OfValueSize(Serializer, maxValueLength, key);

            var expiration = expiresAt.UtcDateTime.Subtract(DateTime.UtcNow);

            return Database.StringSetAsync(key, entryBytes, expiration, when, flag);
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">递增值,默认增长1</param>
        /// <returns></returns>
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = Database.StringIncrement(key, value);
            if (expiry.HasValue)
                Database.KeyExpire(key, expiry);
            return result;
        }

        /// <summary>
        /// value递增
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">递增值,默认增长1</param>
        /// <returns></returns>
        public async Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = await Database.StringIncrementAsync(key, value);
            if (expiry.HasValue)
                await Database.KeyExpireAsync(key, expiry);
            return result;
        }

        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public long StringDecrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = Database.StringDecrement(key, value);
            if (expiry.HasValue)
                Database.KeyExpire(key, expiry);
            return result;
        }

        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<long> StringDecrementAsync(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = await Database.StringDecrementAsync(key, value);
            if (expiry.HasValue)
                await Database.KeyExpireAsync(key, expiry);
            return result;
        }

        private Task<bool> ReplaceAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return SetAsync(key, value, when, flag);
        }

        private Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return SetAsync(key, value, expiresAt, when, flag);
        }

        private Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            return SetAsync(key, value, expiresIn, when, flag);
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = await Database.StringGetAsync(redisKeys).ConfigureAwait(false);
            var dict = new Dictionary<string, T>(redisKeys.Length, StringComparer.Ordinal);

            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = Database.StringGet(redisKeys);
            var dict = new Dictionary<string, T>(redisKeys.Length, StringComparer.Ordinal);

            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, DateTimeOffset expiresAt)
        {
            var tsk1 = GetAllAsync<T>(keys);
            var tsk2 = UpdateExpiryAllAsync(keys.ToArray(), expiresAt);

            await Task.WhenAll(tsk1, tsk2).ConfigureAwait(false);

            return tsk1.Result;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys, TimeSpan expiresIn)
        {
            var tsk1 = GetAllAsync<T>(keys);
            var tsk2 = UpdateExpiryAllAsync(keys.ToArray(), expiresIn);

            await Task.WhenAll(tsk1, tsk2).ConfigureAwait(false);

            return tsk1.Result;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys, TimeSpan expiresIn)
        {
            var tsk1 = GetAllAsync<T>(keys);
            var tsk2 = UpdateExpiryAllAsync(keys.ToArray(), expiresIn);

            Task.WhenAll(tsk1, tsk2).ConfigureAwait(false).GetAwaiter().GetResult();

            return tsk1.Result;
        }

        public Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            return Database.StringSetAsync(values, when, flag);
        }

        public bool SetAll<T>(IList<Tuple<string, T>> items, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            return Database.StringSet(values, when, flag);
        }

        public async Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            var tasks = new Task[values.Length];
            await Database.StringSetAsync(values, when, flag);

            for (var i = 0; i < values.Length; i++)
                tasks[i] = Database.KeyExpireAsync(values[i].Key, expiresAt.UtcDateTime, flag);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return ((Task<bool>)tasks[0]).Result;
        }

        public bool SetAll<T>(IList<Tuple<string, T>> items, DateTimeOffset expiresAt, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            var tasks = new Task[values.Length];
            Database.StringSet(values, when, flag);

            for (var i = 0; i < values.Length; i++)
                tasks[i] = Database.KeyExpireAsync(values[i].Key, expiresAt.UtcDateTime, flag);

            Task.WhenAll(tasks).ConfigureAwait(false);

            return ((Task<bool>)tasks[0]).Result;
        }


        public async Task<bool> SetAllAsync<T>(IList<Tuple<string, T>> items, TimeSpan expiresOn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            var tasks = new Task[values.Length];
            await Database.StringSetAsync(values, when, flag);

            for (var i = 0; i < values.Length; i++)
                tasks[i] = Database.KeyExpireAsync(values[i].Key, expiresOn, flag);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return ((Task<bool>)tasks[0]).Result;
        }

        public bool SetAll<T>(IList<Tuple<string, T>> items, TimeSpan expiresOn, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            var values = items
                        .OfValueInListSize(Serializer, maxValueLength)
                        .Select(x => new KeyValuePair<RedisKey, RedisValue>(x.Key, x.Value))
                        .ToArray();

            var tasks = new Task[values.Length];
            Database.StringSet(values, when, flag);

            for (var i = 0; i < values.Length; i++)
                tasks[i] = Database.KeyExpireAsync(values[i].Key, expiresOn, flag);

            Task.WhenAll(tasks).ConfigureAwait(false);

            return ((Task<bool>)tasks[0]).Result;
        }


        public Task<bool> SetAddAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetAddAsync(key, serializedObject, flag);
        }

        public bool SetAdd<T>(string key, T item, CommandFlags flag = CommandFlags.None)
          where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetAdd(key, serializedObject, flag);
        }

        public Task<long> SetAddAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database
                .SetAddAsync(
                    key,
                    items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray(),
                    flag);
        }

        public long SetAddAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
           where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database
                .SetAdd(
                    key,
                    items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray(),
                    flag);
        }

        public async Task<T> SetPopAsync<T>(string key, CommandFlags flag = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = await Database.SetPopAsync(key, flag).ConfigureAwait(false);

            if (item == RedisValue.Null)
                return default;

            return Serializer.Deserialize<T>(item);
        }

        public T SetPop<T>(string key, CommandFlags flag = CommandFlags.None)
        where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = Database.SetPop(key, flag);

            if (item == RedisValue.Null)
                return default;

            return Serializer.Deserialize<T>(item);
        }

        public async Task<IEnumerable<T>> SetPopAsync<T>(string key, long count, CommandFlags flag = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var items = await Database.SetPopAsync(key, count, flag).ConfigureAwait(false);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item));
        }

        public IEnumerable<T> SetPop<T>(string key, long count, CommandFlags flag = CommandFlags.None)
          where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var items = Database.SetPop(key, count, flag);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item));
        }

        public Task<bool> SetContainsAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetContainsAsync(key, serializedObject, flag);
        }

        public bool SetContains<T>(string key, T item, CommandFlags flag = CommandFlags.None)
           where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetContains(key, serializedObject, flag);
        }

        public Task<bool> SetRemoveAsync<T>(string key, T item, CommandFlags flag = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetRemoveAsync(key, serializedObject, flag);
        }

        public bool SetRemove<T>(string key, T item, CommandFlags flag = CommandFlags.None)
           where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetRemove(key, serializedObject, flag);
        }

        public Task<long> SetRemoveAllAsync<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetRemoveAsync(key, items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray(), flag);
        }

        public long SetRemoveAll<T>(string key, CommandFlags flag = CommandFlags.None, params T[] items)
          where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetRemove(key, items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray(), flag);
        }

        public async Task<string[]> SetMemberAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            var members = await Database.SetMembersAsync(key, flag).ConfigureAwait(false);
            return members.Select(x => x.ToString()).ToArray();
        }

        public string[] SetMember(string key, CommandFlags flag = CommandFlags.None)
        {
            var members = Database.SetMembers(key, flag);
            return members.Select(x => x.ToString()).ToArray();
        }

        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            var members = await Database.SetMembersAsync(key, flag).ConfigureAwait(false);

            return members.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public IEnumerable<T> SetMembers<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            var members = Database.SetMembers(key, flag);

            return members.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public async Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            pattern = $"{keyPrefix}{pattern}";
            var keys = new HashSet<string>();

            var multiplexer = Database.Multiplexer;
            var servers = ServerIteratorFactory.GetServers(connectionPoolManager.GetConnection(), serverEnumerationStrategy).ToArray();

            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command.");

            foreach (var server in servers)
            {
                long nextCursor = 0;
                do
                {
                    var redisResult = await Database.ExecuteAsync("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000").ConfigureAwait(false);
                    var innerResult = (RedisResult[])redisResult;

                    nextCursor = long.Parse((string)innerResult[0]);

                    var resultLines = ((string[])innerResult[1]).ToArray();
                    keys.UnionWith(resultLines);
                }
                while (nextCursor != 0);
            }

            return !string.IsNullOrEmpty(keyPrefix)
                        ? keys.Select(k => k.Substring(keyPrefix.Length))
                        : keys;
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            pattern = $"{keyPrefix}{pattern}";
            var keys = new HashSet<string>();

            var multiplexer = Database.Multiplexer;
            var servers = ServerIteratorFactory.GetServers(connectionPoolManager.GetConnection(), serverEnumerationStrategy).ToArray();

            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command.");

            foreach (var server in servers)
            {
                long nextCursor = 0;
                do
                {
                    var redisResult = Database.Execute("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000");
                    var innerResult = (RedisResult[])redisResult;

                    nextCursor = long.Parse((string)innerResult[0]);

                    var resultLines = ((string[])innerResult[1]).ToArray();
                    keys.UnionWith(resultLines);
                }
                while (nextCursor != 0);
            }

            return !string.IsNullOrEmpty(keyPrefix)
                        ? keys.Select(k => k.Substring(keyPrefix.Length))
                        : keys;
        }

        /// <summary>
        /// 删除redis所有缓存
        /// </summary>
        /// <remarks>谨慎使用</remarks>
        public Task FlushDbAsync()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            var tasks = new List<Task>(endPoints.Length);

            for (var i = 0; i < endPoints.Length; i++)
            {
                var server = Database.Multiplexer.GetServer(endPoints[i]);

                if (!server.IsReplica)
                    tasks.Add(server.FlushDatabaseAsync(Database.Database));
            }

            return Task.WhenAll(tasks);
        }

        public Task SaveAsync(SaveType saveType, CommandFlags flags = CommandFlags.None)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            var tasks = new Task[endPoints.Length];

            for (var i = 0; i < endPoints.Length; i++)
                tasks[i] = Database.Multiplexer.GetServer(endPoints[i]).SaveAsync(saveType, flags);

            return Task.WhenAll(tasks);
        }

        public async Task<Dictionary<string, string>> GetInfoAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')").ConfigureAwait(false)).ToString();

            return ParseInfo(info);
        }

        public async Task<List<InfoDetail>> GetInfoCategorizedAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')").ConfigureAwait(false)).ToString();

            return ParseCategorizedInfo(info);
        }

        /// <summary>
        /// lua脚本
        /// obj :new {key=key}
        /// </summary>
        /// <param name="script"></param>
        /// <param name="obj"></param>
        public object GetByLuaScript(string script, object obj)
        {
            var prepared = LuaScript.Prepare(script);
            return Database.ScriptEvaluate(prepared, obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="func"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds)
        {
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }
            _logger.LogWarning($"测试日志");
            //只有expiryTime参数，锁未释放会直接跳过
            using (var redLock = RedLockFactory.CreateLock(resource, TimeSpan.FromSeconds(timeoutSeconds)))
            {
                if (redLock.IsAcquired)
                {
                    func();
                    return true;
                }
            }

            _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
            return false;
            //在使用块结束时自动释放锁
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="timeoutSeconds">超时时间;当你想锁定一个资源时（如果锁不可用就立即放弃）</param>
        /// <param name="func"></param>
        /// <param name="waitTime"></param>
        /// <param name="retryTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public bool GetDistributedLock<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null)
        {
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            //只有expiryTime参数，锁未释放会直接跳过
            using (var redLock = RedLockFactory.CreateLock(resource, TimeSpan.FromSeconds(timeoutSeconds), TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(retryTime), cancellationToken))
            {
                if (redLock.IsAcquired)
                {
                    func();
                    return true;
                }
            }

            _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
            return false;
            //在使用块结束时自动释放锁
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="timeoutSeconds">超时时间;当你想锁定一个资源时（如果锁不可用就立即放弃）</param>
        /// <param name="func"></param>
        /// <param name="waitTime"></param>
        /// <param name="retryTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds, int waitTime, int retryTime, CancellationToken? cancellationToken = null)
        {
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            //只有expiryTime参数，锁未释放会直接跳过
            using (var redLock = await RedLockFactory.CreateLockAsync(resource, TimeSpan.FromSeconds(timeoutSeconds), TimeSpan.FromSeconds(waitTime), TimeSpan.FromSeconds(retryTime), cancellationToken))
            {
                if (redLock.IsAcquired)
                {
                    func();
                    return true;
                }
            }

            _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
            return false;
            //在使用块结束时自动释放锁
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="timeoutSeconds">超时时间;当你想锁定一个资源时（如果锁不可用就立即放弃）</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<bool> GetDistributedLockAsync<T>(string resource, Action func, int timeoutSeconds)
        {
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            //只有expiryTime参数，锁未释放会直接跳过
            using (var redLock = await RedLockFactory.CreateLockAsync(resource, TimeSpan.FromSeconds(timeoutSeconds)))
            {
                if (redLock.IsAcquired)
                {
                    func();
                    return true;
                }
            }

            _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
            return false;
            //在使用块结束时自动释放锁
        }

        private Dictionary<string, string> ParseInfo(string info)
        {
            // Call Parse Categorized Info to cut back on duplicated code.
            var data = ParseCategorizedInfo(info);

            // Return a dictionary of the Info Key and Info value
            return data.ToDictionary(x => x.Key, x => x.InfoValue);
        }

        private List<InfoDetail> ParseCategorizedInfo(string info)
        {
            var data = new List<InfoDetail>();
            var category = string.Empty;
            if (!string.IsNullOrWhiteSpace(info))
            {
                var lines = info.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    if (line[0] == '#')
                    {
                        category = line.Replace("#", string.Empty).Trim();
                        continue;
                    }

                    var idx = line.IndexOf(':');
                    if (idx > 0)
                    {
                        var key = line.Substring(0, idx);
                        var infoValue = line.Substring(idx + 1).Trim();

                        data.Add(new InfoDetail { Category = category, Key = key, InfoValue = infoValue });
                    }
                }
            }

            return data;
        }
    }
}
