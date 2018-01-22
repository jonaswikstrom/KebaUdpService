using System;

namespace KebaUdpService
{
    public interface IKebaConnector
    {
        event EventHandler<string> ValueReceived;
        void Connect();
        void Close();
    }
}