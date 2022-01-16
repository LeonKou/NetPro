using Grpc.Core;
using XXX.GrpcServer.DemoOne.Services;

namespace XXX.GrpcServer.DemoTwo.Services
{
    [GrpcService]
    public class GreeterTwoService : GreeterTwo.GreeterTwoBase
    {
        private readonly ILogger<GreeterOneService> _logger;
        public GreeterTwoService(ILogger<GreeterOneService> logger)
        {
            _logger = logger;
        }

        public override Task<GreeterTwo_HelloReply> SayHello(GreeterTwo_HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GreeterTwo_HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}