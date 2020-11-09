namespace StackExchange.Redis.Extensions.Core.Configuration
{
    /// <summary>
    /// This class represent the Server enumeration strategy used in the operations
    /// that require to iterate all the redis servers
    /// </summary>
    public partial class ServerEnumerationStrategy
    {
        /// <summary>
        /// 获取服务策略
        /// </summary>
        public enum ModeOptions
        {
            /// <summary>将在所有节点上执行操作。</summary>
            All = 0,

            /// <summary>将在单个节点上执行操作</summary>
            Single
        }

        /// <summary>
        /// 执行的目标角色
        /// </summary>
        public enum TargetRoleOptions
        {
            /// <summary>任何服务</summary>
            Any = 0,

            /// <summary>首选从</summary>
            PreferSlave
        }

        /// <summary>
        /// 服务不可达操作
        /// </summary>
        public enum UnreachableServerActionOptions
        {
            /// <summary>redis服务不可用，抛出异常</summary>
            Throw = 0,

            /// <summary>当redis服务不可用忽略异常</summary>
            IgnoreIfOtherAvailable
        }

        /// <summary>
        /// Gets or sets the strategy mode
        /// </summary>
        /// <value>
        ///   Default value All.
        /// </value>
        public ModeOptions Mode { get; set; }

        /// <summary>
        /// 获取或设置服务角色
        /// </summary>
        /// <value>
        ///   Default value Any.
        /// </value>
        public TargetRoleOptions TargetRole { get; set; }

        /// <summary>
        /// 获取或设置不可到达的服务器操作
        /// </summary>
        /// <value>
        ///   Default value Throw.
        /// </value>
        public UnreachableServerActionOptions UnreachableServerAction { get; set; }
    }
}
