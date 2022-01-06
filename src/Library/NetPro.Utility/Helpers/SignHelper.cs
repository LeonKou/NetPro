using System;
using System.Collections.Generic;
using System.Linq;

namespace System.NetPro
{
    /// <summary>
    /// 签名帮助类
    /// </summary>
    public class SignHelper
    {
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <remarks>
        /// update:增加isToLower参数，忽略参数大小写
        /// updateBy: AceJ
        /// </remarks>
        /// <param name="keys">参数集合，不包含appId，sign参数</param>
        /// <param name="secret">密钥</param>
        /// <param name="isToLower">参数是否转换成小写，默认 false</param>
        /// <returns></returns>
        public static string GetSign(Dictionary<string, object> keys, string secret, bool isToLower = false)
        {
            var exculeKeys = new[] { "appid", "sign" };
            //if (!keys.ContainsKey("appid") || !keys.ContainsKey("sign") || !keys.ContainsKey("ticks")) return sign;
            //if (!long.TryParse(keys["ticks"], out long ticks)) return sign;
            if (string.IsNullOrEmpty(secret)) return null;
            keys = keys.ToDictionary(a => a.Key.ToLower(), a => a.Value);
            var sortKeys = keys.Where(a => !exculeKeys.Contains(a.Key)).ToList();
            sortKeys.Sort(new StringCompare());
            var paramKeys = sortKeys.ToDictionary(a => a.Key, a => a.Value);
            //var paramKeys = keys.ToDictionary(a => a.Key, a => a.Value);
            //a=ab=b
            //a=a&b=b
            string input = string.Join("&", paramKeys.Select(a => $"{a.Key}={a.Value?.ToString().Trim()}"));
            if (isToLower)
                input = input.ToLower();
            input += secret;
            var sign = EncryptHelper.Md5Upper(input);
            return sign;
        }

        public class StringCompare : IComparer<KeyValuePair<string, object>>
        {
            public int Compare(KeyValuePair<string, object> x, KeyValuePair<string, object> y)
            {
                var xarry = x.Key.Select(a => Convert.ToInt32(a)).ToArray();
                var yarry = y.Key.Select(a => Convert.ToInt32(a)).ToArray();
                if (xarry.Length > yarry.Length)
                {
                    var array = new int[xarry.Length];
                    yarry.CopyTo(array, 0);
                    yarry = array;
                }
                else
                {
                    var array = new int[yarry.Length];
                    xarry.CopyTo(array, 0);
                    xarry = array;
                }
                int sort = 0;
                for (int i = 0; i < xarry.Length; i++)
                {
                    if (xarry[i] == yarry[i])
                    {
                        if (i == xarry.Length - 1)
                            sort = 0;
                    }
                    else if (xarry[i] > yarry[i])
                    {
                        sort = 1;
                        break;
                    }
                    else
                    {
                        sort = -1;
                        break;
                    }
                }
                return sort;
            }
        }
    }

}