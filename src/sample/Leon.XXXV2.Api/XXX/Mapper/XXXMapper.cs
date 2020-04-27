using AutoMapper;
using NetPro.Core.Infrastructure.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
