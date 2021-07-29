using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Grpc
{
    public class GrpcServiceAttribute : Attribute
    {
        public bool IsStart { get; set; }
    }
}
