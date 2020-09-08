using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPro.Web.Core.PagedList
{
    /// <summary>
    /// 分页实现
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        /// 页码
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages { get; set; }
        /// <summary>
        /// 起始页码数
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; set; }

        /// <summary>
        /// 子集合
        /// </summary>
        /// <value>The items.</value>
        public IList<T> Items { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// 是否还有下一页
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;


        /// <summary>
        /// 初始化分页的新实例
        /// </summary>
        /// <param name="source">数据源.</param>
        /// <param name="pageIndex">当前页码.</param>
        /// <param name="pageSize">显示条数</param>
        /// <param name="indexFrom">起始页码.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom = 0)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            if (source is IQueryable<T> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = querable.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                Items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = source.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
            }
        }

        /// <summary>
        /// 初始化分页实例
        /// </summary>
        public PagedList() => Items = new T[0];
    }


    /// <summary>
    /// 提供集合转换的
    /// </summary>
    /// <typeparam name="TSource">数据源.</typeparam>
    /// <typeparam name="TResult">返回结果集.</typeparam>
    public class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; }
        /// <summary>
        /// 每页数量
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; }
        /// <summary>
        /// 总数
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; }
        /// <summary>
        /// 总页数
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages { get; }
        /// <summary>
        /// 起始页码
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; }

        /// <summary>
        /// 子集合
        /// </summary>
        /// <value>The items.</value>
        public IList<TResult> Items { get; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// 是否还有下一页
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        /// <summary>
        ///初始化分页实例
        /// </summary>
        /// <param name="source">数据源.</param>
        /// <param name="converter">转换函数体</param>
        /// <param name="pageIndex">页码.</param>
        /// <param name="pageSize">每页显示数量.</param>
        /// <param name="indexFrom">起始页码</param>
        public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom = 0)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            if (source is IQueryable<TSource> querable)
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = querable.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                var items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = source.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

                Items = new List<TResult>(converter(items));
            }
        }

        /// <summary>
        /// 初始化分页实例
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="converter">转换函数体</param>
        public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = new List<TResult>(converter(source.Items));
        }
    }

    /// <summary>
    /// 分页帮助类
    /// </summary>
    public static class PagedList
    {
        /// <summary>
        /// 创建空的分页实例
        /// </summary>
        /// <typeparam name="T">The type for paging </typeparam>
        /// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
        public static IPagedList<T> Empty<T>() => new PagedList<T>();
        /// <summary>
        /// 创建一个从数据源到结果集的分页实例
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <typeparam name="TSource">数据源类型.</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="converter">转换函数体</param>
        /// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
        public static IPagedList<TResult> From<TResult, TSource>(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);
    }
}
