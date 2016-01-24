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
using System.Text;

namespace Lidgren.Network
{
    /// <summary>
    /// Фиксированный размер вектор логических значений...
    /// </summary>
    public sealed class NetBitVector
	{
		private readonly int m_capacity;
		private readonly int[] m_data;
		private int m_numBitsSet;

        /// <summary>
        /// Получает количество бит / логических значений, сохраненных в этом векторе.
        /// </summary>
        public int Capacity { get { return m_capacity; } }

        /// <summary>
        /// NetBit Вектор конструктор.
        /// </summary>
        public NetBitVector(int bitsCapacity)
		{
			m_capacity = bitsCapacity;
			m_data = new int[(bitsCapacity + 31) / 32];
		}

        /// <summary>
        /// Возвращает истину, если все биты / логические устанавливаются в ноль / ложь.
        /// </summary>
        public bool IsEmpty()
		{
			return (m_numBitsSet == 0);
		}

        /// <summary>
        /// Возвращает количество бит / Булев, установленных друг / истинной.
        /// </summary>
        /// <returns></returns>
        public int Count()
		{
			return m_numBitsSet;
		}

        /// <summary>
        /// Сдвиг всех битов на один шаг вниз, езда на велосипеде первый бит в начало...
        /// </summary>
        public void RotateDown()
		{
			int lenMinusOne = m_data.Length - 1;

			int firstBit = m_data[0] & 1;
			for (int i = 0; i < lenMinusOne; i++)
				m_data[i] = ((m_data[i] >> 1) & ~(1 << 31)) | m_data[i + 1] << 31;

			int lastIndex = m_capacity - 1 - (32 * lenMinusOne);

			// special handling of last int
			int cur = m_data[lenMinusOne];
			cur = cur >> 1;
			cur |= firstBit << lastIndex;

			m_data[lenMinusOne] = cur;
		}

        /// <summary>
        /// Получает первый (самый низкий) множество индексов к истине.
        /// </summary>
        public int GetFirstSetIndex()
		{
			int idx = 0;

			int data = m_data[0];
			while (data == 0)
			{
				idx++;
				data = m_data[idx];
			}

			int a = 0;
			while (((data >> a) & 1) == 0)
				a++;

			return (idx * 32) + a;
		}

        /// <summary>
        /// Получает большую книгу / по указанному индексу.
        /// </summary>
        public bool Get(int bitIndex)
		{
			NetException.Assert(bitIndex >= 0 && bitIndex < m_capacity);

			return (m_data[bitIndex / 32] & (1 << (bitIndex % 32))) != 0;
		}

        /// <summary>
        /// Устанавливает или сбрасывает большую книгу / по указанному индексу...
        /// </summary>
        public void Set(int bitIndex, bool value)
		{
			NetException.Assert(bitIndex >= 0 && bitIndex < m_capacity);

			int idx = bitIndex / 32;
			if (value)
			{
				if ((m_data[idx] & (1 << (bitIndex % 32))) == 0)
					m_numBitsSet++;
				m_data[idx] |= (1 << (bitIndex % 32));
			}
			else
			{
				if ((m_data[idx] & (1 << (bitIndex % 32))) != 0)
					m_numBitsSet--;
				m_data[idx] &= (~(1 << (bitIndex % 32)));
			}
		}

        /// <summary>
        /// Получает большую книгу / по указанному индексу.
        /// </summary>
        [System.Runtime.CompilerServices.IndexerName("Bit")]
		public bool this[int index]
		{
			get { return Get(index); }
			set { Set(index, value); }
		}

        /// <summary>
        /// Устанавливает все биты / булевы к нулю / ложной.
        /// </summary>
        public void Clear()
		{
			Array.Clear(m_data, 0, m_data.Length);
			m_numBitsSet = 0;
			NetException.Assert(this.IsEmpty());
		}

		/// <summary>
		/// Returns a string that represents this object
		/// </summary>
		public override string ToString()
		{
			StringBuilder bdr = new StringBuilder(m_capacity + 2);
			bdr.Append('[');
			for (int i = 0; i < m_capacity; i++)
				bdr.Append(Get(m_capacity - i - 1) ? '1' : '0');
			bdr.Append(']');
			return bdr.ToString();
		}
	}
}
