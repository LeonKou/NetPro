using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Maikebing.Data.Taos
{
    public static class DataReaderExtensions
    {
        public static T ToJson<T>(this IDataReader dataReader) where T : class
        {
            return dataReader.ToJson().ToObject<T>();
        }
        public static List<T> ToList<T>(this IDataReader dataReader) where T : class
        {
            return dataReader.ToJson().ToObject<List<T>>();
        }
        public static List<T> ToObject<T>(this IDataReader dataReader)
        {
            List<T> jArray = new List<T>();
            try
            {
                var t = typeof(T);
                var pots = t.GetProperties();
                while (dataReader.Read())
                {
                    T jObject = Activator.CreateInstance<T>();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        try
                        {
                            string strKey = dataReader.GetName(i);
                            if (dataReader[i] != DBNull.Value)
                            {
                                var pr = from p in pots where   p.Name == strKey &&  p.CanWrite select p;
                                if (pr.Any())
                                {
                                    var pi = pr.FirstOrDefault();
                                    pi.SetValue(jObject, Convert.ChangeType(dataReader[i],pi.PropertyType) );
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    jArray.Add(jObject);
                }
            }
            catch (Exception ex)
            {
                TaosException.ThrowExceptionForRC(-10002, $"ToObject<{nameof(T)}>  Error", ex);
            }
            return jArray;
        }

        public static JArray ToJson(this IDataReader dataReader)
        {
            JArray jArray = new JArray();
            try
            {

                while (dataReader.Read())
                {
                    JObject jObject = new JObject();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        try
                        {
                            string strKey = dataReader.GetName(i);
                            if (dataReader[i] != DBNull.Value)
                            {
                                object obj = Convert.ChangeType(dataReader[i], dataReader.GetFieldType(i));
                                jObject.Add(strKey, JToken.FromObject(obj));
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    jArray.Add(jObject);
                }
            }
            catch (Exception ex)
            {
                TaosException.ThrowExceptionForRC(-10001, "ToJson Error", ex);
            }
            return jArray;
        }
        public static DataTable ToDataTable(this IDataReader reader)
        {
            var datatable = new DataTable();
            datatable.Load(reader);
            return datatable;
        }
        public static string RemoveNull(this string str)
        {
            return (!string.IsNullOrEmpty(str) &&  str.IndexOf('\0')>0)? str.Remove(str.IndexOf('\0')):str;
        }
    }
}
