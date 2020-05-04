using System.ComponentModel;

namespace NetPro.Core.Consts
{
    /// <summary>
    /// App平台
    /// </summary>
    public enum EnumAppPlatform : byte
    {
        /// <summary>
        /// 無
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// Android
        /// </summary>
        [Description("Android")]
        Android = 1,

        /// <summary>
        /// IOS
        /// </summary>
        [Description("IOS")]
        Ios = 2,

        /// <summary>
        /// Windows
        /// </summary>
        [Description("Windows")]
        Windows = 3,

        /// <summary>
        /// Web
        /// </summary>
        [Description("Web")]
        Web = 4,


        /// <summary>
        /// 无法识别的平台
        /// </summary>
        [Description("无法识别的平台")]
        Unknown = 100,
    }
}
