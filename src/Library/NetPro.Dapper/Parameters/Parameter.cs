using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPro.Dapper.Parameters
{
    /// <summary>
    /// 自定义sql参数
    /// </summary>
    [Serializable]
    public class Parameter
    {
        public Parameter() { }

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public Parameter(string name, object value, OperateType operateType = OperateType.Equal)
        {
            Name = name;
            OperateType = operateType;
            Value = value;
        }

        public Parameter(string name, object value, OperateType operateType = OperateType.Equal, LogicType logicType = LogicType.And) : this(name, value, operateType)
        {
            LogicType = logicType;
        }

        public string Name { get; set; }
        public object Value { get; set; } = null;
        public OperateType OperateType { get; set; }
        public LogicType LogicType { get; set; }
        public bool EmpytToDBNull { get; set; }

    }

    /// <summary>
    /// sql查询条件构造器
    /// </summary>
    public static class BuildSqlParameter
    {
        /// <summary>
        /// 添加查询条件
        /// （不包含【WHERE】关键字）
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        [Obsolete("为兼容老项目，不推荐使用！！")]
        public static string GetSqlParameter(this Parameter[] pars)
        {
            StringBuilder sbJoin = new StringBuilder();
            string wheres = "";
            if (pars == null || pars.Length == 0)
                return "";

            foreach (Parameter p in pars)
            {
                switch (p.OperateType)
                {
                    case OperateType.Between:
                        wheres += string.Format(" {0} {1} between {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.End:
                        wheres += string.Format(" {0}  {1} ", p.LogicType.ToString(), "'" + p.Value + "'");
                        break;
                    case OperateType.Equal:
                        if (p.Value is DBNull)
                            wheres += string.Format(" {0} {1} IS NULL ", p.LogicType.ToString(), p.Name);
                        else
                            wheres += string.Format(" {0} {1} = {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.NotEqual:
                        if (p.Value is DBNull)
                            wheres += string.Format(" {0} {1} IS NOT NULL ", p.LogicType.ToString(), p.Name);
                        else
                            wheres += string.Format(" {0} {1} != {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.Greater:
                        wheres += string.Format(" {0} {1} > {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.Less:
                        wheres += string.Format(" {0} {1} < {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.GreaterEqual:
                        wheres += string.Format(" {0} {1} >= {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.LessEqual:
                        wheres += string.Format(" {0} {1} <= {2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.Like:
                        wheres += string.Format(" {0} {1} like '%'+{2}+'%' ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.LeftLike:
                        wheres += string.Format(" {0} {1} like {2}+'%' ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.RightLike:
                        wheres += string.Format(" {0} {1} like '%'+{2} ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.NotLike:
                        wheres += string.Format(" {0} {1} not like '%'+{2}+'%' ", p.LogicType.ToString(), p.Name, "'" + p.Value + "'");
                        break;
                    case OperateType.In:
                        if (p.Value != null)
                            wheres += string.Format(" {0} {1} in ({2})", p.LogicType.ToString(), p.Name, "'" + string.Join("','", p.Value) + "'");
                        else
                            wheres += string.Format(" {0} 1<>1 ", p.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        if (p.Value != null)
                            wheres += string.Format(" {0} {1} not in ({2})", p.LogicType.ToString(), p.Name, "'" + string.Join("','", p.Value) + "'");
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr2 = p.Value as object[];
                        if (arr2 != null)
                        {
                            string ps = string.Empty;
                            for (int i = 0; i < arr2.Length; i++)
                            {
                                ps += (",'" + arr2[i].ToString() + "'");
                            }
                            wheres += string.Format(p.Name, ps.Substring(1));
                        }
                        else
                        {
                            wheres += string.Format(p.Name, "'" + p.Value + "'");
                        }
                        break;
                    case OperateType.SqlFormat:
                        object[] arr3 = p.Value as object[];
                        if (arr3 != null)
                            wheres += string.Format(p.Name, ArrayToString(arr3));
                        else
                            wheres += string.Format(p.Name, p.Value);
                        break;
                    default:
                        break;
                }
            }
            return wheres;
        }

        /// <summary>
        /// 强化分页版本
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="tables"></param>
        /// <param name="pars"></param>
        /// <param name="orderby"></param>
        /// <param name="pageIndex"></param>
        /// <param name="length"></param>
        /// <param name="sqlcount"></param>
        /// <returns></returns>
        public static string GetSqlStringOp(string fields, string tables, Parameter[] pars, string orderby, int pageIndex, int length, ref string sqlcount, ref DynamicParameters dp)
        {
            if (length == -1)
            {
                string sql = "select ";
                string sqlorderby = " order by " + orderby;
                string sqlfields = !string.IsNullOrWhiteSpace(fields) ? fields : "*";
                string sqltable = " from " + tables;
                string wherefield = string.Empty;
                string strSqlJoin = string.Empty;
                dp = GetDynamicParameter(ref wherefield, out strSqlJoin, pars);

                if (!string.IsNullOrWhiteSpace(strSqlJoin))
                    sqltable += $" {strSqlJoin} ";

                if (!string.IsNullOrWhiteSpace(wherefield))
                    sqltable += " where 1=1 " + wherefield;
                string neworderby = "";
                if (sqlorderby.IndexOf('.') > -1)
                    neworderby = sqlorderby.Substring(sqlorderby.IndexOf('.') + 1, sqlorderby.Length - sqlorderby.IndexOf('.') - 1);
                else
                    neworderby = orderby;

                sql += sqlfields + sqltable + sqlorderby;
                sqlcount = "select count(1) " + sqltable;
                return sql;
            }
            else
            {


                string sql = "select top " + (pageIndex + 1) * length + " ";
                string sqlorderby = " order by " + orderby;
                string sqlfields = !string.IsNullOrWhiteSpace(fields) ? fields : "*";
                string sqltable = " into #Temporary from " + tables;
                string wherefield = string.Empty;
                string strSqlJoin = string.Empty;
                dp = GetDynamicParameter(ref wherefield, out strSqlJoin, pars);

                if (!string.IsNullOrWhiteSpace(strSqlJoin))
                    sqltable += $" {strSqlJoin} ";

                if (!string.IsNullOrWhiteSpace(wherefield))
                    sqltable += " where 1=1 " + wherefield;

                string neworderby = "";
                if (sqlorderby.IndexOf('.') > -1)
                    neworderby = sqlorderby.Substring(sqlorderby.IndexOf('.') + 1, sqlorderby.Length - sqlorderby.IndexOf('.') - 1);
                else
                    neworderby = orderby;
                string sqlpage = ";with tempTable as (select Row_number() over(order by " + neworderby + ") as num, * from #Temporary)";
                sqlpage += "select top " + length + " * from tempTable where num > " + pageIndex * length + " order by num ;drop table #Temporary";
                sql += sqlfields + sqltable + sqlorderby + sqlpage;
                sqlcount = "select count(1) " + sqltable.Replace("into #Temporary", "");
                return sql;
            }
        }

        public static DynamicParameters GetDynamicParameter(ref string sqlWhere, Parameter[] pars)
        {

            if (pars == null || pars.Length == 0)
            {
                sqlWhere = string.Empty;
                return null;
            }
            DynamicParameters dp = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Parameter item in pars)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Like:
                        if (item.Value.ToString().Replace("%", "").Length == 0)
                            item.Value = item.Value.ToString().Replace("%", "");
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.LeftLike:
                        if (item.Value.ToString().Replace("%", "").Length == 0)
                            item.Value = item.Value.ToString().Replace("%", "");
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value + "%");
                        break;
                    case OperateType.RightLike:
                        if (item.Value.ToString().Replace("%", "").Length == 0)
                            item.Value = item.Value.ToString().Replace("%", "");
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value);
                        break;
                    case OperateType.NotLike:
                        if (item.Value.ToString().Replace("%", "").Length == 0)
                            item.Value = item.Value.ToString().Replace("%", "");
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.In:
                        Array arr = item.Value as Array;
                        if (arr != null)
                            sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr));
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                            sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr1));
                        break;
                    case OperateType.SqlFormat:
                        object[] arr2 = item.Value as object[];
                        if (arr2 != null)
                            sb.AppendFormat(item.Name, ArrayToString(arr2));
                        else
                            sb.AppendFormat(item.Name, item.Value);
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr3 = item.Value as object[];
                        if (arr3 != null)
                        {
                            string[] ps = new string[arr3.Length];
                            for (int i = 0; i < arr3.Length; i++)
                            {
                                ps[i] = "@p_" + index.ToString();
                                dp.Add("@p_" + index.ToString(), arr3[i]);
                                index++;
                            }
                            sb.AppendFormat(item.Name, ps);
                        }
                        else
                        {
                            sb.AppendFormat(item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    default:
                        break;
                }
                index++;
            }
            sqlWhere = sb.ToString();
            return dp;
        }

        /// <summary>
        /// 把Parameter参数数组转换成DynamicParameters参数，并且输出sqlWhere语句（不包含【WHERE】关键字）
        /// </summary>
        /// <param name="sqlWhere">不包含【WHERE】和【WHERE 1=1】的sqlWhere语句</param>
        /// <param name="sqlJoin">sql中join 语句</param>
        /// <param name="pars">Parameter参数数组</param>
        /// <returns>DynamicParameters参数</returns>
        public static DynamicParameters GetDynamicParameter(ref string sqlWhere, out string sqlJoin, Parameter[] pars)
        {
            StringBuilder sbJoin = new StringBuilder();
            int Count = 0;
            sqlJoin = string.Empty;
            if (pars == null || pars.Length == 0)
            {
                sqlWhere = string.Empty;
                return null;
            }
            DynamicParameters dp = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Parameter item in pars)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Like:
                        if (item.Value == null)
                        {
                            break;
                        }
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        item.Value = item.Value.ToString().Replace("%", "");
                        if (item.Value.ToString().Length == 0)
                        {
                            dp.Add("@p_" + index.ToString(), $"{item.Value}");
                            break;
                        }

                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.LeftLike:
                        if (item.Value == null)
                        {
                            break;
                        }
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        item.Value = item.Value.ToString().Replace("%", "");

                        dp.Add("@p_" + index.ToString(), item.Value + "%");
                        break;
                    case OperateType.RightLike:
                        if (item.Value == null)
                        {
                            break;
                        }
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        item.Value = item.Value.ToString().Replace("%", "");

                        dp.Add("@p_" + index.ToString(), "%" + item.Value);
                        break;
                    case OperateType.NotLike:
                        if (item.Value == null)
                        {
                            break;
                        }
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        item.Value = item.Value.ToString().Replace("%", "");
                        if (item.Value.ToString().Length == 0)
                        {
                            dp.Add("@p_" + index.ToString(), $"{item.Value}");
                            break;
                        }

                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.In:
                        Count++;
                        Array arr = item.Value as Array;
                        if (arr != null)
                        {
                            sbJoin.AppendFormat(" join dbo.FunTable({0}) Tast{1} on Tast{1}.strcode={2}", "@p_" + index.ToString(), Count, item.Name);
                            dp.Add("@p_" + index.ToString(), ArrayRemoveRepeat(arr));
                        }
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Count++;
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                        {
                            sbJoin.AppendFormat(" left join dbo.FunTable({0}) Tast{1} on Tast{1}.strcode={2} ", "@p_" + index.ToString(), Count, item.Name);
                            sb.AppendFormat(" and Tast{0}.strcode  is null ", Count);
                            dp.Add("@p_" + index.ToString(), ArrayRemoveRepeat(arr1));
                        }
                        break;

                    //case OperateType.In:
                    //    Array arr = item.Value as Array;
                    //    if (arr != null)
                    //        sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr));
                    //    else
                    //        sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                    //    break;
                    //case OperateType.NotIn:
                    //    Array arr1 = item.Value as Array;
                    //    if (arr1 != null)
                    //        sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr1));
                    //    break;
                    case OperateType.SqlFormat:
                        object[] arr2 = item.Value as object[];
                        if (arr2 != null)
                            sb.AppendFormat(item.Name, ArrayToString(arr2));
                        else
                            sb.AppendFormat(item.Name, item.Value);
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr3 = item.Value as object[];
                        if (arr3 != null)
                        {
                            string[] ps = new string[arr3.Length];
                            for (int i = 0; i < arr3.Length; i++)
                            {
                                ps[i] = "@p_" + index.ToString();
                                dp.Add("@p_" + index.ToString(), arr3[i]);
                                index++;
                            }
                            sb.AppendFormat(item.Name, ps);
                        }
                        else
                        {
                            sb.AppendFormat(item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    default:
                        break;
                }
                index++;
            }
            sqlWhere = sb.ToString();
            sqlJoin = sbJoin.ToString();
            return dp;
        }

        public static string ArrayRemoveRepeat(Array arr)
        {

            // string[] arrlist = arrValue.Trim().Replace(" ","").Split(',');
            //  List<string> list = new List<string>(arr);

            List<string> list = new List<string>();
            foreach (var s in arr)
            {
                list.Add(s.ToString());
            }
            list = list.Distinct().ToList();
            return string.Join(",", list);
        }

        public static string ArrayToString(Array arr)
        {
            if (arr.GetLength(0) == 0)
                return string.Empty;

            object o = arr.GetValue(0);
            string[] str = new string[arr.GetLength(0)];
            if (o is int || o is decimal || o is double || o is float || o is long)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString();
                }
                return string.Join(",", str);
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString().Replace("'", "''");
                }
                return "'" + string.Join("','", str) + "'";
            }
        }

    }
}
