using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Subscriptions
{
    public class ColdSpellLeftNotificationSubscription : ITopicSubscription
    {
        private readonly INotificationService _notificationService;
        private readonly IEnumerable<IMessageHandler<ColdSpellLeftNotification>> _handlers;

        public ColdSpellLeftNotificationSubscription(INotificationService notificationService, IEnumerable<IMessageHandler<ColdSpellLeftNotification>> handlers)
        {
            _notificationService = notificationService;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _notificationService.InitializeAsync<ColdSpellLeftNotification>();

        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var notification = await _notificationService.ReceiveAsync<ColdSpellLeftNotification>();

                if (notification != null)
                {
                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(notification));

                    await notification.CompleteAsync();
                }
            }
        }
    }
}