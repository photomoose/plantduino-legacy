using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Subscriptions
{
    public class LuxTelemetrySubscription : ITopicSubscription
    {
        private readonly ITelemetryService _telemetryService;
        private readonly IEnumerable<IMessageHandler<LuxTelemetry>> _handlers;

        public LuxTelemetrySubscription(ITelemetryService telemetryService, IEnumerable<IMessageHandler<LuxTelemetry>> handlers)
        {
            _telemetryService = telemetryService;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _telemetryService.InitializeAsync<LuxTelemetry>();
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var telemetry = await _telemetryService.ReceiveAsync<LuxTelemetry>();

                if (telemetry != null)
                {
                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(telemetry));

                    await telemetry.CompleteAsync();
                }
            }
        }
    }
}