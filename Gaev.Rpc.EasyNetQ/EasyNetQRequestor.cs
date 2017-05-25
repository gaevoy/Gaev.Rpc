using System.Reflection;
using System.Threading.Tasks;
using EasyNetQ;

namespace Gaev.Rpc.EasyNetQ
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
            var RequestAsync = RequestAsyncOpenMethod.MakeGenericMethod(request.GetType(), typeof(RpcMessage));
            var response = await (Task<RpcMessage>)RequestAsync.Invoke(bus, new[] { request });
            // var response = await bus.RequestAsync<object, RpcResponse>(request);
            return response.Payload;
        }
    }
}