/* Copyright (c) 2010 Michael Lidgren

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Net;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
    /// <summary>
    /// Специализированная версия NetPeer использоваться для "клиента" связи. Это не принимает входящие соединения и поддерживает свойство подключения к серверу
    /// </summary>
    public class NetClient : NetPeer
	{
        /// <summary>
        /// Получает соединение с сервером, если таковые
        /// </summary>
        public NetConnection ServerConnection
		{
			get
			{
				NetConnection retval = null;
				if (m_connections.Count > 0)
				{
					try
					{
						retval = m_connections[0];
					}
					catch
					{
						// preempted!
						return null;
					}
				}
				return retval;
			}
		}

        /// <summary>
        /// Получает состояние подключения соединения с сервером (или NetConnectionStatus.Disconnected если нет соединения)
        /// </summary>
        public NetConnectionStatus ConnectionStatus
		{
			get
			{
				var conn = ServerConnection;
				if (conn == null)
					return NetConnectionStatus.Disconnected;
				return conn.Status;
			}
		}

        /// <summary>
        /// NetClient конструктор
        /// </summary>
        /// <param name="config"></param>
        public NetClient(NetPeerConfiguration config)
			: base(config)
		{
			config.AcceptIncomingConnections = false;
		}

        /// <summary>
        /// Подключение к удаленному серверу
        /// </summary>
        /// <param name="remoteEndPoint">Удаленная подключения к</param>
        /// <param name="hailMessage">Сообщение град пройти</param>
        /// <returns>соединение с сервером, или нуль, если уже подключен</returns>
        public override NetConnection Connect(NetEndPoint remoteEndPoint, NetOutgoingMessage hailMessage)
		{
			lock (m_connections)
			{
				if (m_connections.Count > 0)
				{
					LogWarning("Connect attempt failed; Already connected");
					return null;
				}
			}

			lock (m_handshakes)
			{
				if (m_handshakes.Count > 0)
				{
					LogWarning("Connect attempt failed; Handshake already in progress");
					return null;
				}
			}

			return base.Connect(remoteEndPoint, hailMessage);
		}

        /// <summary>
        /// Отсоедините от сервера
        /// </summary>
        /// <param name="byeMessage">Причина отключения</param>
        public void Disconnect(string byeMessage)
		{
			NetConnection serverConnection = ServerConnection;
			if (serverConnection == null)
			{
				lock (m_handshakes)
				{
					if (m_handshakes.Count > 0)
					{
						LogVerbose("Aborting connection attempt");
						foreach(var hs in m_handshakes)
							hs.Value.Disconnect(byeMessage);
						return;
					}
				}

				LogWarning("Disconnect requested when not connected!");
				return;
			}
			serverConnection.Disconnect(byeMessage);
		}

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method)
		{
			NetConnection serverConnection = ServerConnection;
			if (serverConnection == null)
			{
				LogWarning("Cannot send message, no server connection!");
				return NetSendResult.FailedNotConnected;
			}

			return serverConnection.SendMessage(msg, method, 0);
		}

        /// <summary>
        /// Посылает сообщение на сервер.
        /// </summary>
        public NetSendResult SendMessage(NetOutgoingMessage msg, NetDeliveryMethod method, int sequenceChannel)
		{
			NetConnection serverConnection = ServerConnection;
			if (serverConnection == null)
			{
				LogWarning("Cannot send message, no server connection!");
				Recycle(msg);
				return NetSendResult.FailedNotConnected;
			}

			return serverConnection.SendMessage(msg, method, sequenceChannel);
		}

        /// <summary>
        /// Возвращает строку, представляющую этот объект.
        /// </summary>
        public override string ToString()
		{
			return "[NetClient " + ServerConnection + "]";
		}

	}
}
