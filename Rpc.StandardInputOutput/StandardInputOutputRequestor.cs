using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Rpc.Core;

namespace Rpc.StandardInputOutput
{
    public class StandardInputOutputRequestor : IRequestor, IDisposable
    {
        private readonly StandardInputOutputBus _bus;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _result
            = new ConcurrentDictionary<Guid, TaskCompletionSource<object>>();

        public StandardInputOutputRequestor(Process process)
        {
            _bus = new StandardInputOutputBus(process, Receive);
        }

        public StandardInputOutputRequestor(int processId)
        {
            _bus = new StandardInputOutputBus(processId, Receive);
        }

        public Task<object> Ask(object request)
        {
            var message = new RpcMessage { RequestId = Guid.NewGuid(), Payload = request };
            var result = new TaskCompletionSource<object>();
            _result[message.RequestId] = result;
            _bus.Send(message);
            return result.Task;
        }

        private void Receive(object msg, StandardInputOutputBus bus)
        {
            var response = msg as RpcMessage;
            if (response != null)
            {
                TaskCompletionSource<object> responseTask;
                if (_result.TryRemove(response.RequestId, out responseTask))
                    responseTask.SetResult(response.Payload);
            }
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}