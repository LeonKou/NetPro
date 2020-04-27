using AutoMapper;
using Leon.XXX.Repository;
using NetPro.Core.Infrastructure.Mapper;

namespace Leon.XXX.Domain
{
	public class XXXMapper : Profile, IOrderedMapperProfile
	{
		public XXXMapper()
		{
			CreateMap<XXXDo, XXXAo>();
		}

		public int Order => 0;
	}
}
