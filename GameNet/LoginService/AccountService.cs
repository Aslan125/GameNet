using System;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;
using System.Text;
using GameNet.Common.CommandsBase;
using NLog;
using GameNet.LoginService.LoginClientCmd;

namespace GameNet.LoginService
{
    public class AccountService
    {

        Logger logger = LogManager.GetCurrentClassLogger();
        NetPeerConfiguration _config;
        NetServer _listener;
        ICmdInvoker _invoker;
        Task _peerListenTask;
         
        private AccountService()
        {
           
            _config = new NetPeerConfiguration("AccountService");
            _config.Port = 9595;
            _listener = new NetServer(_config);
            _invoker = new LoginCmdInvoker(); 
            _listener.Start();
            _peerListenTask = new Task(() => _invoker.PeerListener(_listener));
            _peerListenTask.Start();
            Thread.Sleep(50);


            logger.Info("#####Net Peer Configurations#####");
            logger.Info("Application Identifier: {0}", _config.AppIdentifier);
            logger.Info("Local Address: {0}", _config.LocalAddress);
            logger.Info("Port: {0}", _config.Port);
            logger.Info("Broadcast Address: {0}", _config.BroadcastAddress);
            logger.Info("Network ThreadName: {0}", _config.NetworkThreadName);
            logger.Info("Auto Expand MTU: {0}", _config.AutoExpandMTU);
            logger.Info("Auto Flush Send Queue: {0}", _config.AutoFlushSendQueue);
            logger.Info("Accept Incoming Connections: {0}", _config.AcceptIncomingConnections);
            logger.Info("Connection Timeout: {0}", _config.ConnectionTimeout);
            logger.Info("Default Outgoing Message Capacity: {0}", _config.DefaultOutgoingMessageCapacity);
            logger.Info("Enable UPnP: {0}", _config.EnableUPnP);
            logger.Info("Expand MTU Fail Attempts: {0}", _config.ExpandMTUFailAttempts);
            logger.Info("Expand MTU Frequency: {0}", _config.ExpandMTUFrequency);
            logger.Info("Maximum Connections: {0}", _config.MaximumConnections);
            logger.Info("Maximum Handshake Attempts: {0}", _config.MaximumHandshakeAttempts);
            logger.Info("Maximum Transmission Unit: {0}", _config.MaximumTransmissionUnit);
            logger.Info("Ping Interval: {0}", _config.PingInterval);
            logger.Info("Receive Buffer Size: {0}", _config.ReceiveBufferSize);
            logger.Info("Send Buffer Size: {0}", _config.SendBufferSize);
            logger.Info("Resend Handshake Interval: {0}", _config.ResendHandshakeInterval);
            logger.Info("Recycled Cache Max Count: {0}", _config.RecycledCacheMaxCount);
            logger.Info("UseMessageRecycling: {0}", _config.UseMessageRecycling);
            logger.Info("PeerLietener thread status: {0}", _peerListenTask.Status);
            logger.Info("#####Net Peer Listener#####");
            logger.Info("Status: {0}",_listener.Status);
            logger.Info("Unique Identifier: {0}",_listener.UniqueIdentifier);
            logger.Info("Tag: {0}",_listener.Tag);
            logger.Info("Connections Count: {0}",_listener.ConnectionsCount);

        }

        #region Singlton
        private static AccountService _instance;
        private static object sync = new object();
        public static AccountService GetInstance ()
        {
            lock (sync)
            {
                if (_instance == null)
                {
                    _instance = new AccountService();
                }


                return _instance;
            }
        }
         
        #endregion




    }
}
