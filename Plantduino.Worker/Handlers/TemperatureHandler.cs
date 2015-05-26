using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Rumr.Plantduino.Worker.Commands;
using Rumr.Plantduino.Worker.Sms;
using Rumr.Plantduino.Worker.Telemetry;

namespace Rumr.Plantduino.Worker.Handlers
{
    public class TemperatureHandler : ITelemetryHandler
    {
        private readonly ITopicSubscriber _topicSubscriber;
        private readonly ITopicPublisher _topicPublisher;
        private readonly ISmsClient _smsClient;
        private readonly IConfiguration _configuration;
        private bool _isColdSpell;
        private DateTime _coldSpellStartUtc;

        public string SubscriptionName
        {
            get { return Subscriptions.Temperature; }
        }

        public Filter SubscriptionFilter
        {
            get { return new SqlFilter(string.Format("MessageType = '{0}'", typeof(TemperatureTelemetry).Name)); }
        }

        public TemperatureHandler(ITopicSubscriber topicSubscriber, ITopicPublisher topicPublisher, ISmsClient smsClient, IConfiguration configuration)
        {
            _topicSubscriber = topicSubscriber;
            _topicPublisher = topicPublisher;
            _smsClient = smsClient;
            _configuration = configuration;
        }

        public async Task ReceiveAsync()
        {
            var brokeredMessage = await _topicSubscriber.ReceiveFromTopicAsync(Topics.Telemetry, SubscriptionName);

            if (brokeredMessage != null)
            {
                var tempMsg = MessageMapper.Map<TemperatureTelemetry>(brokeredMessage);

                Trace.TraceInformation("{0}: Received temperature telemetry. (Temperature: {1}).", tempMsg.DeviceId, tempMsg.Temperature);

                if (tempMsg.Temperature <= _configuration.ColdSpellTemp && !_isColdSpell)
                {
                    Trace.TraceInformation("{0}: Entering cold spell.", tempMsg.DeviceId);

                    _isColdSpell = true;
                    _coldSpellStartUtc = DateTime.UtcNow;

                    await SendCommandAsync(new ColdLedOnCommand(tempMsg.DeviceId, tempMsg.Temperature));

                    _smsClient.Send("442071838750", "447742471000", "Entering cold spell.");
                }
                else if (tempMsg.Temperature > _configuration.ColdSpellTemp && _isColdSpell)
                {
                    var coldSpellDuration = DateTime.UtcNow - _coldSpellStartUtc;

                    Trace.TraceInformation("{0}: Leaving cold spell. (Duration: {1}.)", tempMsg.DeviceId, coldSpellDuration);

                    _isColdSpell = false;
                    await SendCommandAsync(new ColdLedOffCommand(tempMsg.DeviceId, tempMsg.Temperature));

                    _smsClient.Send("442071838750", "447742471000", string.Format("Cold spell over. Duration {0}.", coldSpellDuration));
                }

                await brokeredMessage.CompleteAsync();
            }
        }

        private async Task SendCommandAsync<T>(T command) where T : Message
        {
            var message = MessageMapper.Map(command);

            await _topicPublisher.SendToTopicAsync(Topics.Commands, message);
        }
    }
}