using System.Threading.Tasks;

namespace Rumr.Plantduino.Worker.MessageHandlers
{
    public interface IMessageHandler<in T>
    {
        Task HandleAsync(T message);
    }
}