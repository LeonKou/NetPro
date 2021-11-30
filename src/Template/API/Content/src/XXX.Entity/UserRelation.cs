namespace XXX.Entity
{
    public class UserRelation
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 公司id
        /// </summary>
        public int CompanyId { get; set; }
    }
}
