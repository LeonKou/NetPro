using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Implementations
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDeleteAsync(key, hashField, commandFlags);
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

        public bool HashDelete(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(key, hashField, commandFlags);
        }

        public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public Task<bool> HashExistsAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashExistsAsync(key, hashField, commandFlags);
        }

        public bool HashExists(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashExists(key, hashField, commandFlags);
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(hashKey, key, commandFlags).ConfigureAwait(false);

            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(hashKey, key, commandFlags);

            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default;
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var tasks = new Task<T>[keys.Count];

            for (var i = 0; i < keys.Count; i++)
                tasks[i] = HashGetAsync<T>(hashKey, keys[i], commandFlags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = new Dictionary<string, T>();

            for (var i = 0; i < tasks.Length; i++)
                result.Add(keys[i], tasks[i].Result);

            return result;
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var tasks = new Task<T>[keys.Count];

            for (var i = 0; i < keys.Count; i++)
                tasks[i] = HashGetAsync<T>(hashKey, keys[i], commandFlags);

            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter();

            var result = new Dictionary<string, T>();

            for (var i = 0; i < tasks.Length; i++)
                result.Add(keys[i], tasks[i].Result);

            return result;
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashGetAllAsync(hashKey, commandFlags).ConfigureAwait(false))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (Database.HashGetAll(hashKey, commandFlags))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public long HashIncerement(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public double HashIncerement(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashKeysAsync(hashKey, commandFlags).ConfigureAwait(false)).Select(x => x.ToString());
        }

        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (Database.HashKeys(hashKey, commandFlags)).Select(x => x.ToString());
        }

        public Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashLengthAsync(hashKey, commandFlags);
        }

        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashLength(hashKey, commandFlags);
        }

        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashSetAsync(hashKey, key, Serializer.Serialize(value), when, commandFlags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashSet(hashKey, key, Serializer.Serialize(value), when, commandFlags);
        }

        public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));

            return Database.HashSetAsync(hashKey, entries.ToArray(), commandFlags);
        }

        public void HashSet<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));

            Database.HashSet(hashKey, entries.ToArray(), commandFlags);
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashValuesAsync(hashKey, commandFlags).ConfigureAwait(false)).Select(x => Serializer.Deserialize<T>(x));
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return (Database.HashValues(hashKey, commandFlags)).Select(x => Serializer.Deserialize<T>(x));
        }

        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashScan(hashKey, pattern, pageSize, commandFlags).ToDictionary(x => x.Name.ToString(), x => Serializer.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }
    }
}
