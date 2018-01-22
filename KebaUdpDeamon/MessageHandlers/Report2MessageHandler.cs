using Microsoft.Extensions.Logging;

namespace KebaUdpService.MessageHandlers
{
    public class Report2MessageHandler : KebaMessageHandler
    {
        public Report2MessageHandler(IKebaConnector kebaConnector, 
            ILoggerFactory loggerfactory, IMqttClient mqttClient) : 
            base(kebaConnector, loggerfactory, mqttClient)
        {
        }

        public override void HandleMessage(dynamic jsonObject)
        {
            int? id = jsonObject.ID;

            if (id.GetValueOrDefault() != 2)
                return;

            int? state = jsonObject.State;
            int? plug = jsonObject.Plug;

            ChargingState = (ChargingState) state.GetValueOrDefault();
            var plugState = (PlugState) plug.GetValueOrDefault();

            SendMessage("/chargingstate", ChargingState.ToString());
            SendMessage("/plugstate", plugState.ToString());

            Logger.LogDebug($"Report 2 handled. " +
                            $"State: {ChargingState.ToString()}," +
                            $"Plug: {plugState.ToString()}");
        }
    }
}