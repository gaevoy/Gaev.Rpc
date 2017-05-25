using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;

namespace Gaev.Rpc.Rebus
{
    public class RebusResponder : IResponder
    {
        readonly BuiltinHandlerActivator activator;
        readonly IBus bus;

        public RebusResponder(BuiltinHandlerActivator activator, IBus bus)
        {
            this.activator = activator;
            this.bus = bus;
        }

        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            activator.Handle<TRequest>(async (_, __, request) =>
            {
                var response = await handle(request);
                if (response != null) await bus.Reply(new RpcMessage { Payload = response });
            });
            bus.Subscribe<TRequest>();
        }
    }
}
