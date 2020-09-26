namespace NetPro.Utility
{
    /// <summary>
    /// viewModel Data validation failed exception
    /// </summary>
    public class ViewModelStateValidException : NetProException
    {
        public string BindModelText { get; set; }

        /// <summary>
        /// viewModel 数据验证失败异常构造函数
        /// </summary>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="bindModelText">bindmodel字符串</param>
        /// <param name="errorCode">错误代码</param>
        public ViewModelStateValidException(string errorMsg, string bindModelText, int errorCode) : base(errorMsg, errorCode)
        {
            BindModelText = bindModelText;
        }
    }
}
