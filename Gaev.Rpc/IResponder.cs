using System;
using System.Threading.Tasks;

namespace Gaev.Rpc
{
    public interface IResponder
    {
        void On<TRequest>(Func<TRequest, Task<object>> handle) where TRequest : class;
    }
}
