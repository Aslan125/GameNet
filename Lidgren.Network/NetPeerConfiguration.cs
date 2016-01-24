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

namespace Lidgren.Network
{
    /// <summary>
    /// Небольшая неизменны после NetPeer был инициализирован.
    /// </summary>
    public sealed class NetPeerConfiguration
	{
        // Максимальная единица передачи 
        // Ethernet может принять 1500 байт данных, так что давайте оставаться ниже. 
        // Цель заключается в макс полном пакете, чтобы быть 1440 байт (30 х 48 байт ниже, чем 1468) 
        // -20 байт IP-заголовка 
        // -8 байт UDP заголовка 
        // -4 байта, чтобы быть на безопасной стороне и выровнять по 8 байт границы 
        // Всего 1408 байт 
        // Обратите внимание, что lidgren заголовки (с5 байт) сюда не включены; так как это часть «МТУ полезной нагрузки"

        /// <summary>
        /// Значение по умолчанию MTU в байтах.
        /// </summary>
        public const int kDefaultMTU = 1408;
		
		private const string c_isLockedMessage = "You may not modify the NetPeerConfiguration after it has been used to initialize a NetPeer";

		private bool m_isLocked;
		private readonly string m_appIdentifier;
		private string m_networkThreadName;
		private IPAddress m_localAddress;
		private IPAddress m_broadcastAddress;
		internal bool m_acceptIncomingConnections;
		internal int m_maximumConnections;
		internal int m_defaultOutgoingMessageCapacity;
		internal float m_pingInterval;
		internal bool m_useMessageRecycling;
		internal int m_recycledCacheMaxCount;
		internal float m_connectionTimeout;
		internal bool m_enableUPnP;
		internal bool m_autoFlushSendQueue;
		private NetUnreliableSizeBehaviour m_unreliableSizeBehaviour;
		internal bool m_suppressUnreliableUnorderedAcks;

		internal NetIncomingMessageType m_disabledTypes;
		internal int m_port;
		internal int m_receiveBufferSize;
		internal int m_sendBufferSize;
		internal float m_resendHandshakeInterval;
		internal int m_maximumHandshakeAttempts;

		// bad network simulation
		internal float m_loss;
		internal float m_duplicates;
		internal float m_minimumOneWayLatency;
		internal float m_randomOneWayLatency;

		// MTU
		internal int m_maximumTransmissionUnit;
		internal bool m_autoExpandMTU;
		internal float m_expandMTUFrequency;
		internal int m_expandMTUFailAttempts;

        /// <summary>
        /// Конструктор Конфигурация NetGear.
        /// </summary>
        public NetPeerConfiguration(string appIdentifier)
		{
			if (string.IsNullOrEmpty(appIdentifier))
				throw new NetException("App identifier must be at least one character long");
			m_appIdentifier = appIdentifier;

			//
			// default values
			//
			m_disabledTypes = NetIncomingMessageType.ConnectionApproval | NetIncomingMessageType.UnconnectedData | NetIncomingMessageType.VerboseDebugMessage | NetIncomingMessageType.ConnectionLatencyUpdated | NetIncomingMessageType.NatIntroductionSuccess;
			m_networkThreadName = "Lidgren network thread";
			m_localAddress = IPAddress.Any;
			m_broadcastAddress = IPAddress.Broadcast;
			var ip = NetUtility.GetBroadcastAddress();
			if (ip != null)
			{
				m_broadcastAddress = ip;
			}
			m_port = 0;
			m_receiveBufferSize = 131071;
			m_sendBufferSize = 131071;
			m_acceptIncomingConnections = false;
			m_maximumConnections = 32;
			m_defaultOutgoingMessageCapacity = 16;
			m_pingInterval = 4.0f;
			m_connectionTimeout = 25.0f;
			m_useMessageRecycling = true;
			m_recycledCacheMaxCount = 64;
			m_resendHandshakeInterval = 3.0f;
			m_maximumHandshakeAttempts = 5;
			m_autoFlushSendQueue = true;
			m_suppressUnreliableUnorderedAcks = false;

			m_maximumTransmissionUnit = kDefaultMTU;
			m_autoExpandMTU = false;
			m_expandMTUFrequency = 2.0f;
			m_expandMTUFailAttempts = 5;
			m_unreliableSizeBehaviour = NetUnreliableSizeBehaviour.IgnoreMTU;

			m_loss = 0.0f;
			m_minimumOneWayLatency = 0.0f;
			m_randomOneWayLatency = 0.0f;
			m_duplicates = 0.0f;

			m_isLocked = false;
		}

		internal void Lock()
		{
			m_isLocked = true;
		}

        /// <summary>
        /// Получает идентификатор данного приложения; библиотека может подключаться только к соответствию идентификаторов приложение сверстников.
        /// </summary>
        public string AppIdentifier
		{
			get { return m_appIdentifier; }
		}

        /// <summary>
        /// Включить получения указанного типа сообщения.
        /// </summary>
        public void EnableMessageType(NetIncomingMessageType type)
		{
			m_disabledTypes &= (~type);
		}

        /// <summary>
        /// Отключено получение указанного типа сообщения.
        /// </summary>
        public void DisableMessageType(NetIncomingMessageType type)
		{
			m_disabledTypes |= type;
		}

        /// <summary>
        /// Включает или отключает получение указанного типа сообщения.
        /// </summary>
        public void SetMessageTypeEnabled(NetIncomingMessageType type, bool enabled)
		{
			if (enabled)
				m_disabledTypes &= (~type);
			else
				m_disabledTypes |= type;
		}

        /// <summary>
        /// Получает, если получение указанного типа сообщения включен.
        /// </summary>
        public bool IsMessageTypeEnabled(NetIncomingMessageType type)
		{
			return !((m_disabledTypes & type) == type);
		}

        /// <summary>
        /// Получает или задает поведение ненадежным посылает выше MTU.
        /// </summary>
        public NetUnreliableSizeBehaviour UnreliableSizeBehaviour
		{
			get { return m_unreliableSizeBehaviour; }
			set { m_unreliableSizeBehaviour = value; }
		}

        /// <summary>
        /// Получает или задает имя библиотеки сети потоке. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public string NetworkThreadName
		{
			get { return m_networkThreadName; }
			set
			{
				if (m_isLocked)
					throw new NetException("NetworkThreadName may not be set after the NetPeer which uses the configuration has been started");
				m_networkThreadName = value;
			}
		}

        /// <summary>
        /// Получает или задает максимальное количество подключений Этот узел может содержать. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public int MaximumConnections
		{
			get { return m_maximumConnections; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_maximumConnections = value;
			}
		}

        /// <summary>
        /// Получает или задает максимальное количество байт для отправки в одном пакете, за исключением IP, UDP и lidgren заголовков. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public int MaximumTransmissionUnit
		{
			get { return m_maximumTransmissionUnit; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				if (value < 1 || value >= ((ushort.MaxValue + 1) / 8))
					throw new NetException("MaximumTransmissionUnit must be between 1 and " + (((ushort.MaxValue + 1) / 8) - 1) + " bytes");
				m_maximumTransmissionUnit = value;
			}
		}

        /// <summary>
        /// Получает или задает емкость по умолчанию в байтах, когда NetPeer.CreateMessage () вызывается без аргументов.
        /// </summary>
        public int DefaultOutgoingMessageCapacity
		{
			get { return m_defaultOutgoingMessageCapacity; }
			set { m_defaultOutgoingMessageCapacity = value; }
		}

        /// <summary>
        /// Получает или задает время между задержкой расчета пингов.
        /// </summary>
        public float PingInterval
		{
			get { return m_pingInterval; }
			set { m_pingInterval = value; }
		}

        /// <summary>
        /// Получает или задает, если библиотека должна утилизации сообщения, чтобы избежать чрезмерного сбора мусора. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public bool UseMessageRecycling
		{
			get { return m_useMessageRecycling; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_useMessageRecycling = value;
			}
		}

        /// <summary>
        /// Получает или задает максимальное количество входящих / исходящих сообщений, чтобы держать в кэше рециркуляции.
        /// </summary>
        public int RecycledCacheMaxCount
		{
			get { return m_recycledCacheMaxCount; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_recycledCacheMaxCount = value;
			}
		}

        /// <summary>
        /// Получает или задает число секунд тайм-аута будет отложено на успешный пинг-понг /.
        /// </summary>
        public float ConnectionTimeout
		{
			get { return m_connectionTimeout; }
			set
			{
				if (value < m_pingInterval)
					throw new NetException("Connection timeout cannot be lower than ping interval!");
				m_connectionTimeout = value;
			}
		}

        /// <summary>
        /// Включить поддержку UPnP; позволяет перенаправление портов и получение внешнего IP.
        /// </summary>
        public bool EnableUPnP
		{
			get { return m_enableUPnP; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_enableUPnP = value;
			}
		}

        /// <summary>
        /// Включает или отключает автоматическую промывку очереди отправки. Если отключена, вы должны вручную вызвать NetPeer.Flush Отправить Queue (), чтобы избавиться отправленные сообщения в сети.
        /// </summary>
        public bool AutoFlushSendQueue
		{
			get { return m_autoFlushSendQueue; }
			set { m_autoFlushSendQueue = value; }
		}

        /// <summary>
        /// Если это правда, не будет посылать подтверждений для ненадежных неупорядоченных сообщений. Это позволит сэкономить пропускную способность, но отключить управление потоком и обнаружение дубликатов для этого типа сообщений.
        /// </summary>
        public bool SuppressUnreliableUnorderedAcks
		{
			get { return m_suppressUnreliableUnorderedAcks; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_suppressUnreliableUnorderedAcks = value;
			}
		}

        /// <summary>
        /// Получает или задает локальный адрес IP связываться с. По умолчанию IPAddress.Any. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public IPAddress LocalAddress
		{
			get { return m_localAddress; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_localAddress = value;
			}
		}

        /// <summary>
        /// Получает или задает локальный широковещательный адрес, используемый при трансляции.
        /// </summary>
        public IPAddress BroadcastAddress
		{
			get { return m_broadcastAddress; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_broadcastAddress = value;
			}
		}

        /// <summary>
        /// Получает или задает локальный порт связываться с. По умолчанию 0. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public int Port
		{
			get { return m_port; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_port = value;
			}
		}

        /// <summary>
        /// Получает или задает размер в байтах буфера принимающего. По умолчанию 131071 байт. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public int ReceiveBufferSize
		{
			get { return m_receiveBufferSize; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_receiveBufferSize = value;
			}
		}

        /// <summary>
        /// Получает или задает размер в байтах буфера передачи. По умолчанию 131071 байт. Не может быть изменен один раз NetPeer инициализации.
        /// </summary>
        public int SendBufferSize
		{
			get { return m_sendBufferSize; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_sendBufferSize = value;
			}
		}

        /// <summary>
        /// Получает или задает о Netperf должны принимать входящие соединения. Это автоматически устанавливается на верно в NetServer и ложным в NetClient.
        /// </summary>
        public bool AcceptIncomingConnections
		{
			get { return m_acceptIncomingConnections; }
			set { m_acceptIncomingConnections = value; }
		}

        /// <summary>
        /// Получает или задает количество секунд между попытками рукопожатие.
        /// </summary>
        public float ResendHandshakeInterval
		{
			get { return m_resendHandshakeInterval; }
			set { m_resendHandshakeInterval = value; }
		}

        /// <summary>
        /// Получает или задает максимальное количество попыток рукопожатие перед отказом подключить.
        /// </summary>
        public int MaximumHandshakeAttempts
		{
			get { return m_maximumHandshakeAttempts; }
			set
			{
				if (value < 1)
					throw new NetException("MaximumHandshakeAttempts must be at least 1");
				m_maximumHandshakeAttempts = value;
			}
		}

        /// <summary>
        /// Получает или задает если NetPeer должны отправлять большие сообщения, чтобы попытаться расширить максимальный размер блока передачи.
        /// </summary>
        public bool AutoExpandMTU
		{
			get { return m_autoExpandMTU; }
			set
			{
				if (m_isLocked)
					throw new NetException(c_isLockedMessage);
				m_autoExpandMTU = value;
			}
		}

        /// <summary>
        /// Получает или задает, как часто отправлять большие сообщения, чтобы расширить MTU, если AUTOEXTEND включен.
        /// </summary>
        public float ExpandMTUFrequency
		{
			get { return m_expandMTUFrequency; }
			set { m_expandMTUFrequency = value; }
		}

        /// <summary>
        /// Получает или задает количество неудачных попыток расширить MTU для выполнения перед установкой окончательного MTU.
        /// </summary>
        public int ExpandMTUFailAttempts
		{
			get { return m_expandMTUFailAttempts; }
			set { m_expandMTUFailAttempts = value; }
		}

#if DEBUG
        /// <summary>
        /// Получает или задает имитацию количество отправленных пакетов, потерянных от 0.0f до 1.0f.
        /// </summary>
        public float SimulatedLoss
		{
			get { return m_loss; }
			set { m_loss = value; }
		}

        /// <summary>
        /// Получает или задает минимальное количество моделируемых одну сторону задержки для отправленных пакетов в сек.
        /// </summary>
        public float SimulatedMinimumLatency
		{
			get { return m_minimumOneWayLatency; }
			set { m_minimumOneWayLatency = value; }
		}

        /// <summary>
        /// Gets or sets the simulated added random amount of one way latency for sent packets in seconds
        /// </summary>
        public float SimulatedRandomLatency
		{
			get { return m_randomOneWayLatency; }
			set { m_randomOneWayLatency = value; }
		}

        /// <summary>
        /// Gets the average simulated one way latency in seconds
        /// </summary>
        public float SimulatedAverageLatency
		{
			get { return m_minimumOneWayLatency + (m_randomOneWayLatency * 0.5f); }
		}

        /// <summary>
        /// Получает или задает имитацию количество дубликатов пакетов от 0.0f до 1.0f.
        /// </summary>
        public float SimulatedDuplicatesChance
		{
			get { return m_duplicates; }
			set { m_duplicates = value; }
		}
#endif

        /// <summary>
        /// Создает почленно мелкий клон этой конфигурации.
        /// </summary>
        public NetPeerConfiguration Clone()
		{
			NetPeerConfiguration retval = this.MemberwiseClone() as NetPeerConfiguration;
			retval.m_isLocked = false;
			return retval;
		}
	}

    /// <summary>
    /// Поведение ненадежным посылает выше MTU.
    /// </summary>
    public enum NetUnreliableSizeBehaviour
	{
        /// <summary>
        /// Отправка недостоверную сообщение будет игнорировать MTU и отправить все в одном пакете; это новое значение по умолчанию.
        /// </summary>
        IgnoreMTU = 0,

        /// <summary>
        /// Старый поведение; использовать нормальный фрагментации ненадежных сообщений - если фрагмент упал, память на полученных фрагментов никогда не утилизирован!
        /// </summary>
        NormalFragmentation = 1,

        /// <summary>
        /// Альтернативная поведение; просто падает ненадежные сообщения выше MTU.
        /// </summary>
        DropAboveMTU = 2,
	}
}
