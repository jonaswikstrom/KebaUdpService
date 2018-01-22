namespace KebaUdpService.MessageHandlers
{
    public interface IKebaMessageHandler
    {
        void HandleMessage(dynamic jsonObject);
    }
}