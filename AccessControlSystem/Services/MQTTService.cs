using AccessControlSystem.Data;
using AccessControlSystem.Models;
using AccessControlSystem.Security;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;  // For MqttProtocolVersion
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AccessControlSystem.Services
{
    /// <summary>
    ///  Listens to card‑reader topics, stores every swipe, and raises an event so
    ///  the UI can refresh in real time.
    /// </summary>
    public class MqttService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;
        private readonly SemaphoreSlim _dbLock = new(1, 1);
        private readonly AccessEvaluator _evaluator;

        public event EventHandler<AccessTime>? AccessTimeLogged;
        private void OnAccessTimeLogged(AccessTime at) =>
        AccessTimeLogged?.Invoke(this, at);
        public MqttService(IUnitOfWork uow)
        {
            _uow = uow;
            _evaluator = new AccessEvaluator(uow);

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithClientId("CardAccessControlClient")
                .WithTcpServer("127.0.0.1", 1883)
                .WithProtocolVersion(MqttProtocolVersion.V500)  // Use MQTT v5
                .Build();

            _client.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected to MQTT broker.");
                await _client.SubscribeAsync("cardreader/+/request");
                Console.WriteLine("Subscribed to topic: cardreader/+/request");
            };

            _client.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                await Task.CompletedTask;
            };

            _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
        }

        public async Task ConnectAsync()
        {
            if (!_client.IsConnected)
            {
                await _client.ConnectAsync(_options);
            }
        }

        private async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload ?? []);

                int readerId = ExtractCardReaderId(topic);
                int cardId = int.Parse(payload);
               
                Console.WriteLine($"Lock: '{readerId}' – card_id: {cardId}");

                bool allowed = await _evaluator.IsAccessAllowedAsync(cardId, readerId);

                var access = new AccessTime
                {
                    Time = DateTime.Now,
                    CardId = int.Parse(payload),
                    CardReaderId = ExtractCardReaderId(topic)
                };
                if (!allowed) 
                {
                    Console.WriteLine("Access Denied!");
                    return;
                }

                Console.WriteLine("Access Granted!");

                await _dbLock.WaitAsync();
                try
                {
                    await _uow.AccessTimes.AddAsync(access);
                    await _uow.CommitAsync();
                }
                finally
                {
                    _dbLock.Release();
                }

                Console.WriteLine($"Access logged: {access.CardId}");

                OnAccessTimeLogged(access); // Notify UI

                await PublishMessage($"actuator/{readerId}/response", $"Processed: {payload} -> Unlock");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MQTT handler error: {ex}");
            }
        }
        

        public async Task PublishMessage(string topic, string message)
        {
            if (!_client.IsConnected)
            { 
                return;
            }
            
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .Build();

            await _client.PublishAsync(mqttMessage);
            Console.WriteLine($"Published message to '{topic}': {message}");
        }

        private static int ExtractCardReaderId(string topic)
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
        public async ValueTask DisposeAsync()
        {
            if (_client.IsConnected) await _client.DisconnectAsync();
            _dbLock.Dispose();
        }
    }
}
