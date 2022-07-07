using Newtonsoft.Json.Converters;

namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// 查询条件类
    /// </summary>
    public class LogSearchAo : SearchPageBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long? EndTime { get; set; }
    }

    /// <summary>
    /// 插入的实体除了主键应该包含其他所有列
    /// </summary>
    public class LogInsertAo
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }
    }
    /// <summary>
    /// 更新实体
    /// </summary>
    public class LogUpdateAo 
    {
        /// <summary>
        /// 日志Id
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

    }

    /// <summary>
    /// 更新分表日志实体
    /// </summary>
    public class LogUpdatePatchAo
    {
        /// <summary>
        /// 日志Id列表
        /// </summary>
        [Required]
        public List<string> Ids { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

    }

    public class CreateLogTable
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTime { get; set; }
    }
}
