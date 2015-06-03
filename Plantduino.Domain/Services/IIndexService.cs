using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages;

namespace Rumr.Plantduino.Domain.Services
{
    public interface IIndexService
    {
        Task IndexMessageAsync<T>(T message) where T : Message;
    }
}