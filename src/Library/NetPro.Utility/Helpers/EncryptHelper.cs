using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace System.NetPro
{
    /// <summary>
    /// 加密操作
    /// 说明：
    /// 1. AES加密整理自支付宝SDK
    /// 2. RSA加密采用 https://github.com/stulzq/DotnetCore.RSA/blob/master/DotnetCore.RSA/RSAHelper.cs
    /// </summary>
    public static class EncryptHelper
    {
        private static readonly object Locker = new object();

        /// <summary>
        /// 通用8位加密KEY
        /// </summary>
        public static string BaseKey = "SHUTDOWN";

        /// <summary>
        /// 生成指定字节密钥
        /// 默认16子节，128位
        /// </summary>
        /// <returns></returns>
        public static string GeneratedKey(int byteNumber = 16)
        {
            byte[] bytes = new byte[byteNumber];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            var key = Convert.ToBase64String(bytes);
            return key;
        }

        #region Md5加密

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By16(string value)
        {
            return Md5By16(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        private static string Md5By16(string value, Encoding encoding)
        {
            return Md5(value, encoding, 4, 8);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        private static string Md5(string value, Encoding encoding, int? startIndex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex.SafeValue(), length.SafeValue());
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "");
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        public static string Md5Upper(string encypStr)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.GetEncoding("utf-8").GetBytes(encypStr);
            bs = md5.ComputeHash(bs);
            return BytesToHexString(bs);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By32(string value)
        {
            return Md5By32(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        private static string Md5By32(string value, Encoding encoding)
        {
            return Md5(value, encoding, null, null);
        }

        /// <summary>
        /// 对象生成Md5 key
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>对象md5值</returns>
        public static string GenerateMd5Hash(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"The parameter cannot be null");
            }
            byte[] soruce;
            using (MemoryStream fs = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                lock (Locker)
                {
                    formatter.Serialize(fs, obj);
                }
                soruce = fs.ToArray();
            }
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(soruce);
            md5.Clear();
            string hashString = BitConverter.ToString(hash);
            return hashString.Replace("-", "");
        }
        #endregion

        #region DES加密

        /// <summary>
        /// DES密钥,24位字符串
        /// </summary>
        private static readonly string DesKey = "#s^qn2oe21fpv%|f0XpB,+vh";

        /// <summary>
        /// DES加密,默认24位密钥
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string DesEncrypt24(object value)
        {
            return DesEncrypt24(value, DesKey);
        }

        /// <summary>
        /// DES加密,密钥,24位
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesEncrypt24(object value, string key)
        {
            return DesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesEncrypt(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (ValidateDes(text, key) == false)
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateEncryptor())
            {
                return GetEncryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 验证Des加密参数
        /// </summary>
        private static bool ValidateDes(string text, string key)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(key))
                return false;
            return key.Length == 24;
        }

        /// <summary>
        /// 创建Des加密服务提供程序
        /// </summary>
        private static TripleDESCryptoServiceProvider CreateDesProvider(string key)
        {
            return new TripleDESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private static string GetEncryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = encoding.GetBytes(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// DES解密,24位密钥
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string DesDecrypt24(object value)
        {
            return DesDecrypt24(value, DesKey);
        }

        /// <summary>
        /// DES解密,密钥,24位
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesDecrypt24(object value, string key)
        {
            return DesDecrypt24(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES解密24
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesDecrypt24(object value, string key, Encoding encoding)
        {
            string text = value.SafeString();
            if (!ValidateDes(text, key))
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateDecryptor())
            {
                return GetDecryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private static string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = Convert.FromBase64String(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result);
        }

        #endregion

        #region DES old func
        private static byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }

            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException("hexString", hexString, "hexString must contain an even number of characters.");
            }

            byte[] result = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                result[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }

            return result;
        }

        private static string BytesToHexString(byte[] bytes)
        {
            var s = new StringBuilder();
            foreach (byte b in bytes)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            return s.ToString();
        }

        /// <summary>
        /// DES加密算法,密钥长度为8个字符
        /// </summary>
        /// <param name="data">加密明文</param>
        /// <param name="key">密钥长度为8个字符</param>
        /// <param name="charset">字符编码</param>
        /// <returns>返回密文</returns>
        public static string DesEncrypt8(string data, string key, string charset)
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(key)
            };


            byte[] inputByteArray = Encoding.GetEncoding(charset).GetBytes(data);

            var ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            byte[] ret = ms.ToArray();

            cs.Close();
            ms.Close();

            return BytesToHexString(ret);
        }

        /// <summary>
        /// DES加密,默认8位密钥
        /// utf-8
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DesEncrypt8(string data)
        {
            return DesEncrypt8(data, BaseKey, "utf-8");
        }

        /// <summary>
        /// DES加密，8位密钥
        /// utf-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesEncrypt8(string data, string key)
        {
            return DesEncrypt8(data, key, "utf-8");
        }

        /// <summary>
        /// DES 解密算法,密钥长度为8个字符
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="key">密钥长度为8个字符</param>
        /// <param name="charset">字符编码</param>
        /// <returns>明文</returns>
        public static string DesDecrypt8(string data, string key, string charset)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(key),
                IV = Encoding.ASCII.GetBytes(key)
            };


            byte[] inputByteArray = HexStringToBytes(data);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();


            var ret = ms.ToArray();

            cs.Close();
            ms.Close();
            var result = Encoding.GetEncoding(charset).GetString(ret);
            return result;
        }

        /// <summary>
        /// DES解密，8位密钥
        /// utf-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DesDecrypt8(string data, string key)
        {
            return DesDecrypt8(data, key, "utf-8");
        }

        /// <summary>
        /// DES 解密算法，默认8位密钥
        /// </summary>
        public static string DesDecrypt8(string data)
        {
            return DesDecrypt8(data, BaseKey);
        }
        #endregion

        #region AES加密

        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] _iv;
        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                if (_iv == null)
                {
                    var size = 16;
                    _iv = new byte[size];
                    for (int i = 0; i < size; i++)
                        _iv[i] = 0;
                }
                return _iv;
            }
        }

        /// <summary>
        /// AES密钥
        /// </summary>
        public static string AesKey = "QaP1AF8utIarcBqdhYTZpVGbiNQ9M6IL";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string AesEncrypt(string value)
        {
            return AesEncrypt(value, AesKey);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string AesEncrypt(string value, string key)
        {
            return AesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密DESCryptoServiceProvider
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string AesEncrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetEncryptResult(value, encoding, transform);
            }
        }

        /// <summary>
        /// 创建RijndaelManaged
        /// </summary>
        private static RijndaelManaged CreateRijndaelManaged(string key)
        {
            return new RijndaelManaged
            {
                Key = Convert.FromBase64String(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = Iv
            };
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string AesDecrypt(string value)
        {
            return AesDecrypt(value, AesKey);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        public static string AesDecrypt(string value, string key)
        {
            return AesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string AesDecrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetDecryptResult(value, encoding, transform);
            }
        }

        #endregion

        #region RSA签名

        /// <summary>
        /// Rsa签名，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string RsaSign(string value, string key)
        {
            return RsaSign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa签名，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string RsaSign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA);
        }

        /// <summary>
        /// Rsa签名，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string Rsa2Sign(string value, string key)
        {
            return Rsa2Sign(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa签名，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string Rsa2Sign(string value, string key, Encoding encoding)
        {
            return RsaSign(value, key, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa签名
        /// </summary>
        private static string RsaSign(string value, string key, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rsa = new RsaHelper(type, encoding, key);
            return rsa.Sign(value);
        }

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public static bool RsaVerify(string value, string publicKey, string sign)
        {
            return RsaVerify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA1 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public static bool RsaVerify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        public static bool Rsa2Verify(string value, string publicKey, string sign)
        {
            return Rsa2Verify(value, publicKey, sign, Encoding.UTF8);
        }

        /// <summary>
        /// Rsa验签，采用 SHA256 算法
        /// </summary>
        /// <param name="value">待验签的值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="sign">签名</param>
        /// <param name="encoding">编码</param>
        public static bool Rsa2Verify(string value, string publicKey, string sign, Encoding encoding)
        {
            return RsaVerify(value, publicKey, sign, encoding, RSAType.RSA2);
        }

        /// <summary>
        /// Rsa验签
        /// </summary>
        private static bool RsaVerify(string value, string publicKey, string sign, Encoding encoding, RSAType type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(sign))
                return false;
            var rsa = new RsaHelper(type, encoding, publicKey: publicKey);
            return rsa.Verify(value, sign);
        }

        /// <summary>
        ///  Rsa 解密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RsaDecrypt(string value, string privateKey)
        {
            return RsaDecrypt(value, privateKey, RSAType.RSA);
        }

        /// <summary>
        /// Rsa 加密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RsaEncrypt(string value, string privateKey)
        {
            return RsaEncrypt(value, privateKey, RSAType.RSA);
        }

        /// <summary>
        ///  Rsa2 解密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string Rsa2Decrypt(string value, string privateKey)
        {
            return RsaDecrypt(value, privateKey, RSAType.RSA2);
        }

        /// <summary>
        ///  Rsa2 加密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string Rsa2Encrypt(string value, string privateKey)
        {
            return RsaEncrypt(value, privateKey, RSAType.RSA2);
        }

        /// <summary>
        ///  Rsa2 解密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="type">加密类型</param>
        /// <returns></returns>
        private static string RsaDecrypt(string value, string privateKey, RSAType type)
        {
            var rsa = new RsaHelper(type, Encoding.UTF8, privateKey: privateKey);
            return rsa.Decrypt(value);
        }

        /// <summary>
        /// Rsa 加密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="type">加密类型</param>
        /// <returns></returns>
        private static string RsaEncrypt(string value, string publicKey, RSAType type)
        {
            var rsa = new RsaHelper(type, Encoding.UTF8, publicKey: publicKey);
            return rsa.Encrypt(value);
        }


        #endregion

        #region SHA256 加密       
        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Sha256(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            HashAlgorithm algorithm = new SHA256Managed();
            return BitConverter.ToString(algorithm.ComputeHash(bytes)).Replace("-", "").ToUpper();
        }
        #endregion
    }
}
