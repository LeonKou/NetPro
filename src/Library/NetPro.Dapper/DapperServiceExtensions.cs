using Microsoft.Extensions.DependencyInjection;
using NetPro.Dapper.Repositories;
using System;

namespace NetPro.Dapper
{
    public static class DapperServiceExtensions
    {
        [Obsolete("过时的数据库操作组件，ORM 建议使用freesql -->https://github.com/dotnetcore/FreeSql")]
        public static IServiceCollection AddDapperRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWorkFactory<>), typeof(NetProUnitOfWorkFactory<>));
            services.AddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));
            services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<,>));

            return services;
        }

    }
}
