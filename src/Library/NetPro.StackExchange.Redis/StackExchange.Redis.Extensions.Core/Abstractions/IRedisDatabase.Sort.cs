using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public partial interface IRedisDatabase
    {
        /// <summary>
        /// 将条目添加到带有排序的有序集
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="value">待添加到有序集中的一项.</param>
        /// <param name="score">带添加的成员所属序号</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果已添加对象，则为true。否则false
        /// </returns>
        Task<bool> SortedSetAddAsync<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 将条目添加到带有排序的有序集
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="value">待添加到有序集中的一项.</param>
        /// <param name="score">带添加的成员所属序号</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果已添加对象，则为true。否则false
        /// </returns>
        bool SortedSetAdd<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None);

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
        Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(
                                    string key,
                                     long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

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
        IEnumerable<T> SortedSetRangeByRank<T>(
                                    string key,
                                     long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 删除有序集的条目
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="value">有序集中待删除的一项.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        ///  如果移除成功返回true，否则返回false
        /// </returns>
        Task<bool> SortedSetRemoveAsync<T>(string key, T value, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 删除有序集的条目
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="value">有序集中待删除的一项.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        ///  如果移除成功返回true，否则返回false
        /// </returns>
        bool SortedSetRemove<T>(string key, T value, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从有序集获取条目
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="start">最小 score</param>
        /// <param name="stop">最大 score</param>
        /// <param name="exclude">包含 start / stop</param>
        /// <param name="order">根据score排序的规则,默认从小到大</param>
        /// <param name="skip">跳过的数量</param>
        /// <param name="take">期望返回的条目数量</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0L, long take = -1L, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从有序集获取条目
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="start">最小 score</param>
        /// <param name="stop">最大 score</param>
        /// <param name="exclude">包含 start / stop</param>
        /// <param name="order">根据score排序的规则,默认从小到大</param>
        /// <param name="skip">跳过的数量</param>
        /// <param name="take">期望返回的条目数量</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        IEnumerable<T> SortedSetRangeByScore<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0L, long take = -1L, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 用score递增的方式添加有序集合
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="score">score</param>
        /// <param name="value">有序集中待删除的一项.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果对象已经被添加，返回先前的score 
        /// </returns>
        Task<double> SortedSetAddIncrementAsync<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 用score递增的方式添加有序集合
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="key">redis key</param>
        /// <param name="score">score</param>
        /// <param name="value">有序集中待删除的一项.</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果对象已经被添加，返回先前的score 
        /// </returns>
        double SortedSetAddIncrement<T>(string key, T value, double score, CommandFlags flag = CommandFlags.None);
    }
}
