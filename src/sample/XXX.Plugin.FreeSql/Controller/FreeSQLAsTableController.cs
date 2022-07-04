using FreeSql.Internal.Model;

namespace XXX.Plugin.FreeSql
{
    /// <summary>
    /// Freesql数据库分表增删改查实例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FreeSQLAsTableController : ControllerBase
    {
        private readonly IFreeSQLAsTableByDependency _logService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logService"></param>
        public FreeSQLAsTableController(IFreeSQLAsTableByDependency logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 新增分表日志示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPost("insert")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> InsertAsync([FromBody] LogInsertAo input, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _logService.InsertAsync(input, dbKey);           
            return result;
        }

        /// <summary>
        /// 更新日志分表整个对象示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPost("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdateAsync([FromBody] LogUpdateAo input, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _logService.UpdateAsync(input, dbKey);
            return result;
        }


        /// <summary>
        /// 批量更新分表日志单个值示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPatch("update")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> UpdateBatchAsync([FromBody] LogUpdatePatchAo input ,[FromQuery] string dbKey = "sqlite")
        {
            var result = await _logService.UpdateBatchAsync(input, dbKey);
            return result;
        }

        /// <summary>
        /// 批量删除分表日志示例
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpDelete("delete")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> DeleteAsync([FromBody][Required] List<string> ids, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _logService.DeleteAsync(ids, dbKey);
            return result;
        }

        /// <summary>
        /// 分页查询分表日志示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpGet("pagequery")]
        [ProducesResponseType(200, Type = typeof(PagedList<Log>))]
        public async Task<PagedList<Log>> PageQueryAsync([FromQuery] LogSearchAo input, [FromQuery] string dbKey = "sqlite")
        {
            var result = await _logService.PageQueryAsync(input, dbKey);
            return result;
        }

        /// <summary>
        /// 根据传入时间批量创建日志表示例
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbKey">数据库别名标识</param>
        [HttpPost("createtablebytimerange")]
        [ProducesResponseType(200, Type = typeof(string[]))]
        public string[] CreateTableByTimeRange([FromQuery] CreateLogTable input, [FromQuery] string dbKey = "sqlite")
        {
            var result = _logService.CreateTableByTimeRange(input, dbKey);
            return result;
        }

    }
}
