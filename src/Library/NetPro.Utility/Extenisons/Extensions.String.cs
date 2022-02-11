using System.IO;
using System.IO.Compression;
using System.Text;

namespace System.NetPro
{
    public static partial class Extensions
    {
        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(this string input)
        {
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(input);
            byte[] compressAfterByte = compressBeforeByte.Compress();
            string compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        /// <summary>
        /// 转base64
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64(this string input)
        {
            byte[] bytes = Encoding.Default.GetBytes(input);
            string str = Convert.ToBase64String(bytes);
            return str;
        }

        /// <summary>
        /// 字节流压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// 字符串解压
        /// </summary>
        public static string DecompressString(this string input)
        {
            string compressString = "";
            byte[] compressBeforeByte = Convert.FromBase64String(input);
            byte[] compressAfterByte = compressBeforeByte.Decompress();
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }

        /// <summary>
        /// 字节流解压
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(this byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
