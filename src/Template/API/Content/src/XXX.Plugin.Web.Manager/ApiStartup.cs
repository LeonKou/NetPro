namespace XXX.Plugin.Web.Manager
{
    public class ApiStartup : INetProStartup
    {
        public double Order { get; set; } = int.MaxValue;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
        }
    }
}
