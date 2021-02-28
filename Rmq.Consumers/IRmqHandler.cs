using System.Threading;
using System.Threading.Tasks;

namespace Rmq.Consumers
{
    public interface IRmqHandler<T> where T : class
    {
        Task Handle(T message, CancellationToken token);
    }
}
