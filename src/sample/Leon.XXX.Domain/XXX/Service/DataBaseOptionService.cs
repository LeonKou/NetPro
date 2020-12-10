using AutoMapper;
using FreeSql.Internal.Model;
using Leon.XXX.Repository;
using NetPro.Web.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class DataBaseOptionService : IDataBaseOptionService
    {
        private readonly IXXXRepository _xxxRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xxxRepository"></param>
        /// <param name="mapper"></param>
        public DataBaseOptionService(
            IXXXRepository xxxRepository
            , IMapper mapper)
        {
            _xxxRepository = xxxRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xXXDo"></param>
        /// <returns></returns>
        public async Task<ResponseResult<XXXDo>> AddAsync(XXXDo xXXDo)
        {
            var result = await Task.FromResult(new ResponseResult<XXXDo> { Result = _xxxRepository.Get(1) });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseResult> DeleteAsync(uint id)
        {
            var resutl = await _xxxRepository.GetAsync(1);
            return new ResponseResult { Result = resutl };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicFilter"></param>
        /// <returns></returns>
        public async Task<ResponseResult<IList<XXXAo>>> DynamicQueryAsync(DynamicFilterInfo dynamicFilter)
        {
            return await Task.FromResult(new ResponseResult<IList<XXXAo>> { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseResult<XXXAo>> FindAsync(uint id)
        {
            return await Task.FromResult(new ResponseResult<XXXAo> { });
        }

        /// <summary>
        ///  根据主键id先从redis查找，查找不到自动到数据库查找并插入到Redis中
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult<XXXAo>> FindByRedisAsync(uint id)
        {
            return await Task.FromResult(new ResponseResult<XXXAo> { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xXXAo"></param>
        /// <returns></returns>
        public async Task<ResponseResult> InsertOrUpdateAsync(XXXAo xXXAo)
        {
            return await Task.FromResult(new ResponseResult { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xXXAo"></param>
        /// <returns></returns>
        public async Task<ResponseResult> UpdateAsync(XXXAo xXXAo)
        {
            return await Task.FromResult(new ResponseResult { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleValue"></param>
        /// <returns></returns>
        public async Task<ResponseResult> UpdateSetAsync(string singleValue)
        {
            return await Task.FromResult(new ResponseResult { });
        }
    }
}
