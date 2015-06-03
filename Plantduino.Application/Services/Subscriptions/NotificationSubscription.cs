using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Subscriptions
{
    public class NotificationSubscription<T> : ITopicSubscription where T : NotificationMessage
    {
        private readonly INotificationService _notificationService;
        private readonly IEnumerable<IMessageHandler<T>> _handlers;
        private readonly IIndexService _indexService;

        public NotificationSubscription(INotificationService notificationService, IEnumerable<IMessageHandler<T>> handlers, IIndexService indexService)
        {
            _notificationService = notificationService;
            _handlers = handlers;
            _indexService = indexService;
        }

        public async Task InitializeAsync()
        {
            await _notificationService.InitializeAsync<T>();
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var notification = await _notificationService.ReceiveAsync<T>();

                if (notification != null)
                {
                    await _indexService.IndexMessageAsync(notification);

                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(notification));

                    await notification.CompleteAsync();
                }
            }
        }
    }
}