using Microsoft.Extensions.DependencyInjection;
using NetPro.Dapper.Repositories;

namespace NetPro.Dapper
{
    public static class DapperServiceExtensions
    {
        public static IServiceCollection AddDapperRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWorkFactory<>),typeof(NetProUnitOfWorkFactory<>));
            services.AddScoped(typeof(IDapperRepository<>),typeof(DapperRepository<>) );
            services.AddScoped(typeof(IGeneralRepository<>),typeof(GeneralRepository<,>));

            return services;
        }

    }
}
