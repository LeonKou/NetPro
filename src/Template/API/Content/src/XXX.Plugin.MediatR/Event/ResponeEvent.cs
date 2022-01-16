using MediatR;
using XXX.Plugin.MediatR.Model;

namespace XXX.Plugin.MediatR.Event
{
    /// <summary>
    /// Response处理端
    /// </summary>
    public class ResponeEvent
      : IRequestHandler<SendRequestModel, string>
    {
        public Task<string> Handle(SendRequestModel request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"获取到MediatR请求的数据Message= {request.Message}");
            Console.WriteLine($"获取到MediatR请求的数据UserId= {request.UserId}");
            return Task.FromResult($"{request.Message}-{request.UserId}");
        }
    }
}
