using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SCADA.Common.ImpulsClient
{
	public class FrameParser
	{
		private static int m_maxFrameLength = 64 * 1024;

		private int m_length;

		private byte[] m_data;
		private byte[] m_readyFrame;
		private bool m_frameStarted;
		private bool m_wasEscByte;
		
		/// <summary>
		/// Байт, определяющий пакет с данными
		/// </summary>
		private byte m_dataByte;

		public FrameParser(): this(false)
		{
			
		}

		public FrameParser(bool broadcastFrames)
		{
			m_data = new byte[m_maxFrameLength];
			if(broadcastFrames)
				m_dataByte = (byte)ServiceByte.BroadcastData;
			else
				m_dataByte = (byte)ServiceByte.RegularData;
			Reset();
		}

        private enum ServiceByte
        {
            FrameBounds = 0x7E,
            EscapeByte = 0x7D,
            FrameBoundsEscaped = 0x5E,
            EscapeByteEscaped = 0x5D,
            RegularData = 0x00,
            BroadcastData = 0x80,
            UDPData = 0x85
        }

        /// <summary>
		/// Тип фрейма
		/// </summary>
		public enum FrameType
        {
            Broadcast = 0,
            Regular = 1,
            UDP = 2
        }

        public bool IsFrameReady
		{
			get
			{
				return m_readyFrame != null;
			}
		}

		public void Parse(byte b)
		{
			if(m_wasEscByte)
			{
				switch((ServiceByte)b)
				{
				case ServiceByte.EscapeByteEscaped:
					if(m_length < m_maxFrameLength)
						m_data[m_length++] = (byte)ServiceByte.EscapeByte;
					break;
				case ServiceByte.FrameBoundsEscaped:
					if(m_length < m_maxFrameLength)
						m_data[m_length++] = (byte)ServiceByte.FrameBounds;
					break;
				}
				m_wasEscByte = false;
				return;
			}

			switch((ServiceByte)b)
			{
			case ServiceByte.EscapeByte:
				m_wasEscByte = !m_wasEscByte;
				break;
			case ServiceByte.FrameBounds:
				if(m_frameStarted)
				{
					//если конец фрейма
					if(m_length != 0)
					{
						if(m_length < m_maxFrameLength)
						{
							//если это пакет с данными
							if(m_data[0] == m_dataByte)
							{
								m_readyFrame = new byte[m_length - 1];
								Buffer.BlockCopy(m_data, 1, m_readyFrame, 0, m_length - 1);
							}
						}
						m_frameStarted = false;
					}
					//если длинна равна 0, то это опять начало фрейма
				}
				else
					m_frameStarted = true;
 				m_length = 0;
 				m_wasEscByte = false;
				break;
			default:
				if(!m_frameStarted)
					return;
				
				if(m_length < m_maxFrameLength)
					m_data[m_length++] = (byte)b;
				
				break;
			}
		}

		public byte[] GetFrame()
		{
			byte[] tmp = m_readyFrame;
			m_readyFrame = null;
			return tmp;
		}
		
		public void Reset()
		{
			m_length = 0;
			m_readyFrame = null;
			m_frameStarted = false;
			m_wasEscByte = false;
		}

        public static byte[] MakeFrame(byte[] data, int offset, int length, FrameType frameType)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            //максимальная длинна передаваемых данных должна быть не больше 
            //половины максимальной длинны фрейма, т.к. возможно дублирование 
            //каждого байта
            if (length > m_maxFrameLength / 2 || length <= 0)
                throw new ArgumentOutOfRangeException("length");
            if (offset > data.Length || offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (offset + length > data.Length)
                throw new ArgumentOutOfRangeException("length");
            byte dataByte;
            switch (frameType)
            {
                case FrameType.Broadcast:
                    dataByte = (byte)ServiceByte.BroadcastData;
                    break;
                case FrameType.Regular:
                    dataByte = (byte)ServiceByte.RegularData;
                    break;
                case FrameType.UDP:
                    dataByte = (byte)ServiceByte.UDPData;
                    break;
                default:
                    dataByte = (byte)ServiceByte.RegularData;
                    break;
            }


            byte[] frame;
            //сразу резервирую максимальный буфер
            frame = new byte[3 + m_maxFrameLength];//3 = метка начала + 0x80 + CRC + CRC + метка конца
            int frameLength = 0;
            frame[frameLength++] = (byte)ServiceByte.FrameBounds;

            //устанавливаю байт данных
            frame[frameLength++] = dataByte;

            //формирую выходной фрейм
            for (int i = 0; i < length; i++)
            {
                switch ((ServiceByte)data[offset + i])
                {
                    case ServiceByte.FrameBounds:
                        frame[frameLength++] = (byte)ServiceByte.EscapeByte;
                        frame[frameLength++] = (byte)ServiceByte.FrameBoundsEscaped;
                        break;
                    case ServiceByte.EscapeByte:
                        frame[frameLength++] = (byte)ServiceByte.EscapeByte;
                        frame[frameLength++] = (byte)ServiceByte.EscapeByteEscaped;
                        break;
                    default:
                        frame[frameLength++] = data[offset + i];
                        break;
                }
            }
            frame[frameLength++] = (byte)ServiceByte.FrameBounds;
            //if(frameLength < m_maxFrameLength)

            Array.Resize(ref frame, frameLength);

            return frame;
        }

        public static byte[] MakeFrame(byte[] data, FrameType frame)
        {
            return MakeFrame(data, 0, data.Length, frame);
        }

    }
}
