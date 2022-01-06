namespace System.NetPro
{
    /// <summary>
    /// 验证操作
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="input">输入值</param>        
        public static bool IsNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            const string pattern = @"^(-?\d*)(\.\d+)?$";
            return RegexHelper.IsMatch(input, pattern);
        }
    }
}
