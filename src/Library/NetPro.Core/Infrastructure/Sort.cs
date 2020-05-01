namespace NetPro.Core.Infrastructure
{
    /// <summary>
    /// 排序对象
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// 排序字段名称
        /// </summary>

        public string Field { get; set; }

        /// <summary>
        /// 排序类型 降序 desc 升序 asc
        /// </summary>

        public string Dir { get; set; }

        /// <summary>
        /// 表达式组合 如: id desc
        /// </summary>
        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }
}
