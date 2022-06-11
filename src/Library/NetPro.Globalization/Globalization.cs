namespace NetPro.Globalization
{
    public class Globalization
    {
        /// <summary>
        /// 多语言数据库连接串
        /// 例如：Data Source=LocalizationRecords.sqlite
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 语言代码
        /// 例如：zh-CN
        /// </summary>
        public string[] Cultures { get; set; }

        /// <summary>
        /// 是否打开注册数据注解本地化服务，默认打开
        /// </summary>
        public bool Annotations { get; set; } = true;

        /// <summary>
        /// 语系不存在是否记录
        /// </summary>
        public bool Record { get; set; } = true;

        /// <summary>
        /// 语言参数名称
        /// </summary>
        public string UIQueryStringKey { get; set; } = "language";
    }
}
