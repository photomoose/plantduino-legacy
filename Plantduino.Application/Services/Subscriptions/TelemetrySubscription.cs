using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Application.Services.Subscriptions
{
    public class TelemetrySubscription<T> : ITopicSubscription where T : TelemetryMessage
    {
        private readonly ITelemetryService _telemetryService;
        private readonly IEnumerable<IMessageHandler<T>> _handlers;
        private readonly IIndexService _indexService;

        public TelemetrySubscription(ITelemetryService telemetryService, IEnumerable<IMessageHandler<T>> handlers, IIndexService indexService)
        {
            _telemetryService = telemetryService;
            _handlers = handlers;
            _indexService = indexService;
        }

        public async Task InitializeAsync()
        {
            await _telemetryService.InitializeAsync<T>();
        }

        public async Task ListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var telemetry = await _telemetryService.ReceiveAsync<T>();

                    if (telemetry != null)
                    {
                        Trace.TraceInformation("{0}: RECEIVED: {1} {{Timestamp: {2}}}", telemetry.DeviceId,
                            telemetry.MessageType, telemetry.Timestamp);

                        await _indexService.IndexMessageAsync(telemetry);

                        Parallel.ForEach(_handlers, async h => await h.HandleAsync(telemetry));

                        await telemetry.CompleteAsync();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }
    }
}