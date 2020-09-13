using Microsoft.Extensions.Caching.Memory;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
    internal partial class StackExchangeRedisManager : IRedisManager
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly RedisCacheOption _option;
        private readonly IDatabase _database;
        private IMemoryCache _memorycache;

        /// <summary>
        /// 自定义Key的前缀
        /// </summary>
        private string CustomPrefixKey { get; set; }
        public StackExchangeRedisManager(ConnectionMultiplexer connection,
            RedisCacheOption option,
             IMemoryCache memorycache)
        {
            _connection = connection;
            _option = option;
            CustomPrefixKey = option.DefaultCustomKey;
            _database = _connection.GetDatabase();
            _memorycache = memorycache;
        }
        public T Get<T>(string key)
        {
            key = AddDefaultPrefixKey(key);
            var result = _<T>(key);
            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            key = AddDefaultPrefixKey(key);
            var result = await _Async<T>(key);
            return result;
        }

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
                        s.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiredTime);
                    return resultTemp;
                });
                return memoryResult;
            }
            return _(key, func, expiredTime);
        }

        /// <summary>
        /// 系统自定义Key前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string AddDefaultPrefixKey(string key)
        {
            var build = new StringBuilder(CustomPrefixKey ?? string.Empty);
            return build.Append(key).ToString();
        }

        private T _<T>(string key, Func<T> func = null, int expiredTime = -1)
        {
            NotNullOrWhiteSpace(key, nameof(key));
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            var _db = _connection.GetDatabase();
            var rValue = _db.StringGet(key);
            if (!rValue.HasValue)
            {
                if (func == null) return default(T);
                var executeResult = func.Invoke();
                if (executeResult == null) return default(T);
                var entryBytes = Common.Serialize(executeResult);
                if (expiredTime == -1)
                    Do(db => db.StringSet(key, entryBytes));
                else Do(db => db.StringSet(key, entryBytes, TimeSpan.FromSeconds(expiredTime)));

                return executeResult;
            }
            var result = Common.Deserialize<T>(rValue);
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
            NotNullOrWhiteSpace(key, nameof(key));
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            var _db = _connection.GetDatabase();
            var rValue = await _db.StringGetAsync(key);
            if (!rValue.HasValue)
            {
                if (func == null) return default(T);
                var executeResult = await func.Invoke();
                if (executeResult == null) return default(T);
                var entryBytes = Common.Serialize(executeResult);
                if (expiredTime == -1)
                    await Do(db => db.StringSetAsync(key, entryBytes));
                else await Do(db => db.StringSetAsync(key, entryBytes, TimeSpan.FromSeconds(expiredTime)));

                return executeResult;
            }
            var result = Common.Deserialize<T>(rValue);
            return result;
        }

        /// <summary>
        ///新增缓存
        /// </summary>
        /// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        public bool Set(string key, object data, int cacheTime = -1)
        {
            key = AddDefaultPrefixKey(key);
            var entryBytes = Common.Serialize(data);
            Common.CheckKey(key);
            if (cacheTime == -1)
                return Do(db => db.StringSet(key, entryBytes));
            return Do(db => db.StringSet(key, entryBytes, TimeSpan.FromMinutes(cacheTime)));
        }

        public async Task<bool> SetAsync(string key, object data, int expiredTime = -1)
        {
            key = AddDefaultPrefixKey(key);
            var entryBytes = Common.Serialize(data);
            Common.CheckKey(key);
            if (expiredTime == -1)
                return await Do(db => db.StringSetAsync(key, entryBytes));
            return await Do(db => db.StringSetAsync(key, entryBytes, TimeSpan.FromMinutes(expiredTime)));
        }

        public bool Exists(string key)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Do(db => db.KeyExists(key));
        }

        public bool Remove(string key)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Do(db => db.KeyDelete(key));
        }

        public bool Remove(string[] keys)
        {
            var redisKeys = keys.Select(a => (RedisKey)AddDefaultPrefixKey(Common.CheckKey(a))).ToArray();
            return Do(db => db.KeyDelete(redisKeys) > 0);
        }

        public bool ZAdd<T>(string key, T obj, decimal score)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Do(db => db.SortedSetAdd(key, Common.SerializeToString(obj), (double)score));
        }

        public List<T> ZRange<T>(string key)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return Common.ConvetList<T>(values);
            });
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func, bool isAwait)
        {
            //resource = AddDefaultPrefixKey(resource);
            Common.CheckKey(resource);

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

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
                    Console.ResetColor();
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

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"当前线程：{Thread.CurrentThread.ManagedThreadId}--未拿到锁!!");
                    Console.ResetColor();
                    return default(T);
                }
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Do(db => db.HashSet(key, field, Common.Serialize(value), flags: CommandFlags.FireAndForget));
        }

        public T HGet<T>(string key, string field)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return Common.Deserialize<T>(Do(db => db.HashGet(key, field)));
        }

        public async Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            return await Do(db => db.HashSetAsync(key, field, Common.Serialize(value), flags: CommandFlags.FireAndForget));
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
            key = AddDefaultPrefixKey(key);
            Common.CheckKey(key);
            var redisvalue = await _database.HashGetAsync(key, field);
            return Common.ConvertObj<T>(redisvalue);
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
            return _database.ScriptEvaluate(prepared, obj);
        }

        /// <summary>
        /// 完全授予自定义能力，用于pipeline
        /// </summary>
        /// <returns></returns>
        public IDatabase GetIDatabase()
        {
            return _database;
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
    }

    internal partial class StackExchangeRedisManager
    {
        /// <summary>
        /// 获取IDatabase处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _connection.GetDatabase();
            return func(database);
        }

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

        private void NotNullOrWhiteSpace(string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(argumentName);
            }
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
