using System.Collections.Generic;

namespace System.NetPro
{
    /// <summary>
    /// 系统扩展 - 公共
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 安全获取值，当值为null时，不会抛出异常
        /// </summary>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>(this T? value) where T : struct
        {
            return value ?? default(T);
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static int Value(this System.Enum instance)
        {
            return EnumHelper.GetValue(instance.GetType(), instance);
        }

        /// <summary>
        /// 获取枚举描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static string Description(this System.Enum instance)
        {
            return EnumHelper.GetDescription(instance.GetType(), instance);
        }

        /// <summary>
        /// 转换为用分隔符连接的字符串
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
        /// <param name="separator">分隔符，默认使用逗号分隔</param>
        public static string Join<T>(this IEnumerable<T> list, string quotes = "", string separator = ",")
        {
            return CommonHelper.Join(list, quotes, separator);
        }

        /// <summary>
        /// 比较字符串 忽略大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="traget"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this String str, string traget)
        {
            if (str == null || traget == null) return false;
            return str.ToUpper() == traget.ToUpper() ? true : false;
        }

        /// <summary>
        /// 生成对象MD5值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToMd5Hash(this object obj)
        {
            return EncryptHelper.GenerateMd5Hash(obj);
        }
    }
}
