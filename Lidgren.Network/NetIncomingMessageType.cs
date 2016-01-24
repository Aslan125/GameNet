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
using System.Diagnostics.CodeAnalysis;

namespace Lidgren.Network
{
    /// <summary>
    /// Тип чистых входящее сообщение.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
	public enum NetIncomingMessageType
	{
        //
        // Библиотека Примечание: значения мощности из-два, но они не являются флаги - это удобство для типов NetPeer Configuration.Disabled Message
        //

        /// <summary>
        /// Ошибка; это значение никогда не должен появляться
        /// </summary>
        Error = 0,

        /// <summary>
        /// Статус для подключения изменились
        /// </summary>
        StatusChanged = 1 << 0,         // Data (string)

        /// <summary>
        /// Данные, отправленные с помощью Отправить сообщение неподсоединенных
        /// </summary>
        UnconnectedData = 1 << 1,       // Data					Based on data received

        /// <summary>
        /// Утверждение соединения требуется
        /// </summary>
        ConnectionApproval = 1 << 2,    // Data

        /// <summary>
        /// Данные Приложения
        /// </summary>
        Data = 1 << 3,                  // Data					Based on data received

        /// <summary>
        /// Поступление доставки
        /// </summary>
        Receipt = 1 << 4,               // Data

        /// <summary>
        /// Запрос Открытие ответа
        /// </summary>
        DiscoveryRequest = 1 << 5,      // (no data)

        /// <summary>
        /// Ответ Открытие на запрос
        /// </summary>
        DiscoveryResponse = 1 << 6,     // Data

        /// <summary>
        /// Подробный Информационное сообщение
        /// </summary>
        VerboseDebugMessage = 1 << 7,   // Data (string)

        /// <summary>
        /// Отладка сообщение
        /// </summary>
        DebugMessage = 1 << 8,          // Data (string)

        /// <summary>
        /// Предупреждающее сообщение
        /// </summary>
        WarningMessage = 1 << 9,        // Data (string)

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        ErrorMessage = 1 << 10,         // Data (string)

        /// <summary>
        /// Введение NAT успешно
        /// </summary>
        NatIntroductionSuccess = 1 << 11, // Data (as passed to master server)

        /// <summary>
        /// Билета туда и обратно была измерена и NetConnection.Average Туда Время обновлена
        /// </summary>
        ConnectionLatencyUpdated = 1 << 12, // Seconds as a Single
	}
}
