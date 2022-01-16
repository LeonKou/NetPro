using XXX.Plugin.MongoDB.Service;

namespace XXX.Plugin.MongoDB
{
    /// <summary>
    /// Redis 操作示例
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MongoDBDemoController : ControllerBase
    {
        private readonly ILogger<MongoDBDemoController> _logger;
        private readonly IWebHelper _webHelper;
        private readonly IMongoDBDemoService _mongoDBService;

        public MongoDBDemoController(ILogger<MongoDBDemoController> logger,
            IWebHelper webHelper,
            IMongoDBDemoService mongoDBService)
        {
            _logger = logger;
            _webHelper = webHelper;
            _mongoDBService = mongoDBService;
        }

        /// <summary>
        /// 通过数据库别名key新增到指定数据库
        /// </summary>
        /// <param name="key">数据库别名key</param>
        /// <returns></returns>
        [HttpPost("InsertOne")]
        [ProducesResponseType(200)]
        public IActionResult InsertOne([FromQuery] string key = "mongo1")
        {
            _mongoDBService.InsertOne(key);
            return Ok();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">数据库别名key</param>
        /// <returns></returns>
        [HttpDelete("DeleteOne")]
        [ProducesResponseType(200)]
        public IActionResult DeleteOne([FromQuery] string key = "mongo1")
        {
            _mongoDBService.DeleteOne(key);
            return Ok();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="key">数据库别名key</param>
        /// <returns></returns>
        [HttpPost("ReplaceOne")]
        [ProducesResponseType(200)]
        public IActionResult ReplaceOne([FromQuery] string key = "mongo1")
        {
            _mongoDBService.ReplaceOne(key);
            return Ok();
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="key">数据库别名key</param>
        /// <returns></returns>
        [HttpGet("Find")]
        [ProducesResponseType(200)]
        public IActionResult Find(string key = "mongo1")
        {
            _mongoDBService.Find(key);
            return Ok();
        }
    }
}
