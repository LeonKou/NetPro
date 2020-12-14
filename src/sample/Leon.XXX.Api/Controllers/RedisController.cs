using AutoMapper;
using Leon.XXX.Domain;
using Leon.XXX.Domain.XXX.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.ResponseCache;
using NetPro.Web.Core.Filters;
using NetPro.Web.Core.Models;
using System.Threading.Tasks;

namespace Leon.XXX.Api
{
    /// <summary>
    /// Redis范例
    /// 路由推荐规则:
    /// api/项目名称/接口版本
    /// </summary>
    /// <remarks>
    /// 推荐路由好处：所有项目使用同一域名的情况下根据项目名称确定实际地址，方便管理域；
    /// 版本号可在不停服情况下更新版本并做到兼容老版本
    /// </remarks>
    [Route("api/microservice/v1/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisOptionService _redisOptionService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="redisOptionService"></param>
        /// <param name="mapper"></param>
        public RedisController(
            ILogger<DatabaseCurdController> logger
            , IRedisOptionService redisOptionService
            , IMapper mapper)
        {
            _logger = logger;
            _redisOptionService = redisOptionService;
            _mapper = mapper;
        }

        /// <summary>
        /// 插入实体，响应缓存1秒
        /// </summary>
        /// <returns></returns>
        [HttpPost("set")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]//swagger
        [PostResponseCache(Duration = 1, IgnoreVaryByQueryKeys = new[] { "createtime" })]
        public async Task<IActionResult> SetAsync(XXXAo xXXAo)
        {
            var result = await _redisOptionService.SetAsync(1, xXXAo);
            //Code等于0才是预期，否则都应该提示
            if (result.Code == 0)
            {
                _logger.LogWarning($"[AddAsync]插入实体，id作为key主要标识，过期时间60秒----Code:{result.Code}---msg:{result.Msg}");
                return BadRequest(new ResponseResult { Result = result, Code = result.Code, Msg = result.Msg });
            }
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        ///  根据key删除缓存,5秒只能删除同一个id一次(有响应缓存)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        [PostResponseCache(Duration = 5)]
        public async Task<IActionResult> RemoveAsync(uint id)
        {
            var result = await _redisOptionService.RemoveAsync(id);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        ///  根据key查询redis，不存在返回null
        /// </summary>
        /// <returns></returns>
        [HttpGet("redis")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> GetAsync(uint id)
        {
            var result = await _redisOptionService.GetAsync(id);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 根据key查询redis，不存在则查询数据库并将结果插入Redis
        /// </summary>
        /// <returns></returns>
        [HttpGet("getorcreate")]
        [PostResponseCache(Duration = 2)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> GetOrCreateAsync(uint id)
        {
            var result = await _redisOptionService.GetOrCreateAsync(id);
            return Ok(new ResponseResult { Result = result });
        }

    }
}
