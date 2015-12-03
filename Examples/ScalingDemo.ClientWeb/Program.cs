using System;
using Gaev.RebbitMqCommandBus.Shared;
using Nancy;
using Nancy.Hosting.Self;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.Rpc;
using Rebus.Rpc.Impl;

namespace ScalingDemo.ClientWeb
{
    class Program
    {
        public static string NodeId = Guid.NewGuid().ToString();
        public static IRequestor Requestor;
        static void Main(string[] args)
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                StartBus(activator);

                Requestor = new RebusRequestor(activator);

                Console.WriteLine("Client {0} is ready on http://localhost:8080/ping.json?payload=ping-payload. Press enter to quit", NodeId);
                var host = new NancyHost(new Uri("http://localhost:8080"));
                host.Start();
                Console.ReadLine();
                host.Stop();
            }
        }

        static IBus StartBus(BuiltinHandlerActivator activator)
        {
            return Configure.With(activator)
                .Logging(e => e.ColoredConsole(LogLevel.Warn))
                .Transport(t => t.UseRabbitMq("amqp://localhost", "ScalingDemo-Requestor-" + NodeId))
                .Routing(e => e.TypeBased())
                .Start();
        }
    }

    public class ApiModule : NancyModule
    {
        const bool async = true;
        public ApiModule()
        {
            IRequestor requestor = Program.Requestor;
            Get["/ping", async] = async (_, __) => await requestor.Ask(new PingMessage
            {
                Sender = Program.NodeId,
                Payload = (string) Request.Query.payload
            });

        }
    }
}
