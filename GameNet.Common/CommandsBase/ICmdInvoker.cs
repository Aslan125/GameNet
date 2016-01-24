

using Lidgren.Network;

namespace GameNet.Common.CommandsBase
{
    public interface ICmdInvoker
    {
        void PeerListener(NetPeer peer);
        void ExecuteCommand(ICommand command);


    }
}
