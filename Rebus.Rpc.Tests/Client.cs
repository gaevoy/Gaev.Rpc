using System;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.Rpc.Impl;
using Rpc.Core;

namespace Rebus.Rpc.Tests
{
    public interface IClient 
    {
        void Start(string nodeId = "");
        void Stop();
        object Ask(object request);
    }

    public class Client : MarshalByRefObject, IClient
    {
        string nodeId;
        BuiltinHandlerActivator activator;
        IRequestor requestor;

        public void Start(string nodeId = "")
        {
            this.nodeId = nodeId;
            activator = new BuiltinHandlerActivator();
            StartBus(activator);
            requestor = new RebusRequestor(activator);
        }

        public object Ask(object request)
        {
            return requestor.Ask(request).Result;
        }

        public void Stop()
        {
            activator.Dispose();
        }

        IBus StartBus(BuiltinHandlerActivator activator)
        {
            return Configure.With(activator)
                .Logging(e => e.ColoredConsole(LogLevel.Warn))
                .Transport(t => t.UseRabbitMq("amqp://localhost", "Rebus.Rpc.Tests-Requestor-" + nodeId))
                .Routing(e => e.TypeBased())
                .Start();
        }
    }
}