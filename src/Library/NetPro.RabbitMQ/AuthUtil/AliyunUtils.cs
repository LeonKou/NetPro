using System;
using System.Security.Cryptography;
using System.Text;

namespace MQMiddleware.AuthUtil
{
    public static class AliyunUtils
    {
        internal static readonly int FROM_USER = 0;
        internal static readonly string COLON = ":";
        internal static readonly DateTime EPOCH_START = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string getUserName(string ak, string resourceOwnerId)
        {
            StringBuilder data = new StringBuilder(64);
            data.Append(FROM_USER).Append(COLON).Append(resourceOwnerId).Append(COLON).Append(ak);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data.ToString()));
        }

        public static string getUserName(string ak, string resourceOwnerId, string stsToken)
        {
            StringBuilder data = new StringBuilder(64);
            data.Append(FROM_USER).Append(COLON).Append(resourceOwnerId).Append(COLON).Append(ak).Append(COLON).Append(stsToken);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data.ToString()));
        }

        public static string getPassword(string sk)
        {
            TimeSpan ts = DateTime.UtcNow - EPOCH_START;
            long timestamp = Convert.ToInt64(ts.TotalMilliseconds);

            KeyedHashAlgorithm algorithm = KeyedHashAlgorithm.Create("HMACSHA1");
            if (null == algorithm)
            {
                throw new InvalidOperationException("HMACSHA1 not exist!");
            }

            try
            {
                algorithm.Key = Encoding.UTF8.GetBytes(timestamp.ToString());
                byte[] bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(sk));
                string signature = byteArrayToUpperString(bytes);

                //  Console.WriteLine(signature);
                StringBuilder data = new StringBuilder(64);
                data.Append(signature).Append(COLON).Append(timestamp);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(data.ToString()));
            }
            finally
            {
                algorithm.Clear();
            }
        }

        private static string byteArrayToUpperString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes array null");
            }
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString().ToUpper();
        }
    }
}
