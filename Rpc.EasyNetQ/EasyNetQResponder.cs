using System;
using System.Threading.Tasks;
using EasyNetQ;
using Rpc.Core;

namespace Rpc.EasyNetQ
{
    public class EasyNetQResponder : IResponder
    {
        readonly IBus bus;

        public EasyNetQResponder(IBus bus)
        {
            this.bus = bus;
        }
        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            bus.RespondAsync<TRequest, RpcResponse>(async request =>
            {
                var payload = await handle(request);
                return new RpcResponse { Payload = payload };
            });
        }
    }
}