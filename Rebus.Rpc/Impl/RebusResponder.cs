using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;

namespace Rebus.Rpc.Impl
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

        public void On<TRequest>(Func<TRequest, Task<object>> handle)
        {
            activator.Handle<TRequest>(async (_, __, request) =>
            {
                var response = await handle(request);
                await bus.Reply(new RpcResponse { Payload = response });
            });
            bus.Subscribe<TRequest>();
        }
    }
}
