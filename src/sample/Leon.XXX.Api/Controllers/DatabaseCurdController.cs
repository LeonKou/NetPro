using AutoMapper;
using FreeSql.Internal.Model;
using Leon.XXX.Domain;
using Leon.XXX.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetPro.ResponseCache;
using NetPro.Sign;
using NetPro.Web.Core.Filters;
using NetPro.Web.Core.Models;
using System.Threading.Tasks;
using System.Web;

namespace Leon.XXX.Api
{
    /// <summary>
    /// 数据库CURD范例
    /// 路由推荐规则:
    /// api/项目名称/接口版本
    /// </summary>
    /// <remarks>
    /// 推荐路由好处：所有项目使用同一域名的情况下根据项目名称确定实际地址，方便管理域；
    /// 版本号可在不停服情况下更新版本并做到兼容老版本
    /// </remarks>
    [Route("api/microservice/v1/[controller]")]
    public class DatabaseCurdController : ControllerBase
    {
        private readonly IDataBaseOptionService _dataBaseOptionService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="dataBaseOptionService"></param>
        /// <param name="mapper"></param>
        public DatabaseCurdController(
            ILogger<DatabaseCurdController> logger
            , IDataBaseOptionService dataBaseOptionService
            , IMapper mapper)
        {
            _logger = logger;
            _dataBaseOptionService = dataBaseOptionService;
            _mapper = mapper;
        }

        /// <summary>
        /// 增加实体；结果缓存3秒
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]//swagger
        [PostResponseCache(Duration = 3, IgnoreVaryByQueryKeys = new[] { "createtime" })]
        public async Task<IActionResult> AddAsync(XXXAo xXXAo)
        {
            var result = await _dataBaseOptionService.AddAsync(_mapper.Map<XXXDo>(xXXAo));
            //Code等于0才是预期，否则都应该提示
            if (result.Code == 0)
            {
                _logger.LogWarning($"[AddAsync]增加实体脱离预期值----Code:{result.Code}---msg:{result.Msg}");
                return BadRequest(new ResponseResult { Result = result, Code = result.Code, Msg = result.Msg });
            }
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 根据主键id删除，也可根据复杂条件删除；结果缓存5秒
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        [PostResponseCache(Duration = 5)]
        public async Task<IActionResult> DeleteAsync(uint id)
        {
            var result = await _dataBaseOptionService.DeleteAsync(id);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 更新整个实体，也可更新多个值而不是整个实体；结果缓存1秒
        /// </summary>
        /// <returns></returns>
        [HttpPost("update")]
        [PostResponseCache(Duration = 1)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> UpdateAsync(XXXAo xXXAo)
        {
            var result = await _dataBaseOptionService.UpdateAsync(xXXAo);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 更新单个值；结果缓存2秒 
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [PostResponseCache(Duration = 2)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> UpdateSetAsync(string singleValue)
        {
            var result = await _dataBaseOptionService.UpdateSetAsync(singleValue);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 插入或者更新整个实体;结果缓存1秒
        /// </summary>
        /// <returns></returns>
        [HttpPost("insertorupdate")]
        [PostResponseCache(Duration = 1)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> InsertOrUpdateAsync([FromBody]XXXAo xXXAo)
        {
            var result = await _dataBaseOptionService.InsertOrUpdateAsync(xXXAo);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 根据主键id查找；结果缓存60秒
        /// </summary>
        /// <returns></returns>
        [HttpGet("find")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "id" })]
        public async Task<IActionResult> FindAsync([FromQuery]uint id)
        {
            var result = await _dataBaseOptionService.FindAsync(id);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 动态条件查询，由客户端自由组织查询条件类似jira条件查询；结果缓存30秒
        /// </summary>
        /// <returns></returns>
        [HttpGet("dynamicquery")]
        [ResponseCache(Duration = 30)]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> DynamicQueryAsync([FromQuery]DynamicFilterInfo dynamicFilter)
        {
            var result = await _dataBaseOptionService.DynamicQueryAsync(dynamicFilter);
            return Ok(new ResponseResult { Result = result });
        }

        /// <summary>
        /// 生成签名 
        /// </summary>
        /// <returns></returns>
        [HttpGet("createsign")]
        [ResponseCache(Duration = 30)]
        public IActionResult CreateSign()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["appid"] = "sadfsdf";       //必传 应用id

            var sign = SignCommon.CreateSign("sadfsdf", query: query, body: new { a = 1, b = "1" });
            query.Add("sign", sign);
            return Ok(sign);
        }
    }
}
