using Gaev.Rpc.Rebus.Tests.Rebus;
using NUnit.Framework;

namespace Gaev.Rpc.Rebus.Tests
{
    [TestFixture]
    public class RebusClientServerTest : ClientServerTest
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
