namespace System.NetPro
{
    /// <summary>
    /// 查询分页请求接口基类
    /// </summary>
    public class SearchPageBase
    {
        /// <summary>
        /// 页码
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页数量
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; } = 10;
    }
}
