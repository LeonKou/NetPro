using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;
using NetPro.RedisManager;
using NetPro.TypeFinder;
using StackExchange.Redis;

namespace NetPro.Web.Core.Infrastructure
{
    /// <summary>
    /// 限流中间件
    /// </summary>
    public class NetProRateLimitStartup : INetProStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
        {

            var isOpen = configuration.GetValue<bool>("EnableAspNetCoreRateLimit", false);
            if (isOpen)
            {
                //需要存储速率和ip规则
                services.AddMemoryCache();
                //加载appsettings.json中的配置项 ，下面三项是加载general,rules
                services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
                services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

                //load general configuration from appsettings.json
                services.Configure<ClientRateLimitOptions>(configuration.GetSection("ClientRateLimiting"));
                //load client rules from appsettings.json
                services.Configure<ClientRateLimitPolicies>(configuration.GetSection("ClientRateLimitPolicies"));

                services.AddStackExchangeRedisCache(options =>
                {
                    //options.ConfigurationOptions = new ConfigurationOptions
                    //{
                    //    //silently retry in the background if the Redis connection is temporarily down
                    //    AbortOnConnectFail = false
                    //};
                    RedisCacheOption redisOptions = new RedisCacheOption(configuration);
                    var configurationOptions = new ConfigurationOptions
                    {
                        ConnectTimeout = redisOptions.ConnectionTimeout,
                        Password = redisOptions.Password,
                        Ssl = redisOptions.IsSsl,
                        SslHost = redisOptions.SslHost,
                    };
                    foreach (var endpoint in redisOptions.Endpoints)
                    {
                        configurationOptions.EndPoints.Add(endpoint.Host, endpoint.Port);
                    }
                    options.ConfigurationOptions = configurationOptions;
                    options.InstanceName = "NetPro.AspNetRateLimit";
                });
                // inject counter and rules stores
                //services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();                   
                services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
                //注入计时器和规则
                //services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                //services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
                services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
                services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


            }
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            var configuration = application.ApplicationServices.GetRequiredService<IConfiguration>();
            //var configuration = webHost.Services.GetRequiredService<IConfiguration>();

            var isOpen = configuration.GetValue<bool>("EnableAspNetCoreRateLimit", false);
            if (isOpen)
            {
                using (var scope = application.ApplicationServices.CreateScope())
                {
                    // get the IpPolicyStore instance
                    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();
                    // seed IP data from appsettings
                    ipPolicyStore.SeedAsync().GetAwaiter().GetResult();
                    // get the ClientPolicyStore instance
                    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
                    // seed client data from appsettings
                    clientPolicyStore.SeedAsync().GetAwaiter().GetResult();
                }
                application.UseClientRateLimiting();
                application.UseIpRateLimiting();
            }

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order
        {
            //common services should be loaded after error handlers
            get { return 100; }
        }
    }
}
