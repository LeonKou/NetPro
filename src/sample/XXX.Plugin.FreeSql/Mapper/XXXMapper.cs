namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// 实体之间隐射配置
    /// 数据库实体与AO业务实体互相映射
    /// </summary>
    public class UserMapper : Profile, IOrderedMapperProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public UserMapper()
        {
            //数据库实体映射AO业务实体,ReverseMap可实现双向映射
            CreateMap<UserInsertAo, User>().ReverseMap();
            CreateMap<UserUpdateAo, User>().ReverseMap();
            CreateMap<UserInsertAo, UserProfile>().ReverseMap();
        }

        /// <summary>
        /// 映射顺序，默认0即可，无需更改
        /// </summary>
        public int Order => 0;
    }
    public class LogMapper : Profile, IOrderedMapperProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public LogMapper()
        {
            CreateMap<LogInsertAo, Log>().ReverseMap();
            CreateMap<LogUpdateAo, Log>().ReverseMap();
        }

        /// <summary>
        /// 映射顺序，默认0即可，无需更改
        /// </summary>
        public int Order => 0;
    }
}
