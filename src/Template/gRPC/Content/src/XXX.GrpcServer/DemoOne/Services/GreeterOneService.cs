using Grpc.Core;

namespace XXX.GrpcServer.DemoOne.Services
{
    [GrpcService]
    public class GreeterOneService : GreeterOne.GreeterOneBase
    {
        private readonly ILogger<GreeterOneService> _logger;
        public GreeterOneService(ILogger<GreeterOneService> logger)
        {
            _logger = logger;
        }

        public override Task<GreeterOne_HelloReply> SayHello(GreeterOne_HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GreeterOne_HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}