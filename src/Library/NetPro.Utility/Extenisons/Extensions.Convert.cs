namespace System.NetPro
{
    /// <summary>
    /// 系统扩展 - 类型转换
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Html编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToHtmlEncodeString(this string obj)
        {
            return System.Web.HttpUtility.HtmlEncode(obj);
        }
        /// <summary>
        /// 安全转换为字符串，去除两端空格，当值为null时返回""
        /// </summary>
        /// <param name="input">输入值</param>
        public static string SafeString(this object input)
        {
            return input == null ? string.Empty : input.ToString().Trim();
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="obj">数据</param>
        public static int ToInt(this object obj)
        {
            return ConvertHelper.ToInt(obj);
        }
    }
}
