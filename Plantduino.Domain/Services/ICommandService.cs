using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Commands;

namespace Rumr.Plantduino.Domain.Services
{
    public interface ICommandService
    {
        Task RaiseAsync(CommandMessage command);

        Task InitializeAsync<T>() where T : CommandMessage;
    }
}