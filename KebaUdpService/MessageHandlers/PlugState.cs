namespace KebaUdpService.MessageHandlers
{
    public enum PlugState
    {
        Unplugged = 0,
        PluggedOnChargingStation = 1,
        PluggedAndLockedOnChargingStation = 3,
        PluggedOnEv = 5,
        PluggedAndLockedOnEv = 7,
    }
}