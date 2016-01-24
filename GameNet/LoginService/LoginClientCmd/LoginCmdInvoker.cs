using GameNet.Common.CommandsBase;
using GameNet.Common.Operations;
using GameNet.LoginService.LoginClientCmd;
using Lidgren.Network;
using System;
using NLog;

namespace GameNet.LoginService.LoginClientCmd
{
    class LoginCmdInvoker : ICmdInvoker
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public void PeerListener(NetPeer peer)
        {
            
            while (true)
            {
                NetIncomingMessage msg;
                while ((msg=peer.ReadMessage())!=null)
                {
                    NetConnection connection = msg.SenderConnection;

                    switch (msg.MessageType)
                    {
                      
                        case NetIncomingMessageType.StatusChanged:
                            switch (msg.SenderConnection.Status)
                            {    
                                case NetConnectionStatus.Connected:
                                    logger.Info("{0}: connected",connection.RemoteEndPoint.Address.ToString());
                                    ExecuteCommand(new CmdInitOk(connection));
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    logger.Info("{0}: disconnected", connection.RemoteEndPoint.Address.ToString());
                                    break;
                                case NetConnectionStatus.Disconnecting:
                                    logger.Info("{0}: disconnecting", connection.RemoteEndPoint.Address.ToString());
                                    break;
                                    
                                                                                                     
                            }
                            break;
                    
                        case NetIncomingMessageType.Data:
                            msg.Decrypt(new NetRC2Encryption(peer, connection.CryptoKey));  
                            string[] OpData = msg.ReadString().Split('&');
                            var OpCode = (LoginOperation)Enum.Parse(typeof(LoginOperation), OpData[0]);

                            switch (OpCode)
                            {                                
                                case LoginOperation.AuthLog:   
                                    string[] authData = OpData[1].Split('#');
                                    string login = authData[0];
                                    string pass = authData[1];
                                    ExecuteCommand(new CmdLoginOkFail(connection, login,pass));
                                    break;
                            }


                            break;                    
                    }

                    peer.Recycle(msg);
                }
            }

        }



        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
        }


    }
}
