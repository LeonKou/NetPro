using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.RedisManager
{
    internal static class Common
    {
        /// <summary>
        /// 反序列化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 反序列化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertObj<T>(string value)
        {
            value = value.TrimStart('\"').TrimEnd('\"');
            if (typeof(T) == typeof(string) || typeof(T).IsValueType)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 序列化为字节
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item);
            jsonString = jsonString.TrimStart('\"').TrimEnd('\"');
            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 序列化为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string SerializeToString<T>(T item)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsValueType)
            {
                return Convert.ToString(item);
            }

            var jsonString = JsonConvert.SerializeObject(item);
            return jsonString;
        }

        public static void CheckAndProcess(ref string key, ref int expiredTime)
        {
            //TODO 检查key规则 
        }

        public static string CheckAndProcess(ref string key)
        {
            return key;
            //TODO 检查key规则 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var valueProperty = type.GetProperty("Value");
                type = valueProperty.PropertyType;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
