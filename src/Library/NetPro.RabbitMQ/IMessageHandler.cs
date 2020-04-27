namespace MQMiddleware
{
    /// <summary>
    /// Interface of a service that handle messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        void Handle(string message, string routingKey);
    }
}