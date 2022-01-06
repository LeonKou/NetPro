using System;
using System.Collections.Generic;
using System.Text;

namespace System.NetPro
{
    public class GrpcServiceAttribute : Attribute
    {
        public bool IsStart { get; set; }
    }
}
