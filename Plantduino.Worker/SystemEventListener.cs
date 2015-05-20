using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public class SystemEventListener
    {
        private readonly ITopicManager _topicManager;
        private readonly IEnumerable<ISystemEventHandler> _handlers;

        public SystemEventListener(ITopicManager topicManager, IEnumerable<ISystemEventHandler> handlers)
        {
            _topicManager = topicManager;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _topicManager.CreateTopicAsync(TopicPaths.SystemEvents);

            var subscriptions = _handlers.Select(CreateSubscriptionAsync);

            await Task.WhenAll(subscriptions);
        }

        public async Task RunAsync(CancellationToken token)
        {
            var taskBuilders = CreateTaskBuilders();

            var tasks = taskBuilders.Select(tb => tb(token));

            await Task.WhenAll(tasks);
        }

        private IEnumerable<Func<CancellationToken, Task>> CreateTaskBuilders()
        {
            var tasks = _handlers.Select<ISystemEventHandler, Func<CancellationToken, Task>>(
                h => async token =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        await h.ReceiveAsync();
                    }
                });

            return tasks;
        }

        private async Task CreateSubscriptionAsync(ISystemEventHandler handler)
        {
            var name = handler.Name;

            var sd = new SubscriptionDescription(TopicPaths.SystemEvents, name);

            await _topicManager.CreateSubscriptionAsync(sd, handler.SubscriptionFilter);
        }
    }
}