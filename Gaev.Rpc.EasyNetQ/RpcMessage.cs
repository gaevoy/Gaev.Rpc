namespace Gaev.Rpc.EasyNetQ
{
    public class RpcMessage
    {
        public object Payload { get; set; }
        public object Error { get; set; }
    }
}
