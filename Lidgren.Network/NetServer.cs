using System;
using System.Collections.Generic;

namespace Lidgren.Network
{
    /// <summary>
    /// Специализированная версия Net Peer, используемого для «сервер» сверстников
    /// </summary>
    public class NetServer : NetPeer
	{
        /// <summary>
        /// NetServer конструктор
        /// </summary>
        public NetServer(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = true;
		}

        /// <summary>
        /// Отправить сообщение для всех соединений
        /// </summary>
        /// <param name="msg">Сообщение для отправки</param>
        /// <param name="method">Как доставить сообщение</param>
        public void SendToAll(NetOutgoingMessage msg, NetDeliveryMethod method)
		{
			var all = this.Connections;
			if (all.Count <= 0) {
				if (msg.m_isSent == false)
					Recycle(msg);
				return;
			}

			SendMessage(msg, all, method, 0);
		}

        /// <summary>
        /// Отправить сообщение для всех соединений, кроме одного
        /// </summary>
        /// <param name="msg">Сообщение для отправки</param>
        /// <param name="method">Как доставить сообщение</param>
        /// <param name="except">Не отправить данном связи</param>
        /// <param name="sequenceChannel">Какая последовательность каналов, чтобы использовать для сообщения</param>
        public void SendToAll(NetOutgoingMessage msg, NetConnection except, NetDeliveryMethod method, int sequenceChannel)
		{
			var all = this.Connections;
			if (all.Count <= 0) {
				if (msg.m_isSent == false)
					Recycle(msg);
				return;
			}

			if (except == null)
			{
				SendMessage(msg, all, method, sequenceChannel);
				return;
			}

			List<NetConnection> recipients = new List<NetConnection>(all.Count - 1);
			foreach (var conn in all)
				if (conn != except)
					recipients.Add(conn);

			if (recipients.Count > 0)
				SendMessage(msg, recipients, method, sequenceChannel);
		}

        /// <summary>
        /// Возвращает строку, представляющую этот объект
        /// </summary>
        public override string ToString()
		{
			return "[NetServer " + ConnectionsCount + " connections]";
		}
	}
}
