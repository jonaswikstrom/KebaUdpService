using Microsoft.Extensions.Logging;

namespace KebaUdpService.MessageHandlers
{
    public class Report3MessageHandler : KebaMessageHandler
    {
        public Report3MessageHandler(IKebaConnector kebaConnector, 
            ILoggerFactory loggerfactory, IMqttClient mqttClient) : 
            base(kebaConnector, loggerfactory, mqttClient)
        {
            
        }

        public override void HandleMessage(dynamic jsonObject)
        {
            int? reportId = jsonObject.ID;


            if(reportId.GetValueOrDefault() != 3)
                return;

            decimal? ePres = jsonObject.Epres;
            decimal? eTotal = jsonObject.Etotal;
            decimal? power = jsonObject.P;

            ePres = ePres.GetValueOrDefault() / 10000;
            eTotal = eTotal.GetValueOrDefault() / 10000;
            power = power.GetValueOrDefault() / 1000;

            if (ChargingState == ChargingState.Charging)
                eTotal = eTotal.GetValueOrDefault() + ePres.GetValueOrDefault();

            SendMessage("power", power.Value.ToString("F1"));

            SendMessage("epres", ePres.Value.ToString("F1"));

            SendMessage("etotal", eTotal.Value.ToString("F1"));


            Logger.LogDebug("Report 3 handled. " +
                            $"Power: {power:F1}W, E-pres: {ePres:F1}kWh, E-total: {eTotal:F1}kWh");
        }


    }
}