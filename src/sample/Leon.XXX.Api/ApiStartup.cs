namespace Leon.XXX.Api
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;
        public static IFreeSql Fsql { get; private set; }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s =>s.GetName().Name.EndsWith("Leon.XXX.Domain")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s =>s.GetName().Name.EndsWith("Leon.XXX.Repository")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());

            var option = configuration.GetSection(nameof(VerifySignOption)).Get<VerifySignOption>();

            //覆盖请求签名组件
            services.AddVerifySign(s =>
             {
                 //自定义签名摘要逻辑
                 s.OperationFilter<VerifySignCustomer>();
             });

            var connectionString = configuration.GetValue<string>("ConnectionStrings:MysqlConnection");
            Fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            .UseAutoSyncStructure(false) //自动同步实体结构到数据库
            .Build(); //请务必定义成 Singleton 单例模式
            services.AddSingleton<IFreeSql>(Fsql);

            services.AddFreeRepository(null,
           this.GetType().Assembly);//批量注入Repository


            var healthbuild = services.AddHealthChecks();

            //services.AddRabbitMqClient(new RabbitMqClientOptions
            //{
            //    HostName = "ribbitmq-rabbitm",
            //    Port = 5672,
            //    Password = "609aZL4zBQ",
            //    UserName = "user",
            //    VirtualHost = "/",
            //})
            //    .AddProductionExchange("exchange", new RabbitMqExchangeOptions
            //    {
            //        DeadLetterExchange = "DeadExchange",
            //        AutoDelete = false,
            //        Type = ExchangeType.Direct,
            //        ConsumeFailedAction= ConsumeFailedAction.RETRY,
            //        Durable = true,
            //        Queues = new List<RabbitMqQueueOptions> {
            //           new RabbitMqQueueOptions { AutoDelete = false, Exclusive = false, Durable = true, Name = "exchange" , RoutingKeys = new HashSet<string> { string.Empty } } }
            //    })
            //    .AddConsumptionExchange($"exchange", new RabbitMqExchangeOptions
            //    {
            //        DeadLetterExchange = "DeadExchange",
            //        AutoDelete = false,
            //        Type = ExchangeType.Direct,
            //        ConsumeFailedAction = ConsumeFailedAction.RETRY,
            //        Durable = true,
            //        Queues = new List<RabbitMqQueueOptions> { new RabbitMqQueueOptions { AutoDelete = false, Exclusive = false, Durable = true, Name = "exchange", RoutingKeys = new HashSet<string> { string.Empty } } }
            //    })
            //    .AddMessageHandlerSingleton<CustomerMessageHandler>(string.Empty);

            //services.BuildServiceProvider().GetRequiredService<IQueueService>().StartConsuming();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseHealthChecks("/health", new HealthCheckOptions()//健康检查服务地址
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            application.UseCheck(envPath: "/env", infoPath: "/info");//envPath:应用环境地址；infoPath:应用自身信息地址
        }
    }
}
