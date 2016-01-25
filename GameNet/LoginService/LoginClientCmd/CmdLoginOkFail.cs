using GameNet.Common.CommandsBase;
using GameNet.Common.Operations;
using GameNet.DataBaseServise;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace GameNet.LoginService.LoginClientCmd
{
    public class CmdLoginOkFail : ICommand<NetConnection>
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private string login;
        private string pass;

        public CmdLoginOkFail(NetConnection receiver, string login, string pass)
        {
            this.receiver = receiver;
            this.login = login;
            this.pass = pass;
        }
        public NetConnection receiver
        {
            get;

            set;
        }
        public void Execute()
        {
            string msgStr = string.Empty;

            Accounts[] account = null;

            using (var db = new DataBase())
            {

                try
                {
                    account = db.accounts.Where(p => p.Login == login).Where(p => p.Password == pass).ToArray();

                }
                catch (Exception ex)
                {
                    logger.Fatal(ex.Message);
                }
            }

            if (account.Length < 1)
            {
                msgStr = LoginOperation.LoginFail + "&" + "Не верный логин или пароль!";
                logger.Warn("{0} user data failed!", receiver.RemoteEndPoint.Address);
            }
            if (account.Length == 1)
            {
                if (account.First().Login == login && account.First().Password == pass)
                {

                    msgStr = LoginOperation.LoginOk + "&" + "Вы успешно авторизовались!";
                    logger.Info("{0} user authorized!", receiver.RemoteEndPoint.Address);
                }
            }
            var msg = receiver.Peer.CreateMessage(msgStr);
            msg.Encrypt(new NetRC2Encryption(receiver.Peer, receiver.CryptoKey));
            receiver.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
            

        }
    }
}
