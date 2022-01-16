using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Implementations
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            return sub.PublishAsync(channel, Serializer.Serialize(message), flags);
        }

        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            return sub.Publish(channel, Serializer.Serialize(message), flags);
        }

        public Task SubscribeAsync<T>(string channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionPoolManager.GetConnection().GetSubscriber();

            return sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(Serializer.Deserialize<T>(value)).ConfigureAwait(false), flags);
        }

        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionPoolManager.GetConnection().GetSubscriber();

            sub.Subscribe(channel, handler, flags);
        }

        public Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            return sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public void Unsubscribe<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            return sub.UnsubscribeAllAsync(flags);
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = connectionPoolManager.GetConnection().GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        public async Task<bool> UpdateExpiryAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (await Database.KeyExistsAsync(key).ConfigureAwait(false))
                return await Database.KeyExpireAsync(key, expiresAt.UtcDateTime.Subtract(DateTime.UtcNow), flags).ConfigureAwait(false);

            return false;
        }

        public async Task<bool> UpdateExpiryAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (await Database.KeyExistsAsync(key).ConfigureAwait(false))
                return await Database.KeyExpireAsync(key, expiresIn, flags).ConfigureAwait(false);

            return false;
        }

        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresAt.UtcDateTime, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }

        public async Task<IDictionary<string, bool>> UpdateExpiryAllAsync(string[] keys, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            var tasks = new Task<bool>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
                tasks[i] = UpdateExpiryAsync(keys[i], expiresIn, flags);

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var results = new Dictionary<string, bool>(keys.Length, StringComparer.Ordinal);

            for (var i = 0; i < keys.Length; i++)
                results.Add(keys[i], tasks[i].Result);

            return results;
        }
    }
}
