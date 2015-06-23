using System;
using System.Threading.Tasks;
using Nest;
using Rumr.Plantduino.Domain.Messages;
using Rumr.Plantduino.Domain.Messages.Notifications;
using Rumr.Plantduino.Domain.Messages.Telemetry;
using Rumr.Plantduino.Domain.Services;

namespace Rumr.Plantduino.Infrastructure.Elastic
{
    public class ElasticIndexClient : IIndexService
    {
        private readonly ElasticClient _client;

        public ElasticIndexClient()
        {
            var node = new Uri("http://plantduino-kibana.cloudapp.net:9200");
            var settings = new ConnectionSettings(node);
#if DEBUG
            settings.MapDefaultTypeIndices(d => d.Add(typeof(TemperatureTelemetry), "dev-telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(LuxTelemetry), "dev-telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(MoistureTelemetry), "dev-telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(ColdSpellEnteredNotification), "dev-notification"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(ColdSpellLeftNotification), "dev-notification"));
#else
            settings.MapDefaultTypeIndices(d => d.Add(typeof(TemperatureTelemetry), "telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(LuxTelemetry), "telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(MoistureTelemetry), "telemetry"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(ColdSpellEnteredNotification), "notification"));
            settings.MapDefaultTypeIndices(d => d.Add(typeof(ColdSpellLeftNotification), "notification"));
#endif
            _client = new ElasticClient(settings);
        }

        public async Task IndexMessageAsync<T>(T message) where T : Message
        {
            await _client.IndexAsync(message);
        }
    }
}