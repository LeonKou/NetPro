using System.Threading.Tasks;

namespace MQMiddleware
{
    /// <summary>
    /// Interface of a service that handle messages asynchronously.
    /// </summary>
    public interface IAsyncMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        Task Handle(string message, string routingKey);
    }
}