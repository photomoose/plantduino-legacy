using System.Threading.Tasks;

namespace Rumr.Plantduino.Domain.Services
{
    public interface IMessageHandler<in T>
    {
        Task HandleAsync(T message);
    }
}