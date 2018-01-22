using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace KebaUdpService.MessageHandlers
{
    public abstract class KebaMessageHandler : IKebaMessageHandler
    {
        private static readonly object SyncRoot = new object();
        private readonly IMqttClient mqttClient;
        protected readonly ILogger Logger;
        private ChargingState chargingState;

        protected ChargingState ChargingState
        {
            get => chargingState;
            set
            {
                lock (SyncRoot)
                {
                    chargingState = value;
                }
            }
        }

        protected KebaMessageHandler(IKebaConnector kebaConnector, 
            ILoggerFactory loggerfactory, IMqttClient mqttClient)
        {
            this.mqttClient = mqttClient;
            Logger = loggerfactory.CreateLogger(GetType());

            kebaConnector.ValueReceived += (sender, message) =>
            {
                if (!message.StartsWith("{"))
                    return;

                try
                {
                    HandleMessage(JObject.Parse(message.Replace(" ", "")));
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            };
        }

        protected void SendMessage(string topic, string value)
        {
            mqttClient.Send(topic, value);
        }

        public abstract void HandleMessage(dynamic jsonObject);
    }
}