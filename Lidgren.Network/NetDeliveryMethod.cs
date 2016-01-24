using System;
using System.Collections.Generic;
using System.Text;

namespace Lidgren.Network
{
    /// <summary>
    /// Как библиотека предложения с отправляет и обработка сообщений конце
    /// </summary>
    public enum NetDeliveryMethod : byte
	{
		//
		// Actually a publicly visible subset of NetMessageType
		//

		/// <summary>
		/// Indicates an error
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Unreliable, unordered delivery
		/// </summary>
		Unreliable = 1,

        /// <summary>
        /// Ненадежный доставки, но автоматически удалять сообщения поздно
        /// </summary>
        UnreliableSequenced = 2,

        /// <summary>
        /// Расписание доставки, но неупорядоченный
        /// </summary>
        ReliableUnordered = 34,

        /// <summary>
        /// Расписание доставка, для поздних сообщений, за исключением, которые упали
        /// </summary>
        ReliableSequenced = 35,

        /// <summary>
        /// Расписание, приказал доставка
        /// </summary>
        ReliableOrdered = 67,
	}
}
