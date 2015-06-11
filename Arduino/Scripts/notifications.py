#!/usr/bin/env python

import json
import urllib2

from azure.servicebus import ServiceBusService, Rule

#service_namespace = 'plantduino-dev'
#key_value = 'CbL51jGlxVFlWKlCjhxbNghW2Au/PqO5LRm3oRxF3k4='

service_namespace = 'plantduino'
key_value = 'HOP2Zzu4S8v+1xN0b9nHwuCmVsJk4YJIi1R46hhL6tY='
key_name = 'Arduino'

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
	try:
		msg = sbs.receive_subscription_message('notifications', 'ColdSpellNotifications_1')
	
		if msg.body:
			obj = json.loads(msg.body)

			print 'Received ' + msg.custom_properties['messagetype']

			if msg.custom_properties['messagetype'] == 'ColdSpellEnteredNotification':
				urllib2.urlopen('http://localhost/mailbox/coldspellledon')
				print 'Cold Spell LED: On'
			elif msg.custom_properties['messagetype'] == 'ColdSpellLeftNotification':
				urllib2.urlopen('http://localhost/mailbox/coldspellledoff')
				print 'Cold Spell LED: Off'

			msg.delete()
	except azure.WindowsAzureError as e:
		print 'Azure Error: ', e
