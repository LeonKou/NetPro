using MediatR;
using System.Threading;

namespace XXX.API.GlobalizationDemo.Service
{
    public class MediatorEvent : INotification
    {
        public MediatorEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class MediatorHandler : INotificationHandler<MediatorEvent>
    {
        private readonly ILogger<MediatorHandler> _logger;

        public MediatorHandler(ILogger<MediatorHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MediatorEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"Handled start: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
