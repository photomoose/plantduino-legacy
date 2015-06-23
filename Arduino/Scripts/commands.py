import json
import urllib2
import ConfigParser

from azure.servicebus import ServiceBusService, Rule

config = ConfigParser.SafeConfigParser()
config.read('config.ini')

namespace = config.get('ServiceBus', 'Namespace')
key_name = config.get('ServiceBus', 'KeyName')
key_value = config.get('ServiceBus', 'KeyValue')

sbs = ServiceBusService(service_namespace,
                        shared_access_key_name=key_name,
                        shared_access_key_value=key_value)

sbs.create_subscription('notifications', 'ColdSpellNotifications_1')
#rule = Rule()
#rule.filter_type = 'SqlFilter'
#rule.filter_type = "MessageType = 'ColdSpellEnteredNotification'"
#sbs.create_rule('notifications', 'ColdSpellNotifications_1', 'ColdSpellNotifications', rule)
#sbs.delete_rule('notifications', 'ColdSpellNotifications_1', DEFAULT_RULE_NAME)

while True:
	msg = sbs.receive_subscription_message('notifications', 'ColdSpellNotifications_1')
	
	if msg.body:
		obj = json.loads(msg.body)

		print 'Received ' + msg.custom_properties['messagetype']

		if msg.custom_properties['messagetype'] == 'ColdSpellEnteredNotification':
			urllib2.urlopen('http://localhost/coldspellledon')
		elif msg.custom_properties['messagetype'] == 'ColdSpellLeftNotification':
			urllib2.urlopen('http://localhost/coldspellledoff')


		msg.delete()
