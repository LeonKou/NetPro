using NetPro.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Web.Api.Infrastructure.Extensions;
using NetPro.TypeFinder;
using NetPro.Swagger;

namespace NetPro.Web.Api.Infrastructure
{
	/// <summary>
	/// 配置应用程序启动时MVC需要的中间件
	/// </summary>
	public class NetProApiStartup : INetProStartup
	{
		/// <summary>
		/// Add and configure any of the middleware
		/// </summary>
		/// <param name="services">Collection of service descriptors</param>
		/// <param name="configuration">Configuration root of the application</param>
		public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ITypeFinder typeFinder)
		{
			//配置 mvc服务
			services.AddNetProApi();
			services.AddNetProSwagger(configuration);
		}

		public void Configure(IApplicationBuilder application)
		{
			application.UseRouting();
			application.UseCors("CorsPolicy");
			application.UseAuthorization();
			application.UseNetProSwagger();
			application.UseEndpoints(s =>
			{
				s.MapControllers();
				//s.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
		}

		/// <summary>
		/// Gets order of this startup configuration implementation
		/// </summary>
		public int Order
		{
			//MVC should be loaded last
			get { return 1001; }
		}
	}
}
