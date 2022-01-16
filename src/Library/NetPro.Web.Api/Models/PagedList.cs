using System.Collections.Generic;
using System.Linq;

namespace System.NetPro
{
    /// <summary>
    /// 分页实现
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    public class PagedList<T>
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
        public long TotalCount { get; set; }
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
        /// 初始化分页的新实例
        /// </summary>
        /// <param name="source">数据源.</param>
        /// <param name="pageIndex">当前页码.</param>
        /// <param name="pageSize">显示条数</param>
        ///  <param name="totalCount">总数</param>
        /// <param name="indexFrom">起始页码.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, long totalCount = 0, int indexFrom = 1)
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

                Items = querable.ToList();
            }
            else
            {
                PageIndex = pageIndex;
                PageSize = pageSize;
                IndexFrom = indexFrom;
                TotalCount = totalCount;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

                Items = source.ToList();
            }
        }
    }


}
