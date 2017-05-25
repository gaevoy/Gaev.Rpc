using Gaev.Rpc.EasyNetQ.Tests.EasyNetQ;
using Gaev.Rpc.Rebus.Tests;
using NUnit.Framework;

namespace Gaev.Rpc.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQClientServerTest: ClientServerTest
    {
        protected override IServer NewServer()
        {
            return New<Server>() as IServer;
        }

        protected override IClient NewClient()
        {
            return New<Client>() as IClient;
        }
    }
}
