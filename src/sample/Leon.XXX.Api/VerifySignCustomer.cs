using Microsoft.Extensions.Configuration;
using NetPro.Sign;

namespace Leon.XXX.Api
{
    public class VerifySignCustomer : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignCustomer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 根据appid获取secret
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public string GetSignSecret(string appid)
        {
            var secret = "1111";
            return secret;
        }

        /// <summary>
        /// 定义摘要算法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="signMethod"></param>
        /// <returns></returns>
        public string GetSignhHash(string message, string secret, EncryptEnum signMethod = EncryptEnum.Default)
        {
            return "5555555";
        }
    }
}
