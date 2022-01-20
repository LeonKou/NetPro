using XXX.Plugin.Tdengine;

namespace XXX.Plugin.Tdengine
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TaosController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<TaosController> _logger;
        private readonly ITaosService _taosService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="taosService"></param>
        public TaosController(
            IHostEnvironment hostEnvironment,
            ILogger<TaosController> logger,
            ITaosService taosService
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _taosService = taosService;
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        /// <param name="taosAo"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        [HttpPut("insert")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> Insert(TaosAo taosAo, string dbKey = "taos1")
        {
            await _taosService.InsertAsync(taosAo, dbKey);
            return Ok();
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="databaseConfig"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        [HttpPut("database")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> CreateDatabaseAsync(DatabaseConfig databaseConfig, string dbKey = "taos1")
        {
            await _taosService.CreateDatabaseAsync(databaseConfig, dbKey);
            return Ok();
        }

        /// <summary>
        /// 创建超级表
        /// </summary>
        /// <param name="superTable"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        [HttpPut("table")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> CreateSuperTableAsync(SuperTable superTable, string dbKey = "taos1")
        {
            await _taosService.CreateSuperTableAsync(superTable, dbKey);
            return Ok();
        }
    }
}
