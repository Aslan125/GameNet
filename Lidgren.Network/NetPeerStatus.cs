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
	/// Status for a NetPeer instance
	/// </summary>
	public enum NetPeerStatus
	{
        /// <summary>
        /// NetPeer не работает; сокет не привязан
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// NetPeer находится в процессе запуска
        /// </summary>
        Starting = 1,

        /// <summary>
        /// NetPeer привязан сокет и прослушивает пакеты
        /// </summary>
        Running = 2,

        /// <summary>
        /// Завершение работы было предложено и будет исполнен в ближайшее время
        /// </summary>
        ShutdownRequested = 3,
	}
}
