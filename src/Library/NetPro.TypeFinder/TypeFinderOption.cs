namespace System.NetPro
{
    /// <summary>
    /// 
    /// </summary>
    public class TypeFinderOption
    {
        /// <summary>
        ///  挂载外部dll路径，用于插件化加载
        /// </summary>
        public string MountePath { get; set; }

        /// <summary>
        /// 自定义程序集正则模式
        /// 例如 ^XXX.* XXX前缀开头并加.
        /// </summary>
        public string CustomDllPattern { get; set; } = null;//RegexOptions.IgnoreCase
    }
}
