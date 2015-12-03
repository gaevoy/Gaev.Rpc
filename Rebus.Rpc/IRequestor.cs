using System.Threading.Tasks;

namespace Rebus.Rpc
{
    public interface IRequestor
    {
        Task<object> Ask(object request);
    }
}
