using AutoMapper;
using NetPro.Core.Infrastructure.Mapper;

namespace Leon.XXXV2.Api
{
    public class XXXMapper : Profile, IOrderedMapperProfile
    {
        public XXXMapper()
        {
            //CreateMap<XXXDo, XXXAo>();
        }

        public int Order => 0;
    }
}
