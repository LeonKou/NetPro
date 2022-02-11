using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.NetPro;

namespace NetPro.AutoMapper
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMapperExtensionService
    {
        /// <summary>
        /// Register and configure AutoMapper
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        public static IServiceCollection AddNetProAutoMapper(this IServiceCollection services, ITypeFinder typeFinder)
        {
            //https://www.cnblogs.com/yan7/p/8085410.html
            //find mapper configurations provided by other assemblies
            var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            //register
            AutoMapperConfiguration.Init(config);
            services.AddSingleton(AutoMapperConfiguration.Mapper);
            return services;
        }
    }
}
