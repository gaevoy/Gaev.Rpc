using System.Threading.Tasks;

namespace Gaev.Rpc
{
    public interface IRequestor
    {
        Task<object> Ask(object request);
    }
}
