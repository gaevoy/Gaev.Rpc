using NUnit.Framework;
using Rebus.Rpc.Tests;
using Rebus.Rpc.Tests.EasyNetQ;
using Rpc.EasyNetQ.Tests.EasyNetQ;

namespace Rpc.EasyNetQ.Tests
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
