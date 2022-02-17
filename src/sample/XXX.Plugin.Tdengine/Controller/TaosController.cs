using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

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
        private readonly ITaosProxy _taosProxy;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="taosService"></param>
        /// <param name="taosProxy"></param>
        /// <param name="httpClientFactory"></param>
        public TaosController(
            IHostEnvironment hostEnvironment,
            ILogger<TaosController> logger,
            ITaosService taosService,
            ITaosProxy taosProxy,
            IHttpClientFactory httpClientFactory
            )
        {
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _taosService = taosService;
            _taosProxy = taosProxy;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 数据插入方便测试demo
        /// </summary>
        /// <returns></returns>
        [HttpGet("inserttest")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> InsertAsync()
        {
            await _taosService.InsertTestAsync();
            return Ok();
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        /// <param name="taosAo"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        [HttpPut("insert")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> InsertAsync(TaosAo taosAo, string dbKey = "taos1")
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
