namespace XXX.Entity
{
    public class Company
    {
        [Column(IsIdentity = true, IsPrimary = true, MapType = typeof(int))]
        public int Id { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Region { get; set; }
    }
}
