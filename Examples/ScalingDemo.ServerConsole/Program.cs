using System;
using System.Threading.Tasks;
using Gaev.RebbitMqCommandBus.Shared;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
using Rebus.Routing.TypeBased;
using Rebus.Rpc;
using Rebus.Rpc.Impl;

namespace ScalingDemo.ServerConsole
{
    class Program
    {
        public static string NodeId = Guid.NewGuid().ToString();
        static void Main(string[] args)
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                var bus = StartBus(activator);

                IResponder responder = new RebusResponder(activator, bus);
                responder.On<PingMessage>(OnPing);

                Console.WriteLine("Server {0}. Press enter to quit", NodeId);
                Console.ReadLine();
            }
        }
        
        static Task<object> OnPing(PingMessage msg)
        {
            Console.WriteLine("Server received {0}", msg.Payload);
            return Task.FromResult<object>(new PongMessage { Sender = NodeId });
        }
        
        static IBus StartBus(BuiltinHandlerActivator activator)
        {
            return Configure.With(activator)
                .Logging(e => e.ColoredConsole(LogLevel.Warn))
                .Transport(t => t.UseRabbitMq("amqp://localhost", "ScalingDemo-Responder"))
                .Routing(e => e.TypeBased())
                .Start();
        }
    }
}
