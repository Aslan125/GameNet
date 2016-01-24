using System;
using GameNet.Common.CommandsBase;
using Lidgren.Network;
using GameNet.Common.Operations;
using NLog;

namespace GameNet.LoginService.LoginClientCmd
{
    class CmdInitOk : ICommand<NetConnection>
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public CmdInitOk(NetConnection receiver)
        {
            this.receiver = receiver;
        }
        public NetConnection receiver
        {
            get;

            set;
        }

        public void Execute()
        {
            var msg = receiver.Peer.CreateMessage();
            
            string key = Guid.NewGuid().ToString();
            
                   
            msg.Write(LoginOperation.InitOk+"&"+ key.ToString());
            msg.Encrypt(new   NetRC2Encryption(receiver.Peer,receiver.CryptoKey));  
            receiver.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
            receiver.CryptoKey = null;
            receiver.CryptoKey = key;
            logger.Info("Key {1} for {0} sended", receiver.RemoteEndPoint.Address.ToString(), key);
        }
    }
}
