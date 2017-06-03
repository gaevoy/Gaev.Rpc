using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;

namespace Gaev.Rpc.Owin
{
    public class OwinResponder : IResponder
    {
        private readonly Dictionary<Type, Func<object, Task<object>>> _handler = new Dictionary<Type, Func<object, Task<object>>>();
        public void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class
        {
            _handler[typeof(TRequest)] = msg => handle((TRequest)msg);
        }

        public void Start(IAppBuilder app)
        {
            app.Use<RpcMiddleware>(this);
        }

        internal Task<object> Handle(object request)
        {
            Func<object, Task<object>> handle;
            if (!_handler.TryGetValue(request.GetType(), out handle))
                throw new ArgumentException($"Request type {request.GetType().Name} is not registered");
            return handle(request);
        }
    }
}