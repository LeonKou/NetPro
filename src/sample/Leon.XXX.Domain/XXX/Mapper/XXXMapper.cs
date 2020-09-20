using AutoMapper;
using Leon.XXX.Repository;
using NetPro.Core.Infrastructure.Mapper;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 数据库实体与AO业务实体互相映射
    /// </summary>
    public class XXXMapper : Profile, IOrderedMapperProfile
    {
        /// <summary>
        /// 
        /// </summary>
        public XXXMapper()
        {
            //数据库实体映射AO业务实体
            CreateMap<XXXDo, XXXAo>();
            //AO业务实体映射数据库实体
            CreateMap<XXXAo, XXXDo>();
        }

        /// <summary>
        /// 映射顺序，默认0即可，无需更改
        /// </summary>
        public int Order => 0;
    }
}
