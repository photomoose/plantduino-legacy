using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Rumr.Plantduino.Worker
{
    public class TelemetryListener
    {
        private readonly ITopicManager _topicManager;
        private readonly IEnumerable<ITelemetryHandler> _handlers;

        public TelemetryListener(ITopicManager topicManager, IEnumerable<ITelemetryHandler> handlers)
        {
            _topicManager = topicManager;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _topicManager.CreateTopicAsync(Topics.Telemetry);

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
            var tasks = _handlers.Select<ITelemetryHandler, Func<CancellationToken, Task>>(
                h => async token =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            await h.ReceiveAsync();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }
                });

            return tasks;
        }

        private async Task CreateSubscriptionAsync(ITelemetryHandler handler)
        {
            var sd = new SubscriptionDescription(Topics.Telemetry, handler.SubscriptionName);

            await _topicManager.CreateSubscriptionAsync(sd, handler.SubscriptionFilter);
        }
    }
}