using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KebaUdpService
{
    public class MqttClient : IMqttClient
    {
        private readonly IConfiguration configuration;
        private readonly uPLibrary.Networking.M2Mqtt.MqttClient mqttClient;
        private readonly string baseTopic;
        private readonly ILogger logger;
        private readonly string hostName;

        public MqttClient(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            logger = loggerFactory.CreateLogger(GetType());

            hostName = configuration["mqtt:hostname"];
            baseTopic = configuration["mqtt:topic"];

            if(string.IsNullOrWhiteSpace(hostName))
                throw new InvalidOperationException("Missing 'hostname' in config file");

            if (string.IsNullOrWhiteSpace(baseTopic))
                throw new InvalidOperationException("Missing 'topic' in config file");

            mqttClient = new uPLibrary.Networking.M2Mqtt.MqttClient(configuration["mqtt:hostname"]);
        }


        public void Connect()
        {
            var username = configuration["mqtt:username"];
            var password = configuration["mqtt:password"];

            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Missing 'username' in config file");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Missing 'password' in config file");

            var clientId = Guid.NewGuid().ToString();

            logger.LogInformation($"Connecting MQTT Server on {hostName}...");
            mqttClient.Connect(clientId, username, password);

            if (SpinWait.SpinUntil(() => mqttClient.IsConnected, 30000))
            {
                logger.LogInformation("Connected");
                return;
            }

            throw new TimeoutException("Could not connect to mqtt client");

        }

        public void Disconnect()
        {
            mqttClient.Disconnect();
        }

        public void Send(string topic, string value)
        {
            if (!topic.StartsWith("/"))
                topic = $"/{topic}";

            var fullTopic = $"{baseTopic}{topic}";
            mqttClient.Publish($"{fullTopic}", Encoding.UTF8.GetBytes(value));
            logger.LogDebug($"Published '{value}' to '{fullTopic}");
        }

        public void Dispose()
        {
            if(!mqttClient.IsConnected)
                return;
            
            Disconnect();
        }
    }
}
