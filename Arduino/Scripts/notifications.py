#!/usr/bin/env python

import sys
import json
import urllib2
import logging
import logging.handlers
import traceback

from azure import WindowsAzureError
from azure.servicebus import ServiceBusService, Rule
from urllib2 import URLError, HTTPError

LOG_FILENAME = 'notifications.log'

# Set up a specific logger with our desired output level
logger = logging.getLogger('notifications')
logger.setLevel(logging.DEBUG)

formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')

# Add the log message handler to the logger
handler = logging.handlers.TimedRotatingFileHandler(
              LOG_FILENAME, when='midnight', backupCount=5, utc=True)
handler.setFormatter(formatter)
logger.addHandler(handler)

#service_namespace = 'plantduino-dev'
#key_value = 'CbL51jGlxVFlWKlCjhxbNghW2Au/PqO5LRm3oRxF3k4='

service_namespace = 'plantduino'
key_value = 'HOP2Zzu4S8v+1xN0b9nHwuCmVsJk4YJIi1R46hhL6tY='
key_name = 'Arduino'

try:
	sbs = ServiceBusService(service_namespace,
	                        shared_access_key_name=key_name,
	                        shared_access_key_value=key_value)

	sbs.create_subscription('notifications', 'ColdSpellNotifications_1')

	coldSpells = {}

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

				logger.info('Received ' + msg.custom_properties['messagetype'])
				logger.debug(obj)

				if msg.custom_properties['messagetype'] == 'ColdSpellEnteredNotification':
					coldSpells[obj['SensorId']] = True
				elif msg.custom_properties['messagetype'] == 'ColdSpellLeftNotification':
					coldSpells[obj['SensorId']] = False


				if True in coldSpells.values():
					urllib2.urlopen('http://localhost/mailbox/coldspellledon')
					logger.info('Cold Spell LED: On')
				else:
					urllib2.urlopen('http://localhost/mailbox/coldspellledoff')
					logger.info('Cold Spell LED: Off')

				msg.delete()
		except (WindowsAzureError, URLError, HTTPError) as e:
			logger.warn('Error: ' + traceback.format_exc())
		except Exception as e:
			logger.error('Error: ' + traceback.format_exc())
			raise
except Exception as e:
	logger.error('Error: ' + traceback.format_exc())