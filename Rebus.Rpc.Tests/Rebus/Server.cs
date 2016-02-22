using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.Rpc.Impl;
using Rpc.Core;

namespace Rebus.Rpc.Tests.Rebus
{
    public class Server : MarshalByRefObject, IServer
    {
        string nodeId;
        BuiltinHandlerActivator activator;
        IResponder responder;

        public void Start(string nodeId = "")
        {
            this.nodeId = nodeId;
            activator = new BuiltinHandlerActivator();
            var bus = StartBus(activator);
            responder = new RebusResponder(activator, bus);
        }

        public void On<TRequest>(Func<TRequest, object> handle) where TRequest : class
        {
            responder.On<TRequest>(req => Task.FromResult(handle(req)));
        }

        public void Dispose()
        {
            activator.Dispose();
        }

        IBus StartBus(BuiltinHandlerActivator activator)
        {
            return Configure.With(activator)
                .Logging(e => e.ColoredConsole(LogLevel.Warn))
                .Transport(t => t.UseRabbitMq("amqp://localhost", "Rebus.Rpc.Tests-Responder-" + nodeId))
                .Routing(e => e.TypeBased())
                .Start();
        }
    }
}