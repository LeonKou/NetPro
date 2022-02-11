using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.NetPro
{
    public static partial class Extensions
    {
        /// <summary>
        /// 分页：起始行数超出总行数一半倒序读取数据提升查询速度
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex">起始行数</param>
        /// <param name="pageSize">每页行数</param>
        /// <param name="totalRowNum">总行数</param>
        /// <param name="orderExpression">排序条件</param>
        /// <returns></returns>
        public static IQueryable<TSource> TakeByRowNum<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize, int totalRowNum, Expression<Func<TSource, long>> orderExpression)
        {
            var skipRowNum = pageIndex * pageSize;
            if (skipRowNum > totalRowNum / 2)
            {
                skipRowNum = totalRowNum - skipRowNum;
                source.OrderBy(orderExpression);
            }
            else
            {
                source.OrderByDescending(orderExpression);
            }
            return source.Skip(skipRowNum).Take(pageSize);
        }
        /// <summary>
        /// 若judgement为真, 执行Where
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="judgement"></param>
        /// <param name="whereExpression">要执行的Where语句</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIfTrue<TSource>(this IQueryable<TSource> source, bool judgement, Expression<Func<TSource, bool>> whereExpression)
        {
            return judgement ? source.Where(whereExpression) : source;
        }
        /// <summary>
        /// 若judgement为真, 执行Where
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="judgement"></param>
        /// <param name="whereExpression">要执行的Where语句</param>
        /// <returns></returns>
        public static IEnumerable<TSource> WhereIfTrue<TSource>(this IEnumerable<TSource> source, bool judgement, Func<TSource, bool> whereExpression)
        {
            return judgement ? source.Where(whereExpression) : source;
        }
        /// <summary>
        /// 若judgement为空, 或者空白字符串, 则不执行where
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="judgement">判断是否为空的对象</param>
        /// <param name="whereExpression">要执行的Where语句</param>
        /// <returns></returns>
        public static IEnumerable<TSource> WhereIfNotNull<TSource>(this IEnumerable<TSource> source, object judgement, Func<TSource, bool> whereExpression)
        {
            bool isNull = false;
            if (judgement is string)
            {
                isNull = string.IsNullOrWhiteSpace(judgement as string);
            }
            else
            {
                isNull = judgement == null;
            }
            return isNull ? source : source.Where(whereExpression);
        }
        /// <summary>
        /// 若judgement为空, 或者空白字符串, 则不执行where
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="judgement">判断是否为空的对象</param>
        /// <param name="whereExpression">要执行的Where语句</param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIfNotNull<TSource>(this IQueryable<TSource> source, object judgement, Expression<Func<TSource, bool>> whereExpression)
        {
            bool isNull = false;
            if (judgement is string)
            {
                isNull = string.IsNullOrWhiteSpace(judgement as string);
            }
            else
            {
                isNull = judgement == null;
            }
            return isNull ? source : source.Where(whereExpression);
        }
    }
}
