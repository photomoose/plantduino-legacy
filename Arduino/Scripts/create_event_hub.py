from azure.servicebus import ServiceBusService

namespace = 'rumruk'
key_name = 'arduino'
key_value = 'T8macrUAHRksynPCOGdEkUnXkZ/uyVZHNk6Y0Thx1gc='
sbs = ServiceBusService(
	namespace, 
	shared_access_key_name = key_name,
	shared_access_key_value = key_value)
	
sbs.create_event_hub('plantduino')
