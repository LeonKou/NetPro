using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQMiddleware;
using MQMiddleware.Configuration;
using NetPro;
using NetPro.Core.Infrastructure;
using NetPro.TypeFinder;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace Leon.XXXV2.Api
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = 1000;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s =>
                    s.GetName().Name.EndsWith("Leon.XXXV2.Api")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            // var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();

            // var connectionString = configuration.GetValue<string>("ConnectionStrings:MysqlConnection");
            // Fsql = new FreeSql.FreeSqlBuilder()
            // .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            // .UseAutoSyncStructure(false) //自动同步实体结构到数据库
            // .Build(); //请务必定义成 Singleton 单例模式
            // services.AddSingleton<IFreeSql>(Fsql);

            // services.AddFreeRepository(null,
            //this.GetType().Assembly);//批量注入Repository

#if DEBUG
            services.AddRabbitMqClient(new RabbitMqClientOptions
            {
                HostName = "ribbitmq-rabbitmq",
                Port = 5672,
                Password = "609aZL4zBQ",
                UserName = "user",
                VirtualHost = "/",
            })
            .AddProductionExchange("exchange", new RabbitMqExchangeOptions
            {
                DeadLetterExchange = "DeadExchange",
                AutoDelete = false,
                Type = ExchangeType.Direct,
                Durable = true,
                Queues = new List<RabbitMqQueueOptions> {
                new RabbitMqQueueOptions { AutoDelete = false, Exclusive = false, Durable = true, Name = "exchange" , RoutingKeys = new HashSet<string> { string.Empty } } }
            })
            .AddConsumptionExchange($"exchange", new RabbitMqExchangeOptions
            {
                DeadLetterExchange = "DeadExchange",
                AutoDelete = false,
                Type = ExchangeType.Direct,
                Durable = true,
                Queues = new List<RabbitMqQueueOptions> { new RabbitMqQueueOptions { AutoDelete = false, Exclusive = false, Durable = true, Name = "exchange", RoutingKeys = new HashSet<string> { string.Empty } } }
            })
            .AddAsyncMessageHandlerSingleton<CustomerMessageHandler>(string.Empty);

            //services.BuildServiceProvider().GetRequiredService<IQueueService>().StartConsuming();
#endif
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
        }
    }
}
