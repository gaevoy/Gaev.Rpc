using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Rpc.Core;

namespace Rpc.StandardInputOutput
{
    public class StandardInputOutputResponder : IResponder, IDisposable
    {
        private readonly StandardInputOutputBus _bus;
        private readonly ConcurrentDictionary<Type, Func<object, Task<object>>> _handler 
            = new ConcurrentDictionary<Type, Func<object, Task<object>>>();

        public StandardInputOutputResponder()
        {
            _bus = new StandardInputOutputBus(Receive);
        }
        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            _handler[typeof(TRequest)] = msg => handle((TRequest)msg);
        }

        private async Task Receive(object msg, StandardInputOutputBus bus)
        {
            var request = msg as RpcMessage;
            if (request == null) return;
            var payloadType = request.Payload?.GetType();
            if (payloadType == null) return;
            Func<object, Task<object>> handle;
            if (!_handler.TryGetValue(payloadType, out handle)) return;
            var response = await handle(request.Payload).ConfigureAwait(false);
            await _bus.Send(new RpcMessage
            {
                RequestId = request.RequestId,
                Payload = response
            }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}