using System;

namespace KebaUdpService
{
    public interface IMqttClient : IDisposable
    {
        void Connect();
        void Disconnect();
        
        void Send(string topic, string value);
    }
}