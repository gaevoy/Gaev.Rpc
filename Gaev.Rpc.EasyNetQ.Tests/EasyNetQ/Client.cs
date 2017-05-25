using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Gaev.Rpc.Rebus.Tests;

namespace Gaev.Rpc.EasyNetQ.Tests.EasyNetQ
{
    public class Client : MarshalByRefObject, IClient
    {
        string nodeId;
        IRequestor requestor;
        IBus bus;

        public void Start(string nodeId = "")
        {
            this.nodeId = nodeId;
            this.bus = RabbitHutch.CreateBus("host=localhost");
            requestor = new EasyNetQRequestor(bus);
        }

        public object Ask(object request)
        {
            return requestor.Ask(request).Result;
        }
        public void AskManyTimes(object request, int times)
        {
            Task.WhenAll(Enumerable.Range(0, times).Select(_ => requestor.Ask(request)));
        }
    
        public void Dispose()
        {
            bus.Dispose();
        }

//        IBus StartBus()
//        {
//            return Configure.With(activator)
//                .Logging(e => e.ColoredConsole(LogLevel.Warn))
//                .Transport(t => t.UseRabbitMq("amqp://localhost", "EasyNetQ.Rpc.Tests-Requestor-" + nodeId))
//                .Routing(e => e.TypeBased())
//                .Start();
//        }
    }
}