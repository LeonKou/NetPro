namespace XXX.Plugin.Web.Demo
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
            //批量注入 Service 后缀的类
            services.Scan(scan => scan
              .FromAssemblies(typeFinder.GetAssemblies().Where(s => s.GetName().Name.EndsWith("XXX.Plugin.Web.Demo")).ToArray())
              .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
              .AsImplementedInterfaces()
              .WithScopedLifetime());
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
