using MediatR;

namespace XXX.Plugin.MediatR
{
    /// <summary>
    /// 订阅同一个对象，A和B会同时被触发执行
    /// </summary>
    public class MediatorEvent : INotification
    {
        public MediatorEvent(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    /// <summary>
    /// 订阅A
    /// </summary>
    public class MediatorConsumAHandler : INotificationHandler<MediatorEvent>
    {
        private readonly ILogger<MediatorConsumAHandler> _logger;

        public MediatorConsumAHandler(ILogger<MediatorConsumAHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(MediatorEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning($"A消费者: {notification.Message}");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 订阅B
    /// </summary>
    public class MediatorConsumBHandler : INotificationHandler<MediatorEvent>
    {
        private readonly ILogger<MediatorConsumBHandler> _logger;

        public MediatorConsumBHandler(ILogger<MediatorConsumBHandler> logger)
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
