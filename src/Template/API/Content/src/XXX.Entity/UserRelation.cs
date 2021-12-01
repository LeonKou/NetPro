namespace XXX.Entity
{
    /// <summary>
    /// 双主键实体示例
    /// 双主键，每个列都加 [Column(IsPrimary = true)]即可
    /// </summary>
    public class UserRelation
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Column(IsPrimary = true)]
        public int UserId { get; set; }

        /// <summary>
        /// 公司id
        /// </summary>
        [Column(IsPrimary = true)]
        public int CompanyId { get; set; }
    }
}
