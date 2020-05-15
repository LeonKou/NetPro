using AutoMapper;
using Dapper;
using Leon.XXX.Repository;
using System.Linq;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXService : IXXXService
    {
        private readonly IXXXRepository _rankRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rankRepository"></param>
        /// <param name="mapper"></param>
        public XXXService(IXXXRepository rankRepository
            , IMapper mapper)
        {
            _rankRepository = rankRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XXXAo GetList()
        {
            var xxxdo = _rankRepository.GetList<XXXDo>("", new DynamicParameters());
            return _mapper.Map<XXXAo>(xxxdo.First());
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
