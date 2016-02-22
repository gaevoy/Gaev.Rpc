using System.Reflection;
using System.Threading.Tasks;
using EasyNetQ;
using Rpc.Core;

namespace Rpc.EasyNetQ
{
    public class EasyNetQRequestor : IRequestor
    {
        readonly IBus bus;
        MethodInfo RequestAsyncOpenMethod = typeof(IBus).GetMethod("RequestAsync");

        public EasyNetQRequestor(IBus bus)
        {
            this.bus = bus;
        }

        public async Task<object> Ask(object request)
        {
            var RequestAsync = RequestAsyncOpenMethod.MakeGenericMethod(request.GetType(), typeof(RpcResponse));
            var response = await (Task<RpcResponse>)RequestAsync.Invoke(bus, new[] { request });
            // var response = await bus.RequestAsync<object, RpcResponse>(request);
            return response.Payload;
        }
    }
}