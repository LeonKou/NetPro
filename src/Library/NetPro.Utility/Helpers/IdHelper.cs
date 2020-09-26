using IdGen;

namespace NetPro.Utility.Helpers
{
    /// <summary>
    /// Id生成器
    /// </summary>
    public static class IdHelper
    {
        /// <summary>
        /// Id
        /// </summary>
        private static string _id;

        /// <summary>
        /// 设置Id
        /// </summary>
        /// <param name="id">Id</param>
        public static void SetId(string id)
        {
            _id = id;
        }

        /// <summary>
        /// 重置Id
        /// </summary>
        public static void Reset()
        {
            _id = null;
        }

        /// <summary>
        /// 创建Id
        /// </summary>
        public static string ObjectId()
        {
            return string.IsNullOrWhiteSpace(_id) ? Internal.ObjectId.GenerateNewStringId() : _id;
        }

        /// <summary>
        /// 用Guid创建Id,去掉分隔符
        /// </summary>
        public static string Guid()
        {
            return string.IsNullOrWhiteSpace(_id) ? System.Guid.NewGuid().ToString("N") : _id;
        }

        /// <summary>
        /// 推特Snowflake算法C#版
        /// 详情了解https://github.com/RobThree/IdGen
        /// </summary>
        /// <returns></returns>
        public static long NewId()
        {
            //如id压力增大，后续移至NetPro.Core中,apollo配置

            var generator = new IdGenerator(0);

            return generator.CreateId();
        }
    }
}
