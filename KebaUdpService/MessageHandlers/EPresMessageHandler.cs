using Microsoft.Extensions.Logging;

namespace KebaUdpService.MessageHandlers
{
    public class EPresMessageHandler : KebaMessageHandler
    {
        public EPresMessageHandler(IKebaConnector kebaConnector, 
            ILoggerFactory loggerfactory, IMqttClient mqttClient) 
            : base(kebaConnector, loggerfactory, mqttClient)
        {
            
        }


        public override void HandleMessage(dynamic jsonObject)
        {
            decimal? epres = jsonObject.Epres;
            decimal? sec = jsonObject.Sec;

            if (!epres.HasValue || sec.HasValue)
                return;

            epres = epres.GetValueOrDefault() / 10000;

            SendMessage("epres", epres.Value.ToString("F1"));

            Logger.LogDebug($"E-pres handled. E-pres: {epres:F1}kWh"); 
        }
    }
}