using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
    //[Obsolete]
    internal partial class StackExchangeRedisManager : IRedisManager
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly RedisCacheOption _option;
        private readonly IDatabase _database;
        private IMemoryCache _memorycache;
        private ILogger _logger;

        /// <summary>
        /// 自定义Key的前缀
        /// </summary>
        private string CustomPrefixKey { get; set; }
        public StackExchangeRedisManager(ConnectionMultiplexer connection,
              ISerializer serializer,
            RedisCacheOption option,
             IMemoryCache memorycache,
             ILogger<StackExchangeRedisManager> logger)
        {
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _connection = connection;
            _option = option;
            CustomPrefixKey = option.DefaultCustomKey;
            if (!string.IsNullOrWhiteSpace(CustomPrefixKey))
                _database = _connection.GetDatabase(_option.Database).WithKeyPrefix(CustomPrefixKey);
            else
                _database = _connection.GetDatabase(_option.Database);
            _memorycache = memorycache;
            _logger = logger;
        }
        public ISerializer Serializer { get; }
        public IDatabase Database
        {
            get
            {
                var db = _connection.GetDatabase(_option.Database);

                if (!string.IsNullOrWhiteSpace(CustomPrefixKey))
                    return db.WithKeyPrefix(CustomPrefixKey);

                return db;
            }
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait)
        {
            resource = CustomPrefixKey + resource;
            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

            if (isAwait)
                //只有expiryTime参数，锁未释放会直接跳过
                using (var redLock = GetDistributedLock().CreateLock(resource, TimeSpan.FromSeconds(timeoutSeconds), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1)))
                {
                    if (redLock.IsAcquired)
                    {
                        var result = func();
                        return result;
                    }

                    _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
                    return default(T);
                }
            else
                using (var redLock = GetDistributedLock().CreateLock(resource, TimeSpan.FromSeconds(timeoutSeconds)))
                {
                    if (redLock.IsAcquired)
                    {
                        var result = func();
                        return result;
                    }

                    _logger.LogWarning($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
                    return default(T);
                }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public long Publish(string channel, string input)
        {
            ISubscriber sub = _connection.GetSubscriber();

            return sub.Publish(channel, input);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="input">发布的消息</param>
        /// <returns></returns>
        public async Task<long> PublishAsync(string channel, string input)
        {
            ISubscriber sub = _connection.GetSubscriber();

            return await sub.PublishAsync(channel, input);
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

        public T Get<T>(string key)
        {
            var valueBytes = Database.StringGet(key);

            if (!valueBytes.HasValue)
                return default;

            return Serializer.Deserialize<T>(valueBytes);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var valueBytes = await Database.StringGetAsync(key).ConfigureAwait(false);

            if (!valueBytes.HasValue)
                return default;

            return Serializer.Deserialize<T>(valueBytes);
        }

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

        public bool Set(string key, object data, TimeSpan? expiredTime = null)
        {
            var entryBytes = Common.Serialize(data);

            return Database.StringSet(key, entryBytes, expiredTime);
        }

        public async Task<bool> SetAsync(string key, object data, TimeSpan? expiredTime = null)
        {
            var entryBytes = Common.Serialize(data);

            return await Database.StringSetAsync(key, entryBytes, expiredTime);
        }

        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await Database.KeyExistsAsync(key);
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await Database.KeyDeleteAsync(key);
        }

        public bool Remove(string key)
        {
            return Database.KeyDelete(key);
        }

        public long Remove(string[] keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDelete(redisKeys);
        }

        public async Task<long> RemoveAsync(string[] keys)
        {
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = Database.KeyDelete(redisKeys);
            if (result > 0)
                return await Task.FromResult((long)keys.Count());
            return await Task.FromResult((long)0);
        }

        public bool SortedSetAdd<T>(string key, T value, decimal score)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.SortedSetAdd(key, entryBytes, (double)score);
        }

        public async Task<bool> SortedSetAddAsync<T>(string key, T value, decimal score)
        {
            var entryBytes = Serializer.Serialize(value);

            return await Database.SortedSetAddAsync(key, entryBytes, (double)score);
        }

        public List<T> SortedSetRangeByRank<T>(string key, long start = 0, long stop = -1)
        {
            var items = Database.SortedSetRangeByRank(key, start, stop);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item))?.ToList();
        }

        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = -1)
        {
            var items = await Database.SortedSetRangeByRankAsync(key, start, stop);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item))?.ToList();
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
        {
            var dic = new Dictionary<string, T>();
            var result = await Database.HashGetAllAsync(key);
            foreach (var item in result)
            {
                dic.Add(item.Name, Serializer.Deserialize<T>(item.Value));
            }

            return dic;
        }

        /// <summary>
        ///  获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            var dic = new Dictionary<string, T>();
            var result = Database.HashGetAll(key);
            foreach (var item in result)
            {
                dic.Add(item.Name, Serializer.Deserialize<T>(item.Value));
            }

            return dic;
        }

        public long HashDelete(string key, IEnumerable<string> field)
        {
            return Database.HashDelete(key, field.Select(x => (RedisValue)x).ToArray());
        }

        public long HashDelete(string key, string[] field)
        {
            return Database.HashDelete(key, field.Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> HashDeleteAsync(string key, IEnumerable<string> field)
        {
            return await Database.HashDeleteAsync(key, field.Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> HashDeleteAsync(string key, string[] field)
        {
            return await Database.HashDeleteAsync(key, field.Select(x => (RedisValue)x).ToArray());
        }

        public async Task<bool> HashExistsAsync(string key, string hashField)
        {
            return await Database.HashExistsAsync(key, hashField);
        }

        public bool HashExists(string key, string hashField)
        {
            return Database.HashExists(key, hashField);
        }

        public bool HashSet<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            return Database.HashSet(key, field, Serializer.Serialize(value));
        }

        public T HashGet<T>(string key, string field)
        {
            var redisValue = Database.HashGet(key, field);

            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public async Task<bool> HashSetAsync<T>(string key, string field, T value, TimeSpan? expiredTime = null)
        {
            return await Database.HashSetAsync(key, field, Serializer.Serialize(value));
        }

        public async Task<T> HashGetAsync<T>(string key, string field)
        {
            var redisValue = await Database.HashGetAsync(key, field).ConfigureAwait(false);

            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public object GetByLuaScript(string script, object obj)
        {
            var prepared = LuaScript.Prepare(script);
            return Database.ScriptEvaluate(prepared, obj);
        }

        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public long StringIncrement(string key, long value = 1, TimeSpan? expiry = null)
        {
            var result = Database.StringIncrement(key, value);
            if (expiry.HasValue)
                Database.KeyExpire(key, expiry);
            return result;
        }

        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
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

        public async Task<long> KeyTimeToLiveAsync(string key)
        {
            var time = await Database.KeyTimeToLiveAsync(key);
            if (time.HasValue)
                return Convert.ToInt64(time.Value.TotalSeconds);
            return 0;
        }

        public long KeyTimeToLive(string key)
        {
            var time = Database.KeyTimeToLive(key);
            if (time.HasValue)
                return Convert.ToInt64(time.Value.TotalSeconds);
            return 0;
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan expiry)
        {
            var issucceed = await Database.KeyExpireAsync(key, expiry);
            return issucceed;
        }

        public bool KeyExpire(string key, TimeSpan expiry)
        {
            var issucceed = Database.KeyExpire(key, expiry);
            return issucceed;
        }
    }

    internal partial class StackExchangeRedisManager
    {
        /// <summary>
        /// Gets the server list.
        /// </summary>
        /// <returns>The server list.</returns>
        public IEnumerable<IServer> GetServerList()
        {
            var endpoints = GetMastersServersEndpoints();

            foreach (var endpoint in endpoints)
            {
                yield return _connection.GetServer(endpoint);
            }
        }

        /// <summary>
        /// Gets the masters servers endpoints.
        /// </summary>
        private List<EndPoint> GetMastersServersEndpoints()
        {
            var masters = new List<EndPoint>();
            foreach (var ep in _connection.GetEndPoints())
            {
                var server = _connection.GetServer(ep);
                if (server.IsConnected)
                {
                    //Cluster
                    if (server.ServerType == ServerType.Cluster)
                    {
                        masters.AddRange(server.ClusterConfiguration.Nodes.Where(n => !n.IsSlave).Select(n => n.EndPoint));
                        break;
                    }
                    // Single , Master-Slave
                    if (server.ServerType == ServerType.Standalone && !server.IsSlave)
                    {
                        masters.Add(ep);
                        break;
                    }
                }
            }
            return masters;
        }

        /// <summary>
        /// 获取分布式锁对象
        /// </summary>
        /// <returns></returns>
        private RedLockFactory GetDistributedLock()
        {
            var multiplexers = new List<RedLockMultiplexer> { _connection };
            var redlockFactory = RedLockFactory.Create(multiplexers);
            return redlockFactory;
        }
    }
}
