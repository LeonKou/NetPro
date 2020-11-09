using NetPro.RedisManager;
using NetPro.Web.Core.Models;
using System.Threading.Tasks;

namespace Leon.XXX.Domain.XXX.Service
{
    /// <summary>
    /// Redis 简单操作操作范例
    /// </summary>
    public class RedisOptionService : IRedisOptionService
    {
        private readonly IRedisManager _redisManager;
        private readonly IDataBaseOptionService _dataBaseOptionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redisManager"></param>
        /// <param name="dataBaseOptionService"></param>
        public RedisOptionService(
            IRedisManager redisManager
            , IDataBaseOptionService dataBaseOptionService)
        {
            _redisManager = redisManager;
            _dataBaseOptionService = dataBaseOptionService;
        }

        /// <summary>
        ///  查询一个key，不存在返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseResult<XXXAo>> GetAsync(uint id)
        {
            var result = await _redisManager.GetAsync<XXXAo>($"RedisOption:Id{id}");
            return new ResponseResult<XXXAo>
            {
                Result = result,
                Code = 0,
                Msg = "查询成功"
            };
        }

        /// <summary>
        /// 根据key查询redis，不存在则查询数据库并将结果插入Redis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseResult<XXXAo>> GetOrCreateAsync(uint id)
        {
            var result = await _redisManager.GetOrSetAsync<XXXAo>($"RedisOption:Id{id}", async () =>
            {
                var @result = await _dataBaseOptionService.FindAsync(id);
                return @result.Result;
            });

            return new ResponseResult<XXXAo>
            {
                Result = result,
            };
        }

        /// <summary>
        /// 根据key删除缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseResult<bool>> RemoveAsync(uint id)
        {
            var result = await _redisManager.RemoveAsync($"RedisOption:Id{id}");
            return new ResponseResult<bool>
            {
                Result = result,
            };
        }

        /// <summary>
        /// 插入实体，id作为key主要标识，过期时间60秒
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xXXAo"></param>
        /// <returns></returns>
        public async Task<ResponseResult<bool>> SetAsync(uint id, XXXAo xXXAo)
        {
            var result = await _redisManager.SetAsync($"RedisOption:Id{id}", xXXAo);
            return new ResponseResult<bool>
            {
                Result = result,
            };
        }
    }
}

namespace Leon.XXX.Domain.XXX.Service
{
    /// <summary>
    /// Redis 简单操作操作范例
    /// </summary>
    public interface IRedisOptionService
    {
        /// <summary>
        /// 插入实体，id作为key主要标识，过期时间60秒
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xXXAo"></param>
        /// <returns></returns>
        Task<ResponseResult<bool>> SetAsync(uint id, XXXAo xXXAo);

        /// <summary>
        /// 根据key删除缓存
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseResult<bool>> RemoveAsync(uint id);

        /// <summary>
        ///  根据key查询redis，不存在返回null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseResult<XXXAo>> GetAsync(uint id);

        /// <summary>
        /// 根据key查询redis，不存在则查询数据库并将结果插入Redis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseResult<XXXAo>> GetOrCreateAsync(uint id);
    }
}
