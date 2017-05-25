using System;

namespace Gaev.Rpc.Rebus.Tests
{
    public interface IClient: IDisposable
    {
        void Start(string nodeId = "");
        object Ask(object request);
        void AskManyTimes(object request, int times);
    }
}