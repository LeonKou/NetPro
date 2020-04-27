namespace MQMiddleware
{
    /// <summary>
    /// Interface of a non-cycling message handler.
    /// </summary>
    public interface INonCyclicMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="queueService">Queue service.</param>
        void Handle(string message, string routingKey, IQueueService queueService);
    }
}