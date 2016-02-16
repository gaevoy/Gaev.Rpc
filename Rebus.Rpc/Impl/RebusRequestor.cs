using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Messages;
using Rebus.Pipeline;
using Rpc.Core;

namespace Rebus.Rpc.Impl
{
    public class RebusRequestor : IRequestor
    {
        readonly IDictionary<string, TaskCompletionSource<object>> result = new ConcurrentDictionary<string, TaskCompletionSource<object>>();
        readonly BuiltinHandlerActivator activator;
        readonly Task completedTask = Task.FromResult(0);

        public RebusRequestor(BuiltinHandlerActivator activator)
        {
            this.activator = activator;
            activator.Bus.Subscribe<RpcResponse>();
            activator.Handle<RpcResponse>(OnResponseCame);
        }

        Task OnResponseCame(IBus _, IMessageContext ctx, RpcResponse msg)
        {
            string reqId;
            if (ctx.Message.Headers.TryGetValue(Headers.CorrelationId, out reqId))
            {
                TaskCompletionSource<object> res;
                if (result.TryGetValue(reqId, out res))
                {
                    res.SetResult(msg.Payload);
                    result.Remove(reqId);
                }
            }

            return completedTask;
        }

        public Task<object> Ask(object msg)
        {
            var res = new TaskCompletionSource<object>();
            var reqId = Guid.NewGuid().ToString();
            result[reqId] = res;
            activator.Bus.Publish(msg, new Dictionary<string, string> { { Headers.CorrelationId, reqId } });
            return res.Task;
        }
    }
}
