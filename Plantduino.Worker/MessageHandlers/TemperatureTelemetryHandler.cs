using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Rumr.Plantduino.Worker.Commands;
using Rumr.Plantduino.Worker.Sms;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.MessageHandlers
{
    public class TemperatureTelemetryHandler : IMessageHandler<TemperatureTelemetry>
    {
        private readonly IConfiguration _configuration;
        private readonly ISmsClient _smsClient;
        private readonly ITopicPublisher _topicPublisher;
        private bool _isColdSpell;
        private DateTime _coldSpellStartUtc;

        public TemperatureTelemetryHandler(IConfiguration configuration, ISmsClient smsClient, ITopicPublisher topicPublisher)
        {
            _configuration = configuration;
            _smsClient = smsClient;
            _topicPublisher = topicPublisher;
        }

        public async Task HandleAsync(TemperatureTelemetry message)
        {
            Trace.TraceInformation("{0}: Received temperature telemetry. (Temperature: {1}).", message.DeviceId, message.Temperature);

            if (message.Temperature <= _configuration.ColdSpellTemp && !_isColdSpell)
            {
                Trace.TraceInformation("{0}: Entering cold spell.", message.DeviceId);

                _isColdSpell = true;
                _coldSpellStartUtc = DateTime.UtcNow;

                await _topicPublisher.SendToTopicAsync(TopicNames.Commands, new ColdLedOnCommand(message.DeviceId, message.Temperature));

                _smsClient.Send("442071838750", "447742471000", "Entering cold spell.");
            }
            else if (message.Temperature > _configuration.ColdSpellTemp && _isColdSpell)
            {
                var coldSpellDuration = DateTime.UtcNow - _coldSpellStartUtc;

                Trace.TraceInformation("{0}: Leaving cold spell. (Duration: {1}.)", message.DeviceId, coldSpellDuration);

                _isColdSpell = false;
                await _topicPublisher.SendToTopicAsync(TopicNames.Commands, new ColdLedOffCommand(message.DeviceId, message.Temperature));

                _smsClient.Send("442071838750", "447742471000", string.Format("Cold spell over. Duration {0}.", coldSpellDuration));
            }
        }
    }
}