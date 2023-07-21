using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace NetPro.Pulsar.WebApiClient.ApiAttribute
{
    /// <summary>
    /// Basic认证
    /// </summary>
    public class BasicAuthorizationAttribute : ApiActionAttribute
    {
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var pulsarOption = context.HttpContext.ServiceProvider.GetService<IOptions<PulsarOption>>()?.Value;
            var basicAuthorization = new BasicAuthenticationHeaderValue(pulsarOption.Authentication.UserName, pulsarOption.Authentication.Password);
            context.HttpContext.RequestMessage.Headers.Authorization = basicAuthorization;

            return Task.CompletedTask;
        }
    }
}
