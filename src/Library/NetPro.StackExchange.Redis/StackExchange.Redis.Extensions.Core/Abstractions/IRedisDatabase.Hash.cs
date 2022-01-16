using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extensions.Core.Abstractions
{
    /// <summary>
    /// The Redis Database
    /// </summary>
    public partial interface IRedisDatabase
    {
        /// <summary>
        /// 从存储在键处的散列中删除指定字段。
        /// 忽略此散列中不存在的指定字段。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">哈希集合成员的健</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果键被删除，返回true。
        /// 如果键不存在，它将被视为空散列，此命令返回false。
        /// </returns>
        Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从存储在键处的散列中删除指定字段。
        /// 忽略此散列中不存在的指定字段。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">哈希集合成员的健</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>
        /// 如果键被删除，返回true。
        /// 如果键不存在，它将被视为空散列，此命令返回false。
        /// </returns>
        bool HashDelete(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从存储在键处的散列中删除指定字段。
        /// 忽略此散列中不存在的指定字段。
        /// 如果键不存在，它将被视为空散列，返回0。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashFields">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>从散列中删除的条目数，不包括指定但不存在的字段.</returns>
        Task<long> HashDeleteAsync(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 从存储在键处的散列中删除指定字段。
        /// 忽略此散列中不存在的指定字段。
        /// 如果键不存在，它将被视为空散列，返回0。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashFields">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>从散列中删除的条目数，不包括指定但不存在的字段.</returns>
        long HashDelete(string key, IEnumerable<string> hashFields, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 检查 hash条目中的 key是否存在
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<bool> HashExistsAsync(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 检查 hash条目中的 key是否存在
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        bool HashExists(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回与存储在键上的散列中的字段相关联的值。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>the value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        Task<T> HashGetAsync<T>(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回与存储在键上的散列中的字段相关联的值。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>the value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        T HashGet<T>(string key, string hashField, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回与存储在键上的散列中的指定字段相关联的值。
        /// 对于散列中不存在的每个字段，将返回一个nil值。
        /// 因为不存在的键被视为空散列，所以针对不存在的键运行HMGET将返回一个空值列表。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="hashFields">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>与给定字段关联的值的列表，与请求它们的顺序相同。.</returns>
        Task<Dictionary<string, T>> HashGetAsync<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回与存储在键上的散列中的指定字段相关联的值。
        /// 对于散列中不存在的每个字段，将返回一个nil值。
        /// 因为不存在的键被视为空散列，所以针对不存在的键运行HMGET将返回一个空值列表。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="hashFields">要从散列中检索的键</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>与给定字段关联的值的列表，与请求它们的顺序相同。.</returns>
        Dictionary<string, T> HashGet<T>(string key, IList<string> hashFields, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回所有与存储在键上的散列中的指定字段相关联的值。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>与给定字段关联的值的列表，与请求它们的顺序相同。.</returns>
        /// <returns>存储在散列中的字段及其值的列表，或当键不存在时为空列表。</returns>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回所有与存储在键上的散列中的指定字段相关联的值。
        /// </summary>
        /// <typeparam name="T">value返回的类型</typeparam>
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>与给定字段关联的值的列表，与请求它们的顺序相同。.</returns>
        /// <returns>存储在散列中的字段及其值的列表，或当键不存在时为空列表。</returns>
        Dictionary<string, T> HashGetAll<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给hash散列集合中的hashfield 递增，默认递增1
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="value">递增值</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<long> HashIncerementByAsync(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给hash散列集合中的hashfield 递增，默认递增1
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="value">递增值</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        long HashIncerement(string key, string hashField, long value = 1, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给hash散列集合中的hashfield 递增，默认递增1
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="value">递增值</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task<double> HashIncerementByAsync(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给hash散列集合中的hashfield 递增，默认递增1
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="hashField">要从散列中检索的键</param>
        /// <param name="value">递增值</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        double HashIncerement(string key, string hashField, double value = 1, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  返回在键处存储的散列中的所有字段名。
        /// </summary>    
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列集合包含的所有成员key名称.</returns>
        Task<IEnumerable<string>> HashKeysAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  返回在键处存储的散列中的所有字段名。
        /// </summary>    
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列集合包含的所有成员key名称.</returns>
        IEnumerable<string> HashKeys(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回存储在键处的散列中包含的key总数。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列中包含的key总数</returns>
        Task<long> HashLengthAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 返回存储在键处的散列中包含的key总数。
        /// </summary>
        /// <param name="key">redis  key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列中包含的key总数</returns>
        long HashLength(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给散列集合增加一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="hashField">要添加到散列中的key</param>
        /// <param name="value">散列集合hashField对应的value</param>
        /// <param name="when">散列成员添加的条件，默认始终添加</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给散列集合增加一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="hashField">要添加到散列中的key</param>
        /// <param name="value">散列集合hashField对应的value</param>
        /// <param name="when">散列成员添加的条件，默认始终添加</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns></returns>
        bool HashSet<T>(string key, string hashField, T value, When when = When.Always, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给散列集合添加多条记录
        /// </summary>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="values">要添加到hash的集合</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        Task HashSetAsync<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// 给散列集合添加多条记录
        /// </summary>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="values">要添加到hash的集合</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        void HashSet<T>(string key, IDictionary<string, T> values, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  获取散列的所有值集合
        /// </summary>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列的所有值集合</returns>
        Task<IEnumerable<T>> HashValuesAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///  获取散列的所有值集合
        /// </summary>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>散列的所有值集合</returns>
        IEnumerable<T> HashValues<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///   The HSCAN command is used to incrementally iterate over a hash.
        /// </summary>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="key">散列的redis key</param>
        /// <param name="pattern">检索的模式</param>
        /// <param name="pageSize">从redsi检索的元素数量</param>
        /// <param name="flag">行为标记,默认None=PreferMaster:尝试主服务器上执行</param>
        /// <returns>生成匹配该模式的散列的所有元素。</returns>
        Dictionary<string, T> HashScan<T>(string key, string pattern, int pageSize = 10, CommandFlags flag = CommandFlags.None);

    }
}
