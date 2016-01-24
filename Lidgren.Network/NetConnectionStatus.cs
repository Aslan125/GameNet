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

namespace Lidgren.Network
{
	/// <summary>
	/// Status for a NetConnection instance
	/// </summary>
	public enum NetConnectionStatus
	{
        /// <summary>
        /// Нет соединения, или попытка, на месте.
        /// </summary>
        None,

        /// <summary>
        /// Подключите был отправлен; ждет Connect ответ.
        /// </summary>
        InitiatedConnect,

        /// <summary>
        /// Подключите было получено, но ответ Подключите не был отправлен еще.
        /// </summary>
        ReceivedInitiation,

        /// <summary>
        /// Подключите было получено сообщение и утверждении выпущен с применением; в ожидании поступления Утвердить () или Запретить ().
        /// </summary>
        RespondedAwaitingApproval, // Мы получили соединение, сообщение релиз утверждения.

        /// <summary>
        /// Подключите было получено и подключение Ответ был отправлен; ждет соединение установлено.
        /// </summary>
        RespondedConnect, // мы получили соединение, послал Подключите Ответ.

        /// <summary>
        /// Связанный
        /// </summary>
        Connected,        // мы получили ответ Connect (инициатора) или соединение установлено (пассивного).

        /// <summary>
        /// В процессе отсоединения.
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Отключен.
        /// </summary>
        Disconnected
    }
}
