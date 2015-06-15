import sys
from azure.servicebus import ServiceBusService, Message

namespace = 'plantduino'
key_name = 'arduino'
key_value = 'HOP2Zzu4S8v+1xN0b9nHwuCmVsJk4YJIi1R46hhL6tY='
sbs = ServiceBusService(
	namespace, 
	shared_access_key_name = key_name,
	shared_access_key_value = key_value)

msg = Message('{ "Temperature": ' + sys.argv[2] + ' }',
	custom_properties = { 'DeviceId': sys.argv[1], 'MessageType': 'TemperatureTelemetry' })

sbs.send_topic_message('telemetry', msg)
