using NetPro.Dapper.Parameters;
using System.Collections.Generic;

namespace NetPro.Dapper.Expressions
{
    /// <summary>
    ///
    /// </summary>
    public static class ParameterListExtension
    {
        /// <summary>
        /// 如果传入的name部位空，且value非类型T的默认值
        /// 则，plist新增一个 Parameter对象
        /// </summary>
        /// <typeparam name="T">“值”得类型</typeparam>
        /// <param name="plist">参数列表</param>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="operateType">运算类型</param>
        public static void AddIfValueNotDefault<T>(
            this IList<Parameter> plist,
            string name,
            T value,
            OperateType operateType)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (EqualityComparer<T>.Default.Equals(value, default(T))) return;
            plist.Add(new Parameter(name, value, operateType));
        }

        /// <summary>
        /// 如果传入的name部位空，且value非类型T的默认值
        /// 则，plist新增一个 Parameter对象
        /// </summary>
        /// <typeparam name="T">“值”的类型</typeparam>
        /// <param name="plist">参数列表</param>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        public static void AddIfValueNotDefault<T>(
                 this IList<Parameter> plist,
            string name, T value)
        {
            plist.AddIfValueNotDefault(name, value, OperateType.Equal);
        }
    }

}
