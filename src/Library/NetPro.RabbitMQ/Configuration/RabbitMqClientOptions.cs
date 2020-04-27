namespace MQMiddleware.Configuration
{
    /// <summary>
    /// RabbitMQ configuration model.
    /// </summary>
    public class RabbitMqClientOptions
    {
        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        public string HostName { get; set; } = "127.0.0.1";

        /// <summary>
        /// Port.
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// UserName that connects to the server.
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// Password of the chosen user.
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// Virtual host.
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Automatic connection recovery option.
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// Topology recovery option.
        /// </summary>
        public bool TopologyRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// Timeout for connection attempts.
        /// </summary>
        public int RequestedConnectionTimeout { get; set; } = 60000;

        /// <summary>
        /// Heartbeat timeout.
        /// </summary>
        public ushort RequestedHeartbeat { get; set; } = 60;
    }
}