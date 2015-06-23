import sys
import ConfigParser
from azure.servicebus import ServiceBusService, Message

config = ConfigParser.SafeConfigParser()
config.read('config.ini')

namespace = config.get('ServiceBus', 'Namespace')
key_name = config.get('ServiceBus', 'KeyName')
key_value = config.get('ServiceBus', 'KeyValue')

sbs = ServiceBusService(
	namespace, 
	shared_access_key_name = key_name,
	shared_access_key_value = key_value)

msg = Message('{ "SensorId": "' + sys.argv[2] + '", "Moisture": ' + sys.argv[3] + ' }',
	custom_properties = { 'DeviceId': sys.argv[1], 'MessageType': 'MoistureTelemetry' })

sbs.send_topic_message('telemetry', msg)
