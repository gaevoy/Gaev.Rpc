using System;
using System.Threading.Tasks;

namespace Rebus.Rpc
{
    public interface IResponder
    {
        void On<TRequest>(Func<TRequest, Task<object>> handle);
    }
}
