using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Notifications;

namespace Rumr.Plantduino.Domain.Services
{
    public interface INotificationService
    {
        Task RaiseAsync(NotificationMessage notification);

        Task<T> ReceiveAsync<T>() where T : NotificationMessage;

        Task InitializeAsync<T>() where T : NotificationMessage;
    }
}