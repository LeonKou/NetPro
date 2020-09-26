using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Authentication
{
    public static class AuthenticationServiceExtension
    {
        public static IServiceCollection AddNetProAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            var option = configuration.GetSection(nameof(AuthenticationOption)).Get<AuthenticationOption>();
            if (option?.Enabled ?? false)
            {
                services.AddAuthentication(options =>
                 {
                     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateAudience = false,
                         ValidateIssuer = false,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.Secret)),
                         ValidateLifetime = true,
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnAuthenticationFailed = context =>
                         {
                             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                             {
                                 context.Response.Headers.Add("Token-Expired", "true");
                             }
                             return Task.CompletedTask;
                         },
                     };
                 });
            }
            return services;
        }
    }
}
