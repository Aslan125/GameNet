using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;

#if !__NOIPENDPOINT__
using NetEndPoint = System.Net.IPEndPoint;
#endif

namespace Lidgren.Network
{
    /// <summary>
    /// Представляет локальный сверстников, способный удерживать ноль, один или несколько подключений к удаленным сверстниками.
    /// </summary>
    public partial class NetPeer
	{
		private static int s_initializedPeersCount;

		private int m_listenPort;
		private object m_tag;
		private object m_messageReceivedEventCreationLock = new object();

		internal readonly List<NetConnection> m_connections;
		private readonly Dictionary<NetEndPoint, NetConnection> m_connectionLookup;

		private string m_shutdownReason;

        /// <summary>
        /// Получает NetPeerStatus из NetPeer
        /// </summary>
        public NetPeerStatus Status { get { return m_status; } }

        /// <summary>
        /// Сигнализации событие, которое может быть ждали на, чтобы определить, когда сообщение помещается в очередь для чтения.
        /// Обратите внимание, что нет никакой гарантии, что после того, как событие сигнализируется заблокированный поток будет
        /// Найти сообщение в очереди. Другие созданные пользователем темы может быть вытеснен из очереди и
        /// Сообщение до ожидающего потока просыпается.
        /// </summary>
        public AutoResetEvent MessageReceivedEvent
		{
			get
			{
				if (m_messageReceivedEvent == null)
				{
					lock (m_messageReceivedEventCreationLock) // make sure we don't create more than one event object
					{
						if (m_messageReceivedEvent == null)
							m_messageReceivedEvent = new AutoResetEvent(false);
					}
				}
				return m_messageReceivedEvent;
			}
		}

        /// <summary>
        /// Возвращает уникальный идентификатор для этого NetPeer на основе МАС-адресов и IP / порт. Заметка! Не доступен до Пуск () была вызвана!
        /// </summary>
        public long UniqueIdentifier { get { return m_uniqueIdentifier; } }

        /// <summary>
        /// Получает номер порта этого NetPeer слушает и отправки на, если старт () была вызвана
        /// </summary>
        public int Port { get { return m_listenPort; } }

        /// <summary>
        /// Возвращает объект UPnP, если включена в настройках NetGear
        /// </summary>
        public NetUPnP UPnP { get { return m_upnp; } }

        /// <summary>
        /// Получает или задает приложений определяется объект, содержащий данные о сверстников
        /// </summary>
        public object Tag
		{
			get { return m_tag; }
			set { m_tag = value; }
		}

        /// <summary>
        /// Получает копию списка связей
        /// </summary>
        public List<NetConnection> Connections
		{
			get
			{
				lock (m_connections)
					return new List<NetConnection>(m_connections);
			}
		}

        /// <summary>
        /// Получает количество активных соединений
        /// </summary>
        public int ConnectionsCount
		{
			get { return m_connections.Count; }
		}

        /// <summary>
        /// Статистика по этой NetPeer, поскольку она была инициализирована
        /// </summary>
        public NetPeerStatistics Statistics
		{
			get { return m_statistics; }
		}

        /// <summary>
        /// Получает конфигурацию, используемый для создания экземпляра этого NetPeer
        /// </summary>
        public NetPeerConfiguration Configuration { get { return m_configuration; } }

        
        /// <summary>
        /// NetPeer конструктор
        /// </summary>
        public NetPeer(NetPeerConfiguration config)
		{
			m_configuration = config;
			m_statistics = new NetPeerStatistics(this);
			m_releasedIncomingMessages = new NetQueue<NetIncomingMessage>(4);
			m_unsentUnconnectedMessages = new NetQueue<NetTuple<NetEndPoint, NetOutgoingMessage>>(2);
			m_connections = new List<NetConnection>();
			m_connectionLookup = new Dictionary<NetEndPoint, NetConnection>();
			m_handshakes = new Dictionary<NetEndPoint, NetConnection>();
			m_senderRemote = (EndPoint)new NetEndPoint(IPAddress.Any, 0);
			m_status = NetPeerStatus.NotRunning;
			m_receivedFragmentGroups = new Dictionary<NetConnection, Dictionary<int, ReceivedFragmentGroup>>();	
		}

        /// <summary>
        /// Привязка к розетке и порождает сетевую нить
        /// </summary>
        public void Start()
		{
			if (m_status != NetPeerStatus.NotRunning)
			{
				// already running! Just ignore...
				LogWarning("Start() called on already running NetPeer - ignoring.");
				return;
			}

			m_status = NetPeerStatus.Starting;

			// fix network thread name
			if (m_configuration.NetworkThreadName == "Lidgren network thread")
			{
				int pc = Interlocked.Increment(ref s_initializedPeersCount);
				m_configuration.NetworkThreadName = "Lidgren network thread " + pc.ToString();
			}

			InitializeNetwork();
			
			// start network thread
			m_networkThread = new Thread(new ThreadStart(NetworkLoop));
			m_networkThread.Name = m_configuration.NetworkThreadName;
			m_networkThread.IsBackground = true;
			m_networkThread.Start();

			// send upnp discovery
			if (m_upnp != null)
				m_upnp.Discover(this);

			// allow some time for network thread to start up in case they call Connect() or UPnP calls immediately
			NetUtility.Sleep(50);
		}

        /// <summary>
        /// Получить соединение, если таковые имеются, в течение определенного удаленной точке
        /// </summary>
        public NetConnection GetConnection(NetEndPoint ep)
		{
			NetConnection retval;

			// this should not pose a threading problem, m_connectionLookup is never added to concurrently
			// and TryGetValue will not throw an exception on fail, only yield null, which is acceptable
			m_connectionLookup.TryGetValue(ep, out retval);

			return retval;
		}

        /// <summary>
        /// Читайте отложенный сообщение от какой-либо связи, блокируя до maxMillis при необходимости
        /// </summary>
        public NetIncomingMessage WaitMessage(int maxMillis)
	        {
	            NetIncomingMessage msg = ReadMessage();
	
	            while (msg == null)
	            {
	                // This could return true...
	                if (!MessageReceivedEvent.WaitOne(maxMillis))
	                {
	                    return null;
	                }
	
	                // ... while this will still returns null. That's why we need to cycle.
	                msg = ReadMessage();
	            }
	
	            return msg;
        	}

        /// <summary>
        /// Читайте отложенный сообщение от всякой связи, если таковые
        /// </summary>
        public NetIncomingMessage ReadMessage()
		{
			NetIncomingMessage retval;
			if (m_releasedIncomingMessages.TryDequeue(out retval))
			{
				if (retval.MessageType == NetIncomingMessageType.StatusChanged)
				{
					NetConnectionStatus status = (NetConnectionStatus)retval.PeekByte();
					retval.SenderConnection.m_visibleStatus = status;
				}
			}
			return retval;
		}

        /// <summary>
        /// Читайте отложенный сообщение от любой связи, если таковые имеются.
        /// </summary>
        public int ReadMessages(IList<NetIncomingMessage> addTo)
		{
			int added = m_releasedIncomingMessages.TryDrain(addTo);
			if (added > 0)
			{
				for (int i = 0; i < added; i++)
				{
					var index = addTo.Count - added + i;
					var nim = addTo[index];
					if (nim.MessageType == NetIncomingMessageType.StatusChanged)
					{
						NetConnectionStatus status = (NetConnectionStatus)nim.PeekByte();
						nim.SenderConnection.m_visibleStatus = status;
					}
				}
			}
			return added;
		}

        // отправить сообщение немедленно и утилизировать его.
        internal void SendLibrary(NetOutgoingMessage msg, NetEndPoint recipient)
		{
			VerifyNetworkThread();
			NetException.Assert(msg.m_isSent == false);

			bool connReset;
			int len = msg.Encode(m_sendBuffer, 0, 0);
			SendPacket(len, recipient, 1, out connReset);

			// no reliability, no multiple recipients - we can just recycle this message immediately
			msg.m_recyclingCount = 0;
			Recycle(msg);
		}

		static NetEndPoint GetNetEndPoint(string host, int port)
		{
			IPAddress address = NetUtility.Resolve(host);
			if (address == null)
				throw new NetException("Could not resolve host");
			return new NetEndPoint(address, port);
		}

        /// <summary>
        /// Создание подключения к удаленной точке.
        /// </summary>
        public NetConnection Connect(string host, int port)
		{
			return Connect(GetNetEndPoint(host, port), null);
		}

        /// <summary>
        /// Создание подключения к удаленной точке.
        /// </summary>
        public NetConnection Connect(string host, int port, NetOutgoingMessage hailMessage)
		{
			return Connect(GetNetEndPoint(host, port), hailMessage);
		}

        /// <summary>
        /// Создание подключения к удаленной точке.
        /// </summary>
        public NetConnection Connect(NetEndPoint remoteEndPoint)
		{
			return Connect(remoteEndPoint, null);
		}

        /// <summary>
        /// Создание подключения к удаленной точке.
        /// </summary>
        public virtual NetConnection Connect(NetEndPoint remoteEndPoint, NetOutgoingMessage hailMessage)
		{
			if (remoteEndPoint == null)
				throw new ArgumentNullException("remoteEndPoint");

			lock (m_connections)
			{
				if (m_status == NetPeerStatus.NotRunning)
					throw new NetException("Must call Start() first");

				if (m_connectionLookup.ContainsKey(remoteEndPoint))
					throw new NetException("Already connected to that endpoint!");

				NetConnection hs;
				if (m_handshakes.TryGetValue(remoteEndPoint, out hs))
				{
					// already trying to connect to that endpoint; make another try
					switch (hs.m_status)
					{
						case NetConnectionStatus.InitiatedConnect:
							// send another connect
							hs.m_connectRequested = true;
							break;
						case NetConnectionStatus.RespondedConnect:
							// send another response
							hs.SendConnectResponse(NetTime.Now, false);
							break;
						default:
							// weird
							LogWarning("Weird situation; Connect() already in progress to remote endpoint; but hs status is " + hs.m_status);
							break;
					}
					return hs;
				}

				NetConnection conn = new NetConnection(this, remoteEndPoint);
				conn.m_status = NetConnectionStatus.InitiatedConnect;
				conn.m_localHailMessage = hailMessage;

				// handle on network thread
				conn.m_connectRequested = true;
				conn.m_connectionInitiator = true;

				m_handshakes.Add(remoteEndPoint, conn);

				return conn;
			}
		}

        /// <summary>
        /// Отправить необработанные байты; используется только для отладки.
        /// </summary>
        public void RawSend(byte[] arr, int offset, int length, NetEndPoint destination)
		{
			// wrong thread - this miiiight crash with network thread... but what's a boy to do.
			Array.Copy(arr, offset, m_sendBuffer, 0, length);
			bool unused;
			SendPacket(length, destination, 1, out unused);
		}

        /// <summary>
        /// В DEBUG, бросает исключение, в релизе регистрирует сообщение об ошибке.
        /// </summary>
        /// <param name="message"></param>
        internal void ThrowOrLog(string message)
		{
#if DEBUG
			throw new NetException(message);
#else
			LogError(message);
#endif
		}

        /// <summary>
        /// Отключите все активные соединения и закрывает сокет.
        /// </summary>
        public void Shutdown(string bye)
		{
			// called on user thread
			if (m_socket == null)
				return; // already shut down

			LogDebug("Shutdown requested");
			m_shutdownReason = bye;
			m_status = NetPeerStatus.ShutdownRequested;
		}
	}
}
