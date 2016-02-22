using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Rpc.Core;

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

        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            activator.Handle<TRequest>(async (_, __, request) =>
            {
                var response = await handle(request);
                if (response != null) await bus.Reply(new RpcResponse { Payload = response });
            });
            bus.Subscribe<TRequest>();
        }
    }
}
