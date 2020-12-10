using System.Collections.Generic;

namespace MQMiddleware.Configuration
{
    /// <summary>
    /// Exchange options.
    /// </summary>
    public class RabbitMqExchangeOptions
    {
        /// <summary>
        /// Exchange type.
        /// direct;fanout ;topic 
        /// </summary>
        public string Type { get; set; } = "direct";

        /// <summary>
        /// Durable option.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// AutoDelete option.
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Default dead-letter-exchange.
        /// </summary>
        public string DeadLetterExchange { get; set; } = " ";

        /// <summary>
        /// Option to re-queue failed messages (once).
        /// </summary>
        public bool RequeueFailedMessages { get; set; } = true;

        /// <summary>
        /// Additional arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Collection of queues connected to the exchange.
        /// </summary>
        public IList<RabbitMqQueueOptions> Queues { get; set; } = new List<RabbitMqQueueOptions>();
    }
}