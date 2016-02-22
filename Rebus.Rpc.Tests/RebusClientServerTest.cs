using NUnit.Framework;
using Rebus.Rpc.Tests.Rebus;

namespace Rebus.Rpc.Tests
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
