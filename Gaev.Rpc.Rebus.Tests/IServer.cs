using System;

namespace Gaev.Rpc.Rebus.Tests
{
    public interface IServer: IDisposable
    {
        void Start(string nodeId = "");
        void On<TRequest>(Func<TRequest, object> handle) where TRequest : class;
    }
}