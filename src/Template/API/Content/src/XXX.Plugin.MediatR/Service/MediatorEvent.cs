using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace XXX.Plugin.MediatR
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
            _logger.LogWarning($"A消费者: {notification.Message}");
            return Task.CompletedTask;
        }
    }

    public class MediatorConsumHandler : INotificationHandler<MediatorEvent>
    {
        private readonly ILogger<MediatorHandler> _logger;

        public MediatorConsumHandler(ILogger<MediatorHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MediatorEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"B消费者: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
