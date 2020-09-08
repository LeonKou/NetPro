using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NetPro.Authentication
{
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseNetProAuthentication(
           this IApplicationBuilder builder)
        {
            var configuration = builder.ApplicationServices.GetService<IConfiguration>();
            var option = configuration.GetSection(nameof(AuthenticationOption)).Get<AuthenticationOption>();
            if (option?.Enabled ?? false)
                builder.UseAuthentication();
            return builder;
        }
    }
}
