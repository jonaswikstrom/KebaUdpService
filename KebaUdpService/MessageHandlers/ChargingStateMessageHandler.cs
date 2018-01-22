using Microsoft.Extensions.Logging;

namespace KebaUdpService.MessageHandlers
{
    public class ChargingStateMessageHandler : KebaMessageHandler
    {
        public ChargingStateMessageHandler(IKebaConnector kebaConnector, ILoggerFactory loggerfactory, IMqttClient mqttClient) : base(kebaConnector, loggerfactory, mqttClient)
        {
        }

        public override void HandleMessage(dynamic jsonObject)
        {
            int? state = jsonObject.State;
            int? id = jsonObject.ID;

            if (id.HasValue || !state.HasValue)
                return;

            ChargingState = (ChargingState) state.GetValueOrDefault();

            SendMessage("/chargingstate", ChargingState.ToString());
            Logger.LogDebug($"Charging state handled. " +
                            $"State: {ChargingState.ToString()}");
        }
    }
}