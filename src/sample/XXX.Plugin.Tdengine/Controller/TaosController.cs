using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using XXX.Plugin.Tdengine.Proxy;

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
        /// 数据插入
        /// </summary>
        /// <param name="taosAo"></param>
        /// <param name="dbKey"></param>
        /// <returns></returns>
        [HttpPut("insertbyrestful")]
        [ProducesResponseType(200, Type = typeof(ResponseResult))]
        public async Task<IActionResult> InsertByRestfulAsync(TaosAo taosAo, string dbKey = "taos1")
        {
            //var command = @"show databases";
            var command = @$"
                                 INSERT INTO  {taosAo.DeviceId} 
                                 USING {"meters"} TAGS (mongo, 67) 
                                 values ( 1608173534840 2 false 'Channel1.窑.烟囱温度' '烟囱温度' '122.00' );";

            var result = await _taosProxy.ExecuteSql(command);
            return Ok(result);
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
