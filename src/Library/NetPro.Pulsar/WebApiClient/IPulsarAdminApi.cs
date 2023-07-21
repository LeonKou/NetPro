using NetPro.Pulsar.WebApiClient.ApiAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace NetPro.Pulsar
{
    /// <summary>
    /// Pulsar管理平台接口
    /// </summary>
    [BasicAuthorization]
    [LoggingFilter]
    public interface IPulsarAdminApi : IHttpApi
    {
        /// <summary>
        /// 获取pulsar租户
        /// </summary>        
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("/admin/v2/tenants")]
        [RawReturn(EnsureSuccessStatusCode = false)]
        Task<HttpResponseMessage> GetTenantsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantAndProject"></param>
        /// <returns></returns>
        [HttpPut]
        [JsonReturn(EnsureSuccessStatusCode = false)]
        Task CreateNameSpaceAsync([Uri] string tenantAndProject);
    }
}
