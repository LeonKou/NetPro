namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// 查询条件类
    /// </summary>
    public class UserSearchAo : SearchPageBase
    {
        public int Age { get; set; }
    }

    /// <summary>
    /// 插入的实体除了主键应该包含其他所有列
    /// </summary>
    public class UserInsertAo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Pwd { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public uint Age { get; set; }

        /// <summary>
        /// 创建时间,数据库时间都应该以时间戳表示，除了确保唯一性的场景其他秒单位足够
        /// </summary>
        public long CreateTime { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

        /// <summary>
        /// 企业Id主键
        /// </summary>
        public int CompanyId { get; set; }
    }
    /// <summary>
    /// 更新实体
    /// </summary>
    public class UserUpdateAo : UserInsertAo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }

    }
}
