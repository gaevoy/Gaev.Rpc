namespace Gaev.Rpc.Rebus
{
    public class RpcMessage
    {
        public object Payload { get; set; }
        public object Error { get; set; }
    }
}
