namespace XXX.Plugin.Tdengine
{
    /// <summary>
    /// 一个采集点位一张表
    /// </summary>
    public class TaosAo
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        ///时间戳 毫秒
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Att[] att { get; set; }
    }

    /// <summary>
    /// 子项
    /// </summary>
    public class Att
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数编码
        /// </summary>
        public string ParameterCode { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public string DataType { get; set; }
    }


    /// <summary>
    /// 创建数据库配置
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 保留天数(单位:天)
        /// </summary>
        public int Keep { get; set; } = 100;

        /// <summary>
        /// 文件的创建周期(单位:天)
        /// </summary>
        /// <remarks>每10天一个周期</remarks>
        public int Days { get; set; } = 10;

        /// <summary>
        /// 块数
        /// </summary>
        /// <remarks>内存块数为 6</remarks>
        public int Blocks { get; set; } = 6;

        /// <summary>
        /// 是否允许更新
        /// </summary>
        public byte AllowUpdate { get; set; } = 0;
    }

    /// <summary>
    /// 超级表
    /// </summary>
    public class SuperTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; } = "table_name";

        /// <summary>
        /// 时间戳名称
        /// </summary>
        public string TimestampName { get; set; } = "timestamp";

        /// <summary>
        /// 物理量
        /// </summary>
        public string[] Fields { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public string[] Tags { get; set; }
    }
}
