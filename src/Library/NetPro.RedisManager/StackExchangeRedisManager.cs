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

        /// <summary>
        /// 自定义Key的前缀
        /// </summary>
        public string CustomPrefixKey { get; set; }
        public StackExchangeRedisManager(ConnectionMultiplexer connection, RedisCacheOption option)
        {
            _connection = connection;
            _option = option;
            CustomPrefixKey = option.DefaultCustomKey;
            _database = _connection.GetDatabase();
           
        }

        public T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class
        {
            NotNullOrWhiteSpace(key, nameof(key));

            Common.CheckKey(key);
            var _db = _connection.GetDatabase();
            var rValue = _db.StringGet(key);
            if (!rValue.HasValue)
            {
                if (func?.Invoke() == null) return default(T);
                var entryBytes = Common.Serialize(func.Invoke());
                if (expiredTime == -1)
                    Do(db => db.StringSet(key, entryBytes));
                else Do(db => db.StringSet(key, entryBytes, TimeSpan.FromSeconds(expiredTime)));

                return func.Invoke();
            }
            var result = Common.Deserialize<T>(rValue);
            return result;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class
        {
            NotNullOrWhiteSpace(key, nameof(key));

            Common.CheckKey(key);
            var _db = _connection.GetDatabase();
            var rValue = await _db.StringGetAsync(key);
            if (!rValue.HasValue)
            {
                if (func?.Invoke() == null) return default(T);
                var entryBytes = Common.Serialize(func.Invoke());
                if (expiredTime == -1)
                    await Do(db => db.StringSetAsync(key, entryBytes));
                else await Do(db => db.StringSetAsync(key, entryBytes, TimeSpan.FromSeconds(expiredTime)));

                return func.Invoke();
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
            var entryBytes = Common.Serialize(data);
            Common.CheckKey(key);
            if (cacheTime == -1)
                return Do(db => db.StringSet(key, entryBytes));
            return Do(db => db.StringSet(key, entryBytes, TimeSpan.FromMinutes(cacheTime)));
        }

        public async Task<bool> SetAsync(string key, object data, int cacheTime)
        {
            var entryBytes = Common.Serialize(data);
            Common.CheckKey(key);
            if (cacheTime == -1)
                return await Do(db => db.StringSetAsync(key, entryBytes));
            return await Do(db => db.StringSetAsync(key, entryBytes, TimeSpan.FromMinutes(cacheTime)));
        }

        public bool IsSet(string key)
        {
            Common.CheckKey(key);
            return Do(db => db.KeyExists(key));
        }

        public bool Remove(string key)
        {
            Common.CheckKey(key);
            return Do(db => db.KeyDelete(key));
        }

        public bool Remove(string[] keys)
        {
            var redisKeys = keys.Select(a => (RedisKey)Common.CheckKey(a)).ToArray();
            return Do(db => db.KeyDelete(redisKeys) > 0);
        }

        public bool ZAdd<T>(string key, T obj, decimal score)
        {
            Common.CheckKey(key);
            return Do(db => db.SortedSetAdd(key, Common.SerializeToString(obj), (double)score));
        }

        public List<T> ZRange<T>(string key)
        {
            Common.CheckKey(key);
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return Common.ConvetList<T>(values);
            });
        }

        public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func)
        {
            Common.CheckKey(resource);

            if (timeoutSeconds <= 0 || string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException($"The timeout is not valid with a distributed lock object--key:{resource}--expiryTime--{timeoutSeconds}");
            }

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
        }

        public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
        {
            Common.CheckKey(key);
            return Do(db => db.HashSet(key, field, Common.Serialize(value), flags: CommandFlags.FireAndForget));
        }

        public T HGet<T>(string key, string field)
        {
            Common.CheckKey(key);
            return Common.Deserialize<T>(Do(db => db.HashGet(key, field)));
        }

        public async Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
        {
            Common.CheckKey(key);
            return await Do(db => db.HashSetAsync(key, field, Common.Serialize(value), flags: CommandFlags.FireAndForget));
        }

        public async Task<T> HGetAsync<T>(string key, string field)
        {
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
