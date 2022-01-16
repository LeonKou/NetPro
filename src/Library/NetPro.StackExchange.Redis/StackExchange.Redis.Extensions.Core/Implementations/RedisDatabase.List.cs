using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Implementations
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPushAsync(key, serializedItem, when, flags);
        }

        public long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
          where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPush(key, serializedItem, when, flags);
        }

        public Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "item cannot be null.");

            var serializedItems = items.Select(x => (RedisValue)Serializer.Serialize(x)).ToArray();

            return Database.ListLeftPushAsync(key, serializedItems, flags);
        }

        public long ListAddToLeft<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
           where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "item cannot be null.");

            var serializedItems = items.Select(x => (RedisValue)Serializer.Serialize(x)).ToArray();

            return Database.ListLeftPush(key, serializedItems, flags);
        }

        public async Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = await Database.ListRightPopAsync(key, flags).ConfigureAwait(false);

            if (item == RedisValue.Null)
                return null;

            return item == RedisValue.Null
                                    ? null
                                    : Serializer.Deserialize<T>(item);
        }

        public T ListGetFromRight<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = Database.ListRightPop(key, flags);

            if (item == RedisValue.Null)
                return null;

            return item == RedisValue.Null
                                    ? null
                                    : Serializer.Deserialize<T>(item);
        }
    }
}
