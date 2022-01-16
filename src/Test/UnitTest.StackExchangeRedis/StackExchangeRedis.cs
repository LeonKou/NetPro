using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.System.Text.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.StackExchangeRedis
{
    [TestClass]
    public class StackExchangeRedis
    {
        IRedisDatabase _redisDatabase;

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            var redisConfig = new RedisConfiguration
            {
                MaxValueLength = 2810,
                PoolSize = 20,
                AbortOnConnectFail = false,
                Database = 0,
                KeyPrefix = "lyl:",
                ConnectTimeout = 100,
                Hosts = new RedisHost[] {
                    new RedisHost { Host = "198.185.15.16", Port = 6379}
                },
            };
            redisConfig.ServerEnumerationStrategy = new ServerEnumerationStrategy()
            {
                Mode = ServerEnumerationStrategy.ModeOptions.All,
                TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
            };
            IRedisCacheConnectionPoolManager poolManager = new RedisCacheConnectionPoolManager(redisConfig);

            _redisDatabase = new RedisDatabase(connectionPoolManager: poolManager, new SystemTextJsonSerializer(), redisConfig.ServerEnumerationStrategy, 0, 0, redisConfig.KeyPrefix);
        }
        [TestMethod]
        public void SetAndExists()
        {
            string key = "6";
            var result = _redisDatabase.Set(key, "呼吁", TimeSpan.FromSeconds(60));
            var isExists = _redisDatabase.Exists(key);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Exists()
        {
            string key = "6";
            var isExists = _redisDatabase.Exists(key);
            Assert.AreEqual(true, isExists);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            string key = "6";
            var result = _redisDatabase.Set(key, "呼吁", TimeSpan.FromSeconds(60));
            var value = await _redisDatabase.GetAsync<string>(key);
            Assert.AreEqual("呼吁", value);
        }

        [TestMethod]
        public async Task StringIncrement()
        {
            string key = "StringIncrement";
            var result = _redisDatabase.StringIncrement(key, 2);
            var value = await _redisDatabase.GetAsync<long>(key);
            Assert.AreEqual(result, value);
        }

        [TestMethod]
        public async Task Remove()
        {
            string key = "StringIncrement";
            var stringIncrement = _redisDatabase.StringIncrement(key);
            var result = await _redisDatabase.RemoveAsync(key);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task SearchKeysAsync()
        {
            var result = await _redisDatabase.SearchKeysAsync("lyl*");
            foreach (var item in result)
            {
                var isexist = await _redisDatabase.ExistsAsync(item);
                Assert.AreEqual(true, isexist);
            }
        }

        [TestMethod]
        public void GetOrSet()
        {
            string key = "yu";
            var temp = 666;
            var result = _redisDatabase.GetOrSet<int>(key, () => { return temp; });
            Assert.AreEqual(temp, result);
        }

        [TestMethod]
        public async Task GetOrSetAsync()
        {
            string key = "yu";
            var temp = 666;
            var result = await _redisDatabase.GetOrSetAsync<int>(key, async () => { return await Task.FromResult(temp); });
            Assert.AreEqual(temp, result);
        }

        [TestMethod]
        public async Task SubscribeAsync()
        {
            var result = _redisDatabase.Publish("publish", "这是消息");
            var channel = new RedisChannel(Encoding.UTF8.GetBytes("publish"), RedisChannel.PatternMode.Auto);
            Func<string, Task> action = value =>
            {
                Assert.AreEqual("这2是消息", value);
                Console.WriteLine(value);
                return Task.CompletedTask;
            };
            await _redisDatabase.SubscribeAsync("publish", action);
            _redisDatabase.Subscribe(channel, (c, v) =>
            {
                Console.WriteLine($"{channel} 发布订阅 value  {v}");
            });
        }

        [TestMethod]
        public void Subscribe()
        {
            var result = _redisDatabase.Publish("publish", "这是消息");
            var channel = new RedisChannel(Encoding.UTF8.GetBytes("publish"), RedisChannel.PatternMode.Auto);

            _redisDatabase.Subscribe(channel, (c, v) =>
            {
                Console.WriteLine($"{channel} 发布订阅 value  {v}");
            });
        }

        [TestMethod]
        public void HashSet()
        {
            string key = "HashSet";
            var result = _redisDatabase.HashSet<dynamic>(key, $"{Guid.NewGuid()}", new { Id = 1, Name = "ddd" });
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task HashSetAsync()
        {
            string key = "HashSet";
            var result = await _redisDatabase.HashSetAsync<dynamic>(key, $"{Guid.NewGuid()}", new { Id = 1, Name = "ddd" });
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task HashLength()
        {
            string key = "HashSet";
            var result = _redisDatabase.HashLength(key);
            await _redisDatabase.HashSetAsync<dynamic>(key, $"{Guid.NewGuid()}", new { Id = 1, Name = "ddd" });
            var result2 = _redisDatabase.HashLength(key);
            Assert.AreEqual(result, result2 - 1);
        }

    }
}
