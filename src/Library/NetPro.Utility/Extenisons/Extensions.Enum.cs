using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace System.NetPro
{
    public static partial class Extensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum) throw new ArgumentException(String.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0)
            {
                return "";
            }
            var member = members[0];
            var descriptionattributes = member.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionattributes.Length > 0)
            {
                return ((DescriptionAttribute)descriptionattributes[0]).Description;
            }
            else
            {
                var displayattributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (displayattributes.Length > 0)
                {
                    return ((DisplayAttribute)displayattributes[0]).GetName();
                }
                else
                {
                    return "";
                }
            }
        }


        /// <summary>
        /// 获得枚举类中有Description标签的字典, 没有标签的为null
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<TEnum, string> GetDescriptions<TEnum>() where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            var result = new Dictionary<TEnum, string>();
            var values = Enum.GetValues(typeof(TEnum));
            foreach (TEnum value in values)
            {
                var field = value.GetType().GetField(value.ToString());
                var objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                var text = (objs as DescriptionAttribute)?.Description;
                result.Add(value, text);
            }
            result = result.OrderBy(item => item.Key).ToDictionary(r => r.Key, v => v.Value);
            return result;
        }
        /// <summary>
        /// 返回枚举类中所有含有该标签的值,以及第一个标签
        /// </summary>
        /// <typeparam name="TEnum">目标枚举类</typeparam>
        /// <typeparam name="TAttr">目标标签</typeparam>
        /// <param name="predict">可选字段过滤表达式</param>
        /// <returns></returns>
        public static IEnumerable<(TEnum @enum, TAttr attr)> GetCustomAttributes<TEnum, TAttr>(Func<FieldInfo, bool> predict = null)
            where TEnum : struct, IComparable, IConvertible, IFormattable
            where TAttr : Attribute
        {
            var type = typeof(TEnum);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                             .Where(predict == null ? f => true : predict);
            foreach (var f in fields)
            {
                var attrs = f.GetCustomAttributes(typeof(TAttr), false).FirstOrDefault() as TAttr;
                yield return ((TEnum)f.GetValue(null), attrs);
            }
        }

        /// <summary>
        /// 获得枚举类中有Description标签的字典, 没有标签的为null
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescriptionsInt<TEnum>() where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            return GetDescriptions<TEnum>().ToDictionary(d => Convert.ToInt32(d.Key), d => d.Value);
        }
    }


    public class SelfAttribute : Attribute
    {
        private string m_DisplayText1 = string.Empty;
        private string m_DisplayText2 = string.Empty;
        public SelfAttribute(string displayText1, string displayText2)
        {
            m_DisplayText1 = displayText1;
            m_DisplayText2 = displayText2;
        }

        public string DisplayText1
        {
            get { return m_DisplayText1; }
        }

        public string DisplayText2
        {
            get { return m_DisplayText2; }
        }
    }

    /// <summary>
    /// 注释
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MultipleDescription : Attribute
    {
        /// <summary>
        /// 得到 值，和对应值得描述
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescriptionValues<TEnum>() where TEnum : struct, IComparable, IConvertible, IFormattable
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(TEnum));
            foreach (var value in values)
            {
                var field = value.GetType().GetField(value.ToString());
                var objs = field.GetCustomAttributes(typeof(MultipleDescription), false).FirstOrDefault();
                if (objs != null)
                {
                    var text = $"{((MultipleDescription)objs).ZH},{((MultipleDescription)objs).EN}";
                    result.Add((int)value, text);
                }
            }
            result = result.OrderBy(item => item.Key).ToDictionary(r => r.Key, v => v.Value);
            return result;
        }

        /// <summary>
        /// 得到枚举的中文
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static string GetZhDescription(object enumType)
        {
            var fieldinfo = enumType.GetType().GetField(enumType.ToString());
            var obj = fieldinfo.GetCustomAttributes(typeof(MultipleDescription), false).FirstOrDefault();
            return ((MultipleDescription)obj).ZH;
        }

        /// <summary>
        /// 得到枚举的英文
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static string GetEnDescription(object enumType)
        {
            var fieldinfo = enumType.GetType().GetField(enumType.ToString());
            var obj = fieldinfo.GetCustomAttributes(typeof(MultipleDescription), false).FirstOrDefault();
            return ((MultipleDescription)obj).ZH;
        }

        /// <summary>
        /// 中文
        /// </summary>
        public string ZH { get; } = "没有中文解释";
        /// <summary>
        /// 英文
        /// </summary>
        public string EN { get; } = "没有英文解释";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zh"></param>
        public MultipleDescription(string zh)
        {
            ZH = zh;
        }
        /// <summary>
        /// 注释
        /// </summary>
        /// <param name="zh">中文</param>
        /// <param name="en">英文</param>
        public MultipleDescription(string zh, string en)
        {
            ZH = zh;
            EN = en;
        }
    }
}
