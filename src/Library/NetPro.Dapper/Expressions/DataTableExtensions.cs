using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace NetPro.Dapper.Expressions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// List 转换成Datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public delegate object[] CreateRowDelegate<T>(T t);
        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn, string[] fields = null)
        {
            //存表的列名
            DataTable dtReturn = new DataTable();

            // 访问属性元素
            PropertyInfo[] oProps = null;

            // 判断属性元素大于0就遍历

            foreach (T rec in varlist)
            {

                // 用反射来获取属性名，创建表，只执行第一次
                if (oProps == null)
                {
                    //得到公有属性
                    oProps = rec.GetType().GetProperties();
                    //遍历属性中的数据
                    foreach (PropertyInfo pi in oProps)
                    {

                        if (fields != null && string.IsNullOrWhiteSpace(fields.FirstOrDefault(s => s.ToLower().Equals(pi.Name.ToLower()))))
                        {
                            continue;
                        }
                        //获取属性的名称与类型   


                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {

                            colType = colType.GetGenericArguments()[0];

                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));

                    }

                }
                //将数据填充到行中
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    if (fields != null && string.IsNullOrWhiteSpace(fields.FirstOrDefault(s => s.ToLower().Equals(pi.Name.ToLower()))))
                    {
                        continue;
                    }
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);

                }

                dtReturn.Rows.Add(dr);

            }

            return (dtReturn);

        }


        public static DataTable ToDateTableNew<T>(this IEnumerable<T> modeList, string tableName, SqlConnection conn, SqlTransaction tran = null)
        {
            string sql = string.Format("select syscolumns.[name],syscolumns.colorder from syscolumns inner join sysobjects on syscolumns.id=sysobjects.id where sysobjects.xtype='U' and sysobjects.name='{0}' order by syscolumns.colid asc", tableName);
            var list = conn.Query<TableCoumns>(sql: sql, transaction: tran);

            DataTable dt = new DataTable();
            Type modelType = typeof(T);
            //  List<PropertyInfo> mappingProps = new List<PropertyInfo>();
            var props = modelType.GetProperties();

            foreach (var column in list)
            {
                PropertyInfo mappingProp = props.Where(a => a.Name.ToLower().Equals(column.Name.ToLower())).FirstOrDefault();
                if (mappingProp == null)
                {
                    dt.Columns.Add(new DataColumn(column.Name, typeof(String)));
                }
                else
                {
                    // mappingProps.Add(mappingProp);
                    Type dataType = GetUnderlyingType(mappingProp.PropertyType);
                    if (dataType.IsEnum)
                        dataType = typeof(int);
                    dt.Columns.Add(new DataColumn(column.Name, dataType));
                }
            }
            foreach (var model in modeList)
            {
                var oProps = model.GetType().GetProperties();
                DataRow dr = dt.NewRow();
                foreach (var column in list)
                {
                    var pi = oProps.FirstOrDefault(s => column.Name.ToLower().Equals(s.Name.ToLower()));
                    if (pi != null)
                    {
                        dr[column.Name] = pi.GetValue(model, null) == null ? DBNull.Value : pi.GetValue(model, null);
                    }
                    else
                    {
                        dr[column.Name] = DBNull.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private static Type GetUnderlyingType(Type type)
        {
            Type unType = Nullable.GetUnderlyingType(type); ;
            if (unType == null)
                unType = type;

            return unType;
        }
    }
    public class TableCoumns
    {
        public string Name { get; set; }
        public int colorder { get; set; }
        public int xxxxd { get; set; }
        public string ddddd { get; set; }
    }
}
