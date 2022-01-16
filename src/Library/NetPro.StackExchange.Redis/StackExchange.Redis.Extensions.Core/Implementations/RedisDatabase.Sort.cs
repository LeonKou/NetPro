using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Implementations
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<bool> SortedSetAddAsync<T>(
                                    string key,
                                    T value,
                                    double score,
                                    CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.SortedSetAddAsync(key, entryBytes, score, commandFlags);
        }

        public bool SortedSetAdd<T>(
                                    string key,
                                    T value,
                                    double score,
                                    CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.SortedSetAdd(key, entryBytes, score, commandFlags);
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员,默认从小到大
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(
                                    string key,
                                     long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            var items = await Database.SortedSetRangeByRankAsync(key, start, stop, order, flags: flags);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item));
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员,默认从小到大
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<T> SortedSetRangeByRank<T>(
                                    string key,
                                     long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            var items = Database.SortedSetRangeByRank(key, start, stop, order, flags: flags);

            return items.Select(item => item == RedisValue.Null ? default : Serializer.Deserialize<T>(item));
        }

        public Task<bool> SortedSetRemoveAsync<T>(
                                    string key,
                                    T value,
                                    CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.SortedSetRemoveAsync(key, entryBytes, commandFlags);
        }

        public bool SortedSetRemove<T>(
                                   string key,
                                   T value,
                                   CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);

            return Database.SortedSetRemove(key, entryBytes, commandFlags);
        }

        public async Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(
                                    string key,
                                    double start = double.NegativeInfinity,
                                    double stop = double.PositiveInfinity,
                                    Exclude exclude = Exclude.None,
                                    Order order = Order.Ascending,
                                    long skip = 0L,
                                    long take = -1L,
                                    CommandFlags commandFlags = CommandFlags.None)
        {
            var result = await Database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, commandFlags).ConfigureAwait(false);

            return result.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public IEnumerable<T> SortedSetRangeByScore<T>(
                                  string key,
                                  double start = double.NegativeInfinity,
                                  double stop = double.PositiveInfinity,
                                  Exclude exclude = Exclude.None,
                                  Order order = Order.Ascending,
                                  long skip = 0L,
                                  long take = -1L,
                                  CommandFlags commandFlags = CommandFlags.None)
        {
            var result = Database.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, commandFlags);

            return result.Select(m => m == RedisValue.Null ? default : Serializer.Deserialize<T>(m));
        }

        public Task<double> SortedSetAddIncrementAsync<T>(string key, T value, double score, CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);
            return Database.SortedSetIncrementAsync(key, entryBytes, score, commandFlags);
        }

        public double SortedSetAddIncrement<T>(string key, T value, double score, CommandFlags commandFlags = CommandFlags.None)
        {
            var entryBytes = Serializer.Serialize(value);
            return Database.SortedSetIncrement(key, entryBytes, score, commandFlags);
        }
    }
}
