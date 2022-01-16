using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace System.NetPro
{
    /// <summary>
    /// 集合类型扩展方法
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 数据转为DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection) where T : class, new()
        {
            var tb = new DataTable(typeof(T).Name);
            if (collection == null || !collection.Any())
                return tb;

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in collection)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }

        /// <summary>
        /// DataTable数据转为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable source) where T : class, new()
        {
            List<T> list = new List<T>();
            if (source == null || source.Rows.Count == 0)
                return list;

            //获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in source.Rows)
            {
                T t = new T();
                //获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  //检查DataTable是否包含此列    
                    if (source.Columns.Contains(tempName))
                    {
                        //判断此属性是否有Setter      
                        if (!pi.CanWrite)
                            continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
    }
}
