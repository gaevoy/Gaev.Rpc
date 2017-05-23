using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rpc.Core;

namespace Rpc.StandardInputOutput
{
    public class StandardInputOutputResponder : IResponder, IDisposable
    {
        private StandardInputOutputBus _bus;
        private readonly Dictionary<Type, Func<object, Task<object>>> _handler = new Dictionary<Type, Func<object, Task<object>>>();

        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            _handler[typeof(TRequest)] = msg => handle((TRequest)msg);
        }

        public void Start()
        {
            _bus = _bus ?? new StandardInputOutputBus(Receive);
        }

        private void Receive(object msg, StandardInputOutputBus bus)
        {
            var request = msg as RpcMessage;
            if (request == null) return;
            var payloadType = request.Payload?.GetType();
            if (payloadType == null) return;
            Func<object, Task<object>> handle;
            if (!_handler.TryGetValue(payloadType, out handle)) return;
            handle(request.Payload).ContinueWith(completedTask =>
            {
                _bus.Send(new RpcMessage
                {
                    RequestId = request.RequestId,
                    Payload = completedTask.Result
                });
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public void Dispose()
        {
            _bus?.Dispose();
        }
    }
}