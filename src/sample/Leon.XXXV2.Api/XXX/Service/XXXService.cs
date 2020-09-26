using AutoMapper;

namespace Leon.XXXV2.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXService : IXXXService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rankRepository"></param>
        /// <param name="mapper"></param>
        public XXXService(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XXXAo GetList()
        {
            return new XXXAo();
            //return _mapper.Map<XXXAo>(xxxdo.First());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetFalse(string name)
        {
            return true;
        }
    }
}
