using System.Threading.Tasks;

namespace Rpc.Core
{
    public interface IRequestor
    {
        Task<object> Ask(object request);
    }
}
