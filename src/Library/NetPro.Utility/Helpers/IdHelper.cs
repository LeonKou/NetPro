using IdGen;
using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace NetPro
{
    /// <summary>
    /// Id生成器
    /// </summary>
    public static class IdHelper
    {
        /// <summary>
        /// Id
        /// </summary>
        private static string _id;

        /// <summary>
        /// 设置Id
        /// </summary>
        /// <param name="id">Id</param>
        public static void SetId(string id)
        {
            _id = id;
        }

        /// <summary>
        /// 重置Id
        /// </summary>
        public static void Reset()
        {
            _id = null;
        }

        /// <summary>
        /// 创建Id
        /// </summary>
        public static string ObjectId()
        {
            return string.IsNullOrWhiteSpace(_id) ? Utility.Helpers.Internal.ObjectId.GenerateNewStringId() : _id;
        }

        /// <summary>
        /// 用Guid创建Id,去掉分隔符
        /// </summary>
        public static string Guid()
        {
            return string.IsNullOrWhiteSpace(_id) ? System.Guid.NewGuid().ToString("N") : _id;
        }

        /// <summary>
        /// 推特Snowflake算法C#版
        /// 详情了解https://github.com/RobThree/IdGen
        /// </summary>
        /// <returns></returns>
        public static long NewId()
        {
            //如id压力增大，后续移至NetPro.Core中,apollo配置

            var generator = new IdGenerator(0);

            return generator.CreateId();
        }
    }

    /// <summary>
    /// 公工服务
    /// </summary>
    public static partial class Extenisons
    {
        /// <summary>
        /// 生成基于时间戳的Id
        /// 例如 164570046556820221029
        /// </summary>
        /// <returns>毫秒时间戳+2位随机数+3位随机数</returns>
        public static string GenerateIdByTimestamp()
        {
            var huge = BigInteger.Parse(Guid.NewGuid().ToString("N"), NumberStyles.AllowHexSpecifier).ToString();
            var uniqueId = $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}{huge.Substring(2, 4)}{Environment.CurrentManagedThreadId.ToString().PadLeft(2, '0')[..2]}{RandomNumberGenerator.GetInt32(10, 99)}";
            return uniqueId;
        }

        /// <summary>
        /// 生成基于UTC时间的Id
        /// 例如 22022410470006830011910
        /// </summary>
        /// <returns>返回基于长度为23；格式：yyyyMMddHH+11位随机数</returns>
        public static string GenerateIdByTime()
        {
            var huge = BigInteger.Parse(Guid.NewGuid().ToString("N"), NumberStyles.AllowHexSpecifier).ToString();
            var timeString = DateTime.UtcNow.ToString("yyMMddHHmm");
            var uniqueId = $"{timeString}{huge.Substring(2, 11)}{Environment.CurrentManagedThreadId.ToString().PadLeft(2, '0')[..2]}";
            return uniqueId;
        }

        /// <summary>
        /// 生成指定长度随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string GenerateRandomNumber(int length)
        {
            StringBuilder startsb = new StringBuilder("1");
            StringBuilder endsb = new StringBuilder("9");
            for (int i = 0; i < length - 1; i++)
            {
                startsb.Append('0');
                endsb.Append('9');
            }
            int startNum = int.Parse(startsb.ToString());
            int endNum = int.Parse(endsb.ToString());
            var result = RandomNumberGenerator.GetInt32(startNum, endNum);

            return result.ToString();
        }

        /// <summary>
        ///  生成左补齐的随机数字
        /// </summary>
        /// <param name="fromInclusive">起始值</param>
        /// <param name="toExclusive">最大值</param>
        /// <param name="totalWidth">结果长度</param>
        /// <param name="paddingChar">补齐值，默认用0左补齐</param>
        /// <returns></returns>
        public static string GenerateRandomNumberPadLeft(int fromInclusive, int toExclusive, int totalWidth = 4, char paddingChar = '0')
        {
            var result = RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
            return result.ToString().PadLeft(totalWidth, paddingChar);
        }
    }
}
