using CardAccessControl.Data;
using CardAccessControl.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;  // For MqttProtocolVersion
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CardAccessControl.Services
{
    public class MqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;

        private readonly SemaphoreSlim _dbSemaphore = new(1, 1);


        public event EventHandler<AccessTime>? AccessTimeLogged;
        private void OnAccessTimeLogged(AccessTime at) =>
        AccessTimeLogged?.Invoke(this, at);
        public MqttService()
        {
            var factory = new MqttFactory(); 
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("CardAccessControlClient")
                .WithTcpServer("127.0.0.1", 1883)
                .WithProtocolVersion(MqttProtocolVersion.V500)  // Use MQTT v5
                .Build();

            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected to MQTT broker.");
                await _mqttClient.SubscribeAsync("cardreader/+/request");
                Console.WriteLine("Subscribed to topic: cardreader/+/request");
            };

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                await Task.CompletedTask;
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? []);
                    
                    Console.WriteLine($"Lock: '{ExtractCardReaderId(topic)}' – card_id: {payload}");

                    var accessTime = new AccessTime
                    {
                        Time = DateTime.Now,
                        CardId = int.Parse(payload),
                        CardReaderId = ExtractCardReaderId(topic)
                    };

      
                    await _dbSemaphore.WaitAsync();
                    try
                    {
                        await using var db = new AccessControlContext();
                        db.AccessTimes.Add(accessTime);
                        await db.SaveChangesAsync();                
                    }
                    finally
                    {
                        _dbSemaphore.Release();
                    }
                    Console.WriteLine($"Access logged: {accessTime.Id}");
                    OnAccessTimeLogged(accessTime);

                    await PublishMessage("actuator/response", $"Processed: {payload}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MQTT handler error: {ex}");
                }
            };
        }

        public async Task ConnectAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                await _mqttClient.ConnectAsync(_mqttOptions);
            }
        }

        public async Task PublishMessage(string topic, string message)
        {
            if (_mqttClient.IsConnected)
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine($"Published message to '{topic}': {message}");
            }
        }

        private int ExtractCardReaderId(string topic)
        {
            string pattern = @"cardreader/(\d+)/request";

            Match match = Regex.Match(topic, pattern);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                throw new ArgumentException("Input string does not match the expected format.");
            }
        }
    }
}
