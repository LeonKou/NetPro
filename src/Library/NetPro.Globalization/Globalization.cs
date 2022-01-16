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
    }
}
