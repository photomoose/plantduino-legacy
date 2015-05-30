using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Subscriptions
{
    public class TemperatureTelemetrySubscription : ITopicSubscription
    {
        private readonly ITelemetryService _telemetryService;
        private readonly IEnumerable<IMessageHandler<TemperatureTelemetry>> _handlers;

        public TemperatureTelemetrySubscription(ITelemetryService telemetryService, IEnumerable<IMessageHandler<TemperatureTelemetry>> handlers)
        {
            _telemetryService = telemetryService;
            _handlers = handlers;
        }

        public async Task InitializeAsync()
        {
            await _telemetryService.InitializeAsync<TemperatureTelemetry>();
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var telemetry = await _telemetryService.ReceiveAsync<TemperatureTelemetry>();

                if (telemetry != null)
                {
                    Parallel.ForEach(_handlers, async h => await h.HandleAsync(telemetry));

                    await telemetry.CompleteAsync();
                }
            }
        }
    }
}