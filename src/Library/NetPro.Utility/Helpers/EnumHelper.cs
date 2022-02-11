using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System.NetPro
{
    /// <summary>
    /// 枚举操作
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="member">成员名或值,范例:Enum1枚举有成员A=0,则传入"A"或"0"获取 Enum1.A</param>
        public static TEnum Parse<TEnum>(object member)
        {
            string value = member.SafeString();
            if (string.IsNullOrWhiteSpace(value))
            {
                if (typeof(TEnum).IsGenericType)
                    return default(TEnum);
                throw new ArgumentNullException(nameof(member));
            }
            return (TEnum)Enum.Parse(CommonHelper.GetType<TEnum>(), value, true);
        }

        /// <summary>
        /// 获取成员名
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="member">成员名、值、实例均可,范例:Enum1枚举有成员A=0,则传入Enum1.A或0,获取成员名"A"</param>
        public static string GetName<TEnum>(object member)
        {
            return GetName(CommonHelper.GetType<TEnum>(), member);
        }

        /// <summary>
        /// 获取成员名
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="member">成员名、值、实例均可</param>
        public static string GetName(Type type, object member)
        {
            if (type == null)
                return string.Empty;
            if (member == null)
                return string.Empty;
            if (member is string)
                return member.ToString();
            if (type.GetTypeInfo().IsEnum == false)
                return string.Empty;
            return Enum.GetName(type, member);
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="member">成员名、值、实例均可，范例:Enum1枚举有成员A=0,可传入"A"、0、Enum1.A，获取值0</param>
        public static int GetValue<TEnum>(object member)
        {
            return GetValue(CommonHelper.GetType<TEnum>(), member);
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="member">成员名、值、实例均可</param>
        public static int GetValue(Type type, object member)
        {
            string value = member.SafeString();
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(member));
            return (int)Enum.Parse(type, member.ToString(), true);
        }

        /// <summary>
        /// 获取描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="member">成员名、值、实例均可</param>
        public static string GetDescription<TEnum>(object member)
        {
            return ReflectionHelper.GetDescription<TEnum>(GetName<TEnum>(member));
        }

        /// <summary>
        /// 获取描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="member">成员名、值、实例均可</param>
        public static string GetDescription(Type type, object member)
        {
            return ReflectionHelper.GetDescription(type, GetName(type, member));
        }

        #region 获取枚举Description注释
        /// <summary>
        /// 获取枚举Description注释
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string Description(Enum en)
        {
            Type type = en.GetType(); MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        /// <summary>
        /// 获取自定义枚举特性描述（传输描述位置）
        /// </summary>
        /// <param name="enumSubitem"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        public static void GetSelfAttributeInfo(Enum enumSubitem, out string text, int pos)
        {
            Object obj = GetAttributeClass(enumSubitem, typeof(SelfAttribute));
            if (obj == null)
            {
                text = enumSubitem.ToString();
            }
            else
            {
                SelfAttribute da = (SelfAttribute)obj;
                if (pos == 1)
                    text = da.DisplayText1;
                else if (pos == 2)
                    text = da.DisplayText2;
                else
                    text = da.DisplayText1;
            }
        }

        /// <summary>
        /// 获取枚举类子项描述信息
        /// </summary>
        /// <param name="enumSubitem">枚举类子项</param>        
        public static string GetEnumDescription(Enum enumSubitem)
        {
            Object obj = GetAttributeClass(enumSubitem, typeof(DescriptionAttribute));
            if (obj == null)
            {
                return enumSubitem.ToString();
            }
            else
            {
                DescriptionAttribute da = (DescriptionAttribute)obj;
                return da.Description;
            }
        }

        /// <summary>
        /// 获取自定义枚举特性描述
        /// </summary>
        /// <param name="enumSubitem"></param>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        public static void GetselfAttributeInfo(Enum enumSubitem, out string text1, out string text2)
        {
            Object obj = GetAttributeClass(enumSubitem, typeof(SelfAttribute));
            if (obj == null)
            {
                text1 = text2 = enumSubitem.ToString();
            }
            else
            {
                SelfAttribute da = (SelfAttribute)obj;
                text1 = da.DisplayText1;
                text2 = da.DisplayText2;
            }
        }


        /// <summary>
        /// 根据枚举获取 值，描述，可以用于生产下拉列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescriptionValues<T>()
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                //取得枚举的描述：做下拉的名称
                var field = value.GetType().GetField(value.ToString());
                var objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                var text = objs.Length == 0 ? value.ToString() : ((DescriptionAttribute)objs[0]).Description;
                result.Add((int)value, text);
            }
            result = result.OrderBy(item => item.Key).ToDictionary(r => r.Key, v => v.Value);
            return result;
        }

        /// <summary>
        /// 获取指定属性类的实例
        /// </summary>
        /// <param name="enumSubitem">枚举类子项</param>
        /// <param name="attributeType">DescriptionAttribute属性类或其自定义属性类 类型，例如：typeof(DescriptionAttribute)</param>
        /// <returns></returns>
        private static Object GetAttributeClass(Enum enumSubitem, Type attributeType)
        {
            FieldInfo fieldinfo = enumSubitem.GetType().GetField(enumSubitem.ToString());
            if (fieldinfo == null)
            {
                return null;
            }
            Object[] objs = fieldinfo.GetCustomAttributes(attributeType, false);
            if (objs.Length == 0)
            {
                return null;
            }
            return objs[0];
        }

        #endregion

    }
}
