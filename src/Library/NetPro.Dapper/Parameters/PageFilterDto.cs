using System.Collections.Generic;

namespace NetPro.Dapper
{
    public class PageFilterDto
    {
        /// <summary>
        /// 查询条件
        /// </summary>
        public IEnumerable<FilterDto> Filters { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public Dictionary<string, string> Sorts { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        public class FilterDto
        {
            /// <summary>
            /// 谓词
            /// </summary>
            public FilterTypeEnum FilterType { get; set; }

            /// <summary>
            /// 字段名称
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// 字段值
            /// </summary>
            public string Value { get; set; }

            public enum FilterTypeEnum
            {
                /// <summary>
                /// 不等于
                /// </summary>
                UnEqual = 0,

                /// <summary>
                /// 等于 
                /// =
                /// </summary>
                Equal = 1,

                /// <summary>
                /// 大于等于 
                /// >=
                /// </summary>
                Greater = 2,

                /// <summary>
                /// 小于等于 
                /// <![CDATA[ <=]]>
                /// </summary>
                Less = 3,

                /// <summary>
                /// 模糊
                /// like  
                /// </summary>
                Like = 4,

                /// <summary>
                /// in  
                /// 包含查询
                /// </summary>
                In = 5,

                /// <summary>
                /// 取 Top
                /// </summary>
                TopIn = 6,

                /// <summary>
                /// 不包含
                /// </summary>
                NotIn = 7,

                ///// <summary>
                ///// 或
                ///// </summary>
                //Or = 9
            }
        }
    }
}
