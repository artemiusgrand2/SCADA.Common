using System;
using System.Collections.Generic;
using System.Text;
using SCADA.Common.ImpulsClient.requests;

namespace SCADA.Common.ImpulsClient
{
    class TimeParser
    {
        static public unsafe DateTime ParseTablesAnswer(byte[] answer)
        {
            //если получил неправильный пакет
            if (answer.Length < RequestHeader.Size)
                return DateTime.MinValue;

            TimeAnswer* answerHeader;
            fixed (byte* pAnswer = answer)
            {
                answerHeader = (TimeAnswer*)pAnswer;
            }
            if (answerHeader->Header.PacketType != (int)AnswerType.Time)
                return DateTime.MinValue;
            if (answerHeader->Header.PacketSize != answer.Length)
                return DateTime.MinValue;
            //
            return new DateTime(answerHeader->Time);
        }
    }
}
