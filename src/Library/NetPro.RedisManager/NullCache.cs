using NetPro.RedisManager;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.RedisManager
{
	internal class NullCache : IRedisManager
	{
		public NullCache()
		{
			Console.WriteLine("redis drive is null");
		}
        public T GetOrCreate<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class
		{
            return default;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<T> func = null, int expiredTime = -1) where T : class
		{
			return default;
		}

		public object GetByLuaScript(string script, object obj)
		{
			return default;
		}

		public T GetDistributedLock<T>(string resource, int timeoutSeconds, Func<T> func)
		{
			return default;
		}

		public IDatabase GetIDatabase()
		{
			return default;
		}

		public T HGet<T>(string key, string field)
		{
			return default;
		}

		public Task<T> HGetAsync<T>(string key, string field)
		{
			return default;
		}

		public bool HSet<T>(string key, string field, T value, int expirationMinute = 1)
		{
			return default;
		}

		public Task<bool> HSetAsync<T>(string key, string field, T value, int expirationMinute = 1)
		{
			return default;
		}

		public bool IsSet(string key)
		{
			return default;
		}

		public bool Remove(string key)
		{
			return default;
		}

		public bool Remove(string[] keys)
		{
			return default;
		}

		public bool Set(string key, object data, int? cacheTime = -1)
		{
			return default;
		}

		public Task<bool> SetAsync(string key, object data, int? cacheTime)
		{
			return default;
		}

		public bool ZAdd<T>(string key, T obj, decimal score)
		{
			return default;
		}

		public List<T> ZRange<T>(string key)
		{
			return default;
		}
	}
}
