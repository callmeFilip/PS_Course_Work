import sys
import paho.mqtt.client as mqtt

def main():
    # Check if a card reader ID was provided as an argument
    if len(sys.argv) < 3:
        print("Usage: python publisher.py <cardreader_id> <card_id>")
        sys.exit(1)

    cardreader_id = int(sys.argv[1])
    card_id = int(sys.argv[2])
    topic = f"cardreader/{cardreader_id}/request"

    # Create an MQTT client instance and connect to the broker
    client = mqtt.Client()
    client.connect("localhost", 1883, 60)

    print(f"Publishing to topic: {topic}")
    
    try:
        print(f"Publishing card_id: {card_id}")
        client.publish(topic, payload=card_id)
        print(f"Done!")

    except KeyboardInterrupt:
        print("Exiting...")
        client.disconnect()

if __name__ == "__main__":
    main()
