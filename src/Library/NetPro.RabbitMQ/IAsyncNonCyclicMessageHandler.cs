using System.Threading.Tasks;

namespace MQMiddleware
{
    /// <summary>
    /// Interface of a non-cycling async message handler.
    /// </summary>
    public interface IAsyncNonCyclicMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        Task Handle(string message, string routingKey, IQueueService queueService);
    }
}