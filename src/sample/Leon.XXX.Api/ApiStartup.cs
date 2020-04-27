using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetPro.Core.Infrastructure;

namespace Leon.XXX.Api
{
	public class ApiStartup : INetProStartup
	{
		public int Order => 900;

		public void ConfigureServices(IServiceCollection services, IConfiguration configuration = null, ITypeFinder typeFinder = null)
		{

		}

		public void Configure(IApplicationBuilder application)
		{	           
        }
	}
}
