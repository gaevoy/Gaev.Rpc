using System;

namespace Rpc.StandardInputOutput
{
    public class RpcMessage
    {
        public Guid RequestId { get; set; }
        public object Payload { get; set; }
    }
}