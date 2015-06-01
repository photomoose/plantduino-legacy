using System;
using System.Threading.Tasks;
using Nest;
using Rumr.Plantduino.Domain;
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
            settings.MapDefaultTypeIndices(d => d.Add(typeof(TelemetryIndex<TemperatureTelemetry>), "temperature-telemetry"));
            _client = new ElasticClient(settings);
        }

        public async Task IndexAsync<T>(TelemetryIndex<T> message) where T : TelemetryMessage 
        {
            await _client.IndexAsync(message);
        }
    }
}