import paho.mqtt.client as mqtt
import sys

if len(sys.argv) != 2:
        print("Usage: python actuator_subscriber.py <actuator_id>")
        sys.exit(1)

actuator_id = int(sys.argv[1])

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected successfully")
        client.subscribe(f"actuator/{actuator_id}/response")
    else:
        print(f"Failed to connect, return code {rc}")

def on_message(client, userdata, msg):
    print(f"Received message on topic {msg.topic}: {msg.payload.decode()}")

def on_disconnect(client, userdata, rc):
    print("Disconnected with result code " + str(rc))

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message
client.on_disconnect = on_disconnect

try:
    client.connect("localhost", 1883, 60)
    client.loop_forever()
except KeyboardInterrupt:
    print("Exiting...")
    client.disconnect()
except Exception as e:
    print("An error occurred:", e)
