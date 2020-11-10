using Microsoft.Extensions.Configuration;

namespace NetPro.Sign
{
    public class VerifySignDefaultFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        public VerifySignDefaultFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 获取appid对应的secret
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public virtual string GetSignSecret(string appid)
        {
            var secret = _configuration.GetValue<string>($"{nameof(VerifySignOption)}:AppSecret:AppId:{appid}");

            return secret;
        }

        /// <summary>
        /// 签名算法；支持hmac-sha256；md5
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="signMethod">哈希算法，默认HMACSHA256</param>
        /// <returns>签名16进制</returns>
        public virtual string GetSignhHash(string message, string secret, EncryptEnum signMethod = EncryptEnum.Default)
        {
            if (signMethod.HasFlag(EncryptEnum.Default) || signMethod.HasFlag(EncryptEnum.SignHMACSHA256))
            {
                return SignCommon.GetHMACSHA256Sign(message, secret);
            }

            else if (signMethod.HasFlag(EncryptEnum.SignSHA256))
            {
                return SignCommon.GetSHA256Sign(message, secret);
            }
            else if (signMethod.HasFlag(EncryptEnum.SignMD5))
            {
                return SignCommon.CreateMD5(message, secret);
            }
            else
            {
                return SignCommon.GetHMACSHA256Sign(message, secret);
            }
        }
    }
}
