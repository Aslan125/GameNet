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
    /// Все константы, используемые при компиляции библиотеки
    /// </summary>
    internal static class NetConstants
	{
		internal const int NumTotalChannels = 99;

		internal const int NetChannelsPerDeliveryMethod = 32;

		internal const int NumSequenceNumbers = 1024;

		internal const int HeaderByteSize = 5;

		internal const int UnreliableWindowSize = 128;
		internal const int ReliableOrderedWindowSize = 64;
		internal const int ReliableSequencedWindowSize = 64;
		internal const int DefaultWindowSize = 64;

		internal const int MaxFragmentationGroups = ushort.MaxValue - 1;

		internal const int UnfragmentedMessageHeaderSize = 5;

        /// <summary>
        /// Количество каналов который нужен Порядковый номер работы
        /// </summary>
        internal const int NumSequencedChannels = ((int)NetMessageType.UserReliableOrdered1 + NetConstants.NetChannelsPerDeliveryMethod) - (int)NetMessageType.UserSequenced1;

        /// <summary>
        /// Число надежных каналов
        /// </summary>
        internal const int NumReliableChannels = ((int)NetMessageType.UserReliableOrdered1 + NetConstants.NetChannelsPerDeliveryMethod) - (int)NetMessageType.UserReliableUnordered;
		
		internal const string ConnResetMessage = "Connection was reset by remote host";
	}
}
