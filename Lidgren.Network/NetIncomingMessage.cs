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
using System.Diagnostics;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
    /// <summary>
    /// Входящее сообщение отправлено либо из удаленного узла или генерируется в библиотеке
    /// </summary>
    [DebuggerDisplay("Type={MessageType} LengthBits={LengthBits}")]
	public sealed class NetIncomingMessage : NetBuffer
	{
		internal NetIncomingMessageType m_incomingMessageType;
		internal NetEndPoint m_senderEndPoint;
		internal NetConnection m_senderConnection;
		internal int m_sequenceNumber;
		internal NetMessageType m_receivedMessageType;
		internal bool m_isFragment;
		internal double m_receiveTime;

        /// <summary>
        /// Получает тип данного входящего сообщения.
        /// </summary>
        public NetIncomingMessageType MessageType { get { return m_incomingMessageType; } }

        /// <summary>
        /// Получает способ доставки это сообщение было отправлено с (если пользовательских данных)
        /// </summary>
        public NetDeliveryMethod DeliveryMethod { get { return NetUtility.GetDeliveryMethod(m_receivedMessageType); } }

        /// <summary>
        /// Получает канал последовательность это сообщение было отправлено с (если пользовательские данные)
        /// </summary>
        public int SequenceChannel { get { return (int)m_receivedMessageType - (int)NetUtility.GetDeliveryMethod(m_receivedMessageType); } }

        /// <summary>
        /// Конечной точкой отправителя, если таковые
        /// </summary>
        public NetEndPoint SenderEndPoint { get { return m_senderEndPoint; } }

        /// <summary>
        /// NetConnection отправителя, если таковые
        /// </summary>
        public NetConnection SenderConnection { get { return m_senderConnection; } }

        /// <summary>
        /// Что местному времени было получено сообщение из сети
        /// </summary>
        public double ReceiveTime { get { return m_receiveTime; } }

		internal NetIncomingMessage()
		{
		}

		internal NetIncomingMessage(NetIncomingMessageType tp)
		{
			m_incomingMessageType = tp;
		}

		internal void Reset()
		{
			m_incomingMessageType = NetIncomingMessageType.Error;
			m_readPosition = 0;
			m_receivedMessageType = NetMessageType.LibraryError;
			m_senderConnection = null;
			m_bitLength = 0;
			m_isFragment = false;
		}

        /// <summary>
        /// Расшифровать сообщение
        /// </summary>
        /// <param name="encryption">Алгоритм шифрования, используемый для шифрования сообщения</param>
        /// <returns>правда на успех</returns>
        public bool Decrypt(NetEncryption encryption)
		{
			return encryption.Decrypt(this);
		}

        /// <summary>
        /// Читает значение, по местному времени, сравнимый с NetTime.Now, написанный с использованием WriteTime ()
        /// Должен иметь подключенный отправителя
        /// </summary>
        public double ReadTime(bool highPrecision)
		{
			return ReadTime(m_senderConnection, highPrecision);
		}

        /// <summary>
        /// Возвращает строку, представляющую этот объект
        /// </summary>
        public override string ToString()
		{
			return "[NetIncomingMessage #" + m_sequenceNumber + " " + this.LengthBytes + " bytes]";
		}
	}
}
