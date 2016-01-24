using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lidgren.Network
{
	public partial class NetBuffer
	{
        /// <summary>
        /// Количество байтов в течение выделять для каждого сообщения, чтобы избежать изменения размера.
        /// </summary>
        protected const int c_overAllocateAmount = 4;

		private static readonly Dictionary<Type, MethodInfo> s_readMethods;
		private static readonly Dictionary<Type, MethodInfo> s_writeMethods;

		internal byte[] m_data;
		internal int m_bitLength;
		internal int m_readPosition;

        /// <summary>
        /// Получает или задает внутренний буфер данных.
        /// </summary>
        public byte[] Data
		{
			get { return m_data; }
			set { m_data = value; }
		}

        /// <summary>
        /// Получает или устанавливает длину используемого части буфера в байтах.
        /// </summary>
        public int LengthBytes
		{
			get { return ((m_bitLength + 7) >> 3); }
			set
			{
				m_bitLength = value * 8;
				InternalEnsureBufferSize(m_bitLength);
			}
		}

        /// <summary>
        /// Получает или устанавливает длину используемого части буфера в битах.
        /// </summary>
        public int LengthBits
		{
			get { return m_bitLength; }
			set
			{
				m_bitLength = value;
				InternalEnsureBufferSize(m_bitLength);
			}
		}

        /// <summary>
        /// Получает или задает позицию чтения в буфере, в битах (не байт).
        /// </summary>
        public long Position
		{
			get { return (long)m_readPosition; }
			set { m_readPosition = (int)value; }
		}

        /// <summary>
        /// Получает позицию в буфере в байтах; обратите внимание, что биты первого байта возвращается, возможно, уже читали - проверить свойство Позиция, чтобы убедиться.
        /// </summary>
        public int PositionInBytes
		{
			get { return (int)(m_readPosition / 8); }
		}
		
		static NetBuffer()
		{
			s_readMethods = new Dictionary<Type, MethodInfo>();
			MethodInfo[] methods = typeof(NetIncomingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo mi in methods)
			{
				if (mi.GetParameters().Length == 0 && mi.Name.StartsWith("Read", StringComparison.InvariantCulture) && mi.Name.Substring(4) == mi.ReturnType.Name)
				{
					s_readMethods[mi.ReturnType] = mi;
				}
			}

			s_writeMethods = new Dictionary<Type, MethodInfo>();
			methods = typeof(NetOutgoingMessage).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo mi in methods)
			{
				if (mi.Name.Equals("Write", StringComparison.InvariantCulture))
				{
					ParameterInfo[] pis = mi.GetParameters();
					if (pis.Length == 1)
						s_writeMethods[pis[0].ParameterType] = mi;
				}
			}
		}
	}
}
