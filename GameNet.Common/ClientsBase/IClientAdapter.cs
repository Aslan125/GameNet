using Lidgren.Network;

namespace GameNet.Common.ClientsBase
{
    public interface IClientAdapter
    {

        void OnConnected(NetIncomingMessage msg);
         void OnDisconnecting(NetIncomingMessage msg);
         void OnDisconnected(NetIncomingMessage msg);
         void OnData(NetIncomingMessage msg);
    }
}
