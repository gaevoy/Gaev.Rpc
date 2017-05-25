using System;
using System.Threading.Tasks;
using EasyNetQ;
using Gaev.Rpc.Rebus.Tests;

namespace Gaev.Rpc.EasyNetQ.Tests.EasyNetQ
{
    public class Server : MarshalByRefObject, IServer
    {
        string nodeId;
        IResponder responder;
        IBus bus;

        public void Start(string nodeId = "")
        {
            this.nodeId = nodeId;
            this.bus = RabbitHutch.CreateBus("host=localhost");
            responder = new EasyNetQResponder(bus);
        }

        public void On<TRequest>(Func<TRequest, object> handle) where TRequest : class
        {
            responder.On<TRequest>(req => Task.FromResult(handle(req)));
        }

        public void Dispose()
        {
            bus.Dispose();
        }
        /*
                IBus StartBus(BuiltinHandlerActivator activator)
                {
                    return Configure.With(activator)
                        .Logging(e => e.ColoredConsole(LogLevel.Warn))
                        .Transport(t => t.UseRabbitMq("amqp://localhost", "Rebus.Rpc.Tests-Responder-" + nodeId))
                        .Routing(e => e.TypeBased())
                        .Start();
                }*/
    }
}