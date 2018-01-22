using Microsoft.Extensions.Logging;

namespace KebaUdpService.MessageHandlers
{
    public class PlugStateMessageHandler : KebaMessageHandler
    {
        public PlugStateMessageHandler(IKebaConnector kebaConnector, ILoggerFactory loggerfactory, IMqttClient mqttClient) : base(kebaConnector, loggerfactory, mqttClient)
        {
        }

        public override void HandleMessage(dynamic jsonObject)
        {
            int? id = jsonObject.ID;
            int? plug = jsonObject.Plug;

            if(id.HasValue || !plug.HasValue)
                return;

            var plugState = (PlugState)plug.GetValueOrDefault();

            SendMessage("/plugstate", plugState.ToString());

            Logger.LogDebug($"Plugstate handled. " +
                            $"Plug: {plugState.ToString()}");
        }
    }
}