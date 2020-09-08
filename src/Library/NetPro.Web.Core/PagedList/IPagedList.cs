using System.Collections.Generic;

namespace NetPro.Web.Core.PagedList
{
    /// <summary>
    /// 为分页集合提供接口
    /// </summary>
    /// <typeparam name="T">The type for paging.</typeparam>
    public interface IPagedList<T>
    {
        /// <summary>
        /// 起始页码数
        /// </summary>
        /// <value>The index from.</value>
        int IndexFrom { get; }
        /// <summary>
        /// 页码
        /// </summary>
        /// <value>The index of the page.</value>
        int PageIndex { get; }
        /// <summary>
        /// 每页数量
        /// </summary>
        /// <value>The size of the page.</value>
        int PageSize { get; }
        /// <summary>
        /// 总条数
        /// </summary>
        /// <value>The total count.</value>
        int TotalCount { get; }
        /// <summary>
        /// 总页数
        /// </summary>
        /// <value>The total pages.</value>
        int TotalPages { get; }
        /// <summary>
        /// 子集合
        /// </summary>
        /// <value>The items.</value>
        IList<T> Items { get; }
        /// <summary>
        /// 是否还有上一页
        /// </summary>
        /// <value>The has next page.</value>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否还有下一页
        /// </summary>
        /// <value>The has next page.</value>
        bool HasNextPage { get; }
    }
}
