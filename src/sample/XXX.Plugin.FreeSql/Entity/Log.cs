namespace XXX.Entity
{
    [JsonObject(MemberSerialization.OptIn), Table(Name = "log_{yyyyMM}", AsTable = "CreateTime=2022-1-1(1 month)")]
    public class Log
    {
        [Column(IsIdentity = true)]
        public string Id { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; } = DateTimeOffset.UtcNow.DateTime;
    }
}