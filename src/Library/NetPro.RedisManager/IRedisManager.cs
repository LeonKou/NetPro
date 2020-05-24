using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
	/// <summary>
	/// Cache manager interface
	/// </summary>
	public interface IRedisManager
	{
		/// <summary>
		/// 获取缓存，没有则新增缓存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="func"></param>
		/// <param name="expiredTime"></param>
		/// <returns></returns>
		T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class;

		/// <summary>
		/// 获取缓存没有则新增缓存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="func"></param>
		/// <param name="expiredTime"></param>
		/// <returns></returns>
		Task<T> GetOrCreateAsync<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class;

		/// <summary>
		///新增缓存
		/// </summary>
		/// <param name="key">缓存key值,key值必须满足规则：模块名:类名:业务方法名:参数.不满足规则将不会被缓存</param>
		/// <param name="data">Value for caching</param>
		/// <param name="cacheTime">Cache time in minutes</param>
		bool Set(string key, object data, int? cacheTime=-1);

		Task<bool> SetAsync(string key, object data, int? cacheTime);

		bool IsSet(string key);

		bool Remove(string key);

		bool Remove(string[] keys);

		bool ZAdd<T>(string key, T obj, decimal score);

		List<T> ZRange<T>(string key);

		T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func);

		bool HSet<T>(string key, string field, T value, int expirationMinute = 1);
		T HGet<T>(string key, string field);

		Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1);

		Task<T> HGetAsync<T>(string key, string field);

		/// <summary>
		/// lua脚本
		/// obj :new {key=key}
		/// </summary>
		/// <param name="script"></param>
		/// <param name="obj"></param>
		object GetByLuaScript(string script, object obj);

		/// <summary>
		/// 获得使用原生stackexchange.redis的能力，用于pipeline (stackExchange.redis专用，Csredis驱动使用此方法会报异常)
		/// </summary>
		/// <returns></returns>
		IDatabase GetIDatabase();
	}
}
