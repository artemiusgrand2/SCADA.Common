using System;
using System.Collections.Generic;
using System.Text;
using SCADA.Common.ImpulsClient.requests;

namespace SCADA.Common.ImpulsClient
{
    class TableParser
    {
        static public unsafe ImpulsesAnswer ParseTablesAnswer(byte[] answer)
        {
            ImpulsesAnswer answerParsed = new ImpulsesAnswer();
            
            //если получил неправильный пакет
            if (answer.Length < RequestHeader.Size)
                return null;

            ImpulsesAnswerHeader* answerHeader;
			fixed(byte* pAnswer = answer)
            {
                answerHeader = (ImpulsesAnswerHeader*)pAnswer;
            }
            if (answerHeader->Header.PacketType != (int)AnswerType.StationTables)
                return null;
            if (answerHeader->Header.PacketSize != answer.Length)
                return null;

            if (answerHeader->TSCount == 0 && answerHeader->TUCount == 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("for station {0} received 0 TS impulses and 0 TU impulses", answerHeader->StationID));
                return null;
            }
            //определяю размер таблиц импульсов
            int buffLength = ImpulsesAnswerHeader.Size
                + answerHeader->TSCount / 4
                + ((answerHeader->TSCount % 4 == 0) ? (0) : (1))
                + (answerHeader->TUCount * 3) / 8
                + (((answerHeader->TUCount * 3) % 8 == 0) ? (0) : (1));
            //если не совпадает с длинной принятого буфера, то не обрабатывать
            if (answer.Length != buffLength)
                return null;
            bool tableActive = false;
            //заполняю таблицы
            fixed (byte* pImpulsesArray = &answer[ImpulsesAnswerHeader.Size])
            {
                byte* pBuff = pImpulsesArray;
				
//				System.Console.WriteLine("############## BYTES ###############");
//				
//				System.Console.WriteLine("############## TC ###############");
				
                //ТС
				
                byte[] impulses = new byte[answerHeader->TSCount];
                int* pTmp = (int*)pBuff;
                int shift = 0;
				int _jumps = 0;
                try
                {
                    for (int impIndex = 0; impIndex < answerHeader->TSCount; impIndex++)
                    {
                        //если обработал байт, то перехожу на следующий
                        if (shift == 32)
                        {
                            shift = 0;
							pTmp++;
							_jumps ++;
                        }
                        //по 2 бита на импульс, от младших к старшим
                        impulses[impIndex] = (byte)((*pTmp >> shift) & 0x03);
						if ((shift % 8 == 0)&(shift != 0))
//							System.Console.WriteLine();
//						System.Console.Write(impulses[impIndex] + " ");
                        if (impulses[impIndex] == (byte)ImpulseState.ActiveState
                            || impulses[impIndex] == (byte)ImpulseState.PassiveState)
                        {
                            tableActive = true;
                        }
                        shift += 2;
                    }
                }
                catch (SystemException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return null;
                }
//                System.Console.WriteLine();
//				System.Console.WriteLine("###############TU################");
				
				Array.Resize(ref answerParsed.TsImpulses, answerHeader->TSCount);
                answerParsed.TsImpulses = impulses;
                
				if (shift >= 0)
					_jumps ++;
				
                //если для этой станции есть таблица ТУ, то заполнить и её
                byte[] _tu_impulses = new byte[answerHeader->TUCount];
				int _tu_pos = answerHeader->TSCount / 4
                    + ((answerHeader->TSCount % 4 == 0) ? (0) : (1));
                //импульсы начинаются после последнего байта массива ТС
                pTmp = (int*)&pBuff[_tu_pos];
				pBuff += _tu_pos;
                shift = 0;
				try
                {
                    for (int impIndex = 0; impIndex < answerHeader->TUCount; impIndex++)
                    {
                        if (shift == 24)
                        {
                            shift = 0;
                            pBuff += 3;
                            pTmp = (int*)pBuff;
                        }
                        //по 3 бита на импульс, от младших к старшим
						byte _state = (byte)((*pTmp >> shift) & 7);
                        _tu_impulses[impIndex] = _state;
						if ((shift % 12 == 0)&(shift != 0))
//							System.Console.WriteLine();
//						System.Console.Write(_tu_impulses[impIndex] + " ");
                        shift += 3;
                    }
                }
                catch (SystemException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return null;
                }
     
//				if (tableActive)
//                {
                    Array.Resize(ref answerParsed.TuImpulses, answerHeader->TUCount);
                    answerParsed.TuImpulses = _tu_impulses;
//                }
            }

            answerParsed.Header = *answerHeader;

            return answerParsed;
        }
    }
}
