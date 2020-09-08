using NetPro.Web.Core.PagedList;
using System.Collections.Generic;

namespace NetPro.Web.Core.Models
{
    /// <summary>
    /// api分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedListModel<T>
    {
        public PagedListModel()
        {

        }

        public PagedListModel(IPagedList<T> paged)
        {
            this.Items = paged.Items;
            this.PageIndex = paged.PageIndex;
            this.PageSize = paged.PageSize;
            this.TotalCount = paged.TotalCount;
            this.TotalPages = paged.TotalPages;
            this.IndexFrom = paged.IndexFrom;
        }

        public PagedListModel(ICollection<T> items, int pageIndex, int pageSize, int totalCount, int totalPages)
        {
            this.Items = items;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.TotalPages = totalPages;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get;  set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get;  set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get;  set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get;  set; }

        /// <summary>
        /// Gets the index from.
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public ICollection<T> Items { get; set; }
    }
}
