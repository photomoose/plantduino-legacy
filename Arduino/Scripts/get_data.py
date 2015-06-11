import sys
sys.path.insert(0, '/usr/lib/python2.7/bridge/')

from bridgeclient import BridgeClient as BridgeClient

bridge = BridgeClient()

bridge.put('IsCold', '1')
