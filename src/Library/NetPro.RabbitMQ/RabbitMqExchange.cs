using MQMiddleware.Configuration;

namespace MQMiddleware
{
    /// <summary>
    /// Exchange model.
    /// </summary>
    public class RabbitMqExchange
    {
        /// <summary>
        /// The unique name of the exchange.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag determining whether the exchange is made for message consumption.
        /// If false then an exchange made only for publishing.
        /// </summary>
        public bool IsConsuming { get; set; }

        /// <summary>
        /// Exchange options.
        /// </summary>
        public RabbitMqExchangeOptions Options { get; set; }
    }
}