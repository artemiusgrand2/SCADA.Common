using System;
using System.Collections.Generic;
using System.Text;

namespace SCADA.Common.ImpulsClient.requests
{
    public class RequestHandler
    {
        private DataContainer _dataContainer;

        public RequestHandler(DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
        }

        /// <summary>
        /// Обработать запрос
        /// </summary>
        /// <param name="requestBuffer">Буфер запроса</param>
        /// <returns>Список с буферами ответов</returns>
        public IList<byte[]> HandleRequest(byte[] requestBuffer)
        {
            // проверяю размер запроса
            if (requestBuffer.Length < RequestHeader.Size)
                return HandleError(RequestType.Unknown, RequestError.UnexpectedError);

            // получаю заголовок запроса
            unsafe
            {
                fixed (byte* pBuff = requestBuffer)
                {
                    RequestHeader* pRequestHeader = (RequestHeader*)pBuff;
                    if (pRequestHeader->PacketSize != requestBuffer.Length)
                        return HandleError(RequestType.Unknown, RequestError.WrongRequest);

                    switch (pRequestHeader->PacketType)
                    {
                        case (int)RequestType.StationTables:
                            if (requestBuffer.Length != ImpulsesRequest.Size)
                                PackError(RequestType.StationTables, RequestError.WrongRequest);

                            return HandleStationTables(requestBuffer);

                        case (int)RequestType.Command:
                            if (requestBuffer.Length != CommandRequest.Size)
                                return HandleError(RequestType.Command, RequestError.WrongRequest);

                            return HandleCommand(requestBuffer);

                        case (int)RequestType.ListOfTables:
                            if (requestBuffer.Length < TablesListRequestHeader.Size)
                                return HandleError(RequestType.ListOfTables, RequestError.UnknownRequest);

                            TablesListRequestHeader* lstHdr = (TablesListRequestHeader*)pBuff;
                            if (requestBuffer.Length != lstHdr->StationsCount * 4 + TablesListRequestHeader.Size)
                                return HandleError(RequestType.ListOfTables, RequestError.WrongRequest);

                            return HandleTablesList(requestBuffer);

                        default:
                            return HandleError((RequestType)pRequestHeader->PacketType, RequestError.UnknownRequest);
                    }
                }
            }
        }

        #region private methods



        /// <summary>
		/// Обработать запрос с командой
		/// </summary>
		/// <param name="commandBuffer">Буффер запроса</param>
		/// <returns>Список с буфером ответа</returns>
		private unsafe List<byte[]> HandleCommand(byte[] commandBuffer)
        {
            CommandRequest* pCommand;
            fixed (byte* pTmp = commandBuffer)
            {
                pCommand = (CommandRequest*)pTmp;
            }
            string sender = PointerToString(pCommand->SenderID, CommandRequest.SenderIDLength, Encoding.Unicode);
            string serverName = System.Net.Dns.GetHostName();

            Station station = _dataContainer.GetStation(pCommand->StationID);
            if (station == null)
                return PackCommandAnswerList(commandBuffer, RequestError.UnknownStation);
            //
            string imp = PointerToString(pCommand->CommandID, CommandRequest.CommandIDLength, Encoding.Unicode);
            string send_ID = PointerToString(pCommand->SenderID, CommandRequest.SenderIDLength, Encoding.Unicode);
            //
            byte[] answer = null;
            RequestError error;
            fixed (byte* pBuff = answer)
            {
                CommandRequest* request = (CommandRequest*)pBuff;
                serverName = PointerToString(request->SenderID,
                    CommandRequest.SenderIDLength, Encoding.Unicode);
                error = (RequestError)request->CommandValue;

            }
            //
            List<byte[]> result = new List<byte[]>(1);
            result.Add(FrameParser.MakeFrame(answer, FrameParser.FrameType.Regular));

            return result;
        }

        /// <summary>
		/// Собрать ответ на отправление импульса
		/// </summary>
		/// <param name="stationCode">Код станции на которую отправлялся импульс</param>
		/// <param name="impulse">Название импульса</param>
		/// <param name="error">Результат отправления</param>
		/// <returns>Буфер с результатом отправления</returns>
		public static unsafe List<byte[]> PackCommandAnswerList(byte[] commandBuffer, RequestError error)
        {
            byte[] answerBuffer = new byte[CommandRequest.Size];
            answerBuffer = PackCommandAnswer(commandBuffer, error);
            /*fixed(byte* pAnswerBuffer = answerBuffer, pCommandBuffer = commandBuffer)
			{
				CommandRequest* pAnswer = (CommandRequest*)pAnswerBuffer;
				CommandRequest* pCommand = (CommandRequest*)pCommandBuffer;

				pAnswer->Header.PacketType = (int)AnswerType.Command;
				pAnswer->Header.PacketSize = CommandRequest.Size;

				//в качестве идентификатора использую имя компьютера
				byte[] tmp = Encoding.Unicode.GetBytes(Program.Settings.ComputerName);
				int count = (tmp.Length < CommandRequest.SenderIDLength) ? (tmp.Length) : (CommandRequest.SenderIDLength);
				for(int i = 0; i < count; i++)
					pAnswer->SenderID[i] = tmp[i];

				pAnswer->StationID = pCommand->StationID;
				pAnswer->CommandValue = (short)error;
				CopyBytes(pCommand->CommandID, pAnswer->CommandID, CommandRequest.CommandIDLength);
			}*/

            List<byte[]> result = new List<byte[]>(1);
            result.Add(FrameParser.MakeFrame(answerBuffer, FrameParser.FrameType.Regular));
            return result;
        }


        public static unsafe byte[] PackCommandAnswer(byte[] commandBuffer, RequestError error)
        {
            byte[] answerBuffer = new byte[CommandRequest.Size];
            fixed (byte* pAnswerBuffer = answerBuffer, pCommandBuffer = commandBuffer)
            {
                CommandRequest* pAnswer = (CommandRequest*)pAnswerBuffer;
                CommandRequest* pCommand = (CommandRequest*)pCommandBuffer;

                pAnswer->Header.PacketType = (int)AnswerType.Command;
                pAnswer->Header.PacketSize = CommandRequest.Size;

                //в качестве идентификатора использую имя компьютера
                byte[] tmp = Encoding.Unicode.GetBytes(System.Net.Dns.GetHostName());
                int count = (tmp.Length < CommandRequest.SenderIDLength) ? (tmp.Length) : (CommandRequest.SenderIDLength);
                for (int i = 0; i < count; i++)
                    pAnswer->SenderID[i] = tmp[i];

                pAnswer->StationID = pCommand->StationID;
                pAnswer->CommandValue = (short)error;
                CopyBytes(pCommand->CommandID, pAnswer->CommandID, CommandRequest.CommandIDLength);
            }

            return answerBuffer;
        }

        ///// <summary>
        ///// Собрать запрос на отправление импульса
        ///// </summary>
        ///// <param name="stationCode">Код станции на которую отправлялся импульс</param>
        ///// <param name="impulse">Название импульса</param>
        ///// <param name="error">Результат отправления</param>
        ///// <returns>Буфер с результатом отправления (не упакованный)</returns>
        //private unsafe byte[] PackCommandAnswer(int stationCode, string impulse)
        //{
        //    byte[] answer = new byte[CommandRequest.Size];
        //    fixed (byte* pAnswer = answer)
        //    {
        //        CommandRequest* result = (CommandRequest*)pAnswer;
        //        result->Header.PacketType = (int)RequestType.Command;
        //        result->Header.PacketSize = CommandRequest.Size;
        //        //в качестве идентификатора использую имя компьютера
        //        byte[] tmp = Encoding.Unicode.GetBytes(System.Net.Dns.GetHostName());
        //        int count = (tmp.Length < CommandRequest.SenderIDLength) ? (tmp.Length) : (CommandRequest.SenderIDLength);
        //        for (int i = 0; i < count; i++)
        //            result->SenderID[i] = tmp[i];

        //        result->StationID = stationCode;
        //        result->CommandValue = (short)0x0001;
        //        //заполняю команду
        //        tmp = Encoding.Unicode.GetBytes(impulse);
        //        count = (tmp.Length < CommandRequest.CommandIDLength) ? (tmp.Length) : (CommandRequest.CommandIDLength);
        //        for (int i = 0; i < count; i++)
        //            result->CommandID[i] = tmp[i];
        //    }
        //    return answer;
        //}

        /// <summary>
        /// Обработать запрос с таблицами импульсов станции
        /// </summary>
        /// <param name="stationBuffer">Буффер запроса</param>
        /// <returns>Список с буфером ответа</returns>
        private unsafe List<byte[]> HandleStationTables(byte[] stationBuffer)
        {
            Station station = null;
            fixed (byte* pBuff = stationBuffer)
            {
                ImpulsesRequest* request = (ImpulsesRequest*)pBuff;
                station = _dataContainer.GetStation(request->StationID);
            }

            if (station == null)
                return HandleError(RequestType.StationTables, RequestError.UnknownStation);

            List<byte[]> result = new List<byte[]>();
            result.Add(FrameParser.MakeFrame(PackTablesBuffer(station), FrameParser.FrameType.Regular));
            return result;
        }

        /// <summary>
        /// Обработать запрос с таблицами импульсов нескольких станций
        /// </summary>
        /// <param name="stationBuffer">Буффер запроса</param>
        /// <returns>Список с буферами ответов</returns>
        private unsafe List<byte[]> HandleTablesList(byte[] stationsBuffer)
        {
            fixed (byte* pBuff = stationsBuffer)
            {
                TablesListRequestHeader* request = (TablesListRequestHeader*)pBuff;
                int* stationID = (int*)&request[1];
                List<byte[]> result = new List<byte[]>(request->StationsCount);
                for (int i = 0; i < request->StationsCount; i++)
                {
                    Station station = _dataContainer.GetStation(stationID[i]); 
                    if (station == null)
                        result.Add(PackError(RequestType.StationTables, RequestError.UnknownStation));
                    else
                        result.Add(PackTablesBuffer(station));
                }

                return result;
            }
        }

        /// <summary>
        /// Обработать ошибку
        /// </summary>
        /// <param name="request">Тип запроса</param>
        /// <param name="error">Тип ошибки</param>
        /// <returns>Список с буфером ответа</returns>
        private List<byte[]> HandleError(RequestType request, RequestError error)
        {
            List<byte[]> result = new List<byte[]>();
            result.Add(PackError(request, error));
            return result;
        }

        /// <summary>
        /// Упаковать ответ об ошибке выполнения запроса
        /// </summary>
        /// <param name="request">Тип ошибочного запроса</param>
        /// <param name="error">Ошибка выполнения</param>
        /// <returns>Буфер с ответом упакованный для отправки клиенту</returns>
        private byte[] PackError(RequestType request, RequestError error)
        {
            byte[] buff = new byte[ErrorRequestAnswer.Size];
            unsafe
            {
                fixed (byte* pBuff = buff)
                {
                    ErrorRequestAnswer* answer = (ErrorRequestAnswer*)pBuff;
                    answer->Header.PacketType = (short)AnswerType.Error;
                    answer->Header.PacketSize = ErrorRequestAnswer.Size;
                    answer->Error = (short)error;
                    answer->FailedRequestType = (short)request;
                }
            }

            return FrameParser.MakeFrame(buff, FrameParser.FrameType.Regular);
        }


        /// <summary>
        /// Упаковать таблицы импульсов станции
        /// </summary>
        /// <param name="station">Станция</param>
        /// <returns>Буфер с таблицами упакованный для отправки клиенту</returns>
        private unsafe byte[] PackTablesBuffer(Station station)
        {
            // определяю размер буфера ответа
            // 2 бита на импульс ТС, 3 бита на импульс ТУ
            int buffLength = ImpulsesAnswerHeader.Size
                + station.TS.Count / 4
                + ((station.TS.Count % 4 == 0) ? (0) : (1))
                + (station.TU.Count * 3) / 8
                + (((station.TU.Count * 3) % 8 == 0) ? (0) : (1));
            byte[] answerBuffer = new byte[buffLength];

            fixed (byte* pBuff = answerBuffer)
            {
                ImpulsesAnswerHeader* answer = (ImpulsesAnswerHeader*)pBuff;
                answer->Header.PacketType = (short)AnswerType.StationTables;
                answer->Header.PacketSize = (short)buffLength;
                answer->StationID = station.Code;
                answer->TSCount = (short)station.TS.Count;
                answer->TUCount = (short)station.TU.Count;

                // заполняю массив импульсов ТС (по 2 бита на импульс)
                int shift = 0;
                int* tmp = (int*)&pBuff[ImpulsesAnswerHeader.Size];
                for (int i = 0; i < answer->TSCount; i++)
                {
                    //если заполнил 4 байта, перехожу на следующие 4
                    if (shift == 32)
                    {
                        shift = 0;
                        tmp++;
                    }
                    *tmp |= ((int)station.TS[i] << shift);
                    shift += 2;
                }
                // заполняю массив импульсов ТУ (по 3 бита на импульс), 
                // начинается со следующего байта после массива импульсов ТС
                // определяю смещение массива
                byte* impulses = &pBuff[ImpulsesAnswerHeader.Size + station.TS.Count / 4
                    + ((station.TS.Count % 4 == 0) ? (0) : (1))];
                tmp = (int*)impulses;
                shift = 0;
                for (int i = 0; i < answer->TUCount; i++)
                {
                    //если заполнил 3 байта, перехожу на следующие 3
                    if (shift == 24)
                    {
                        shift = 0;
                        impulses += 3;
                        tmp = (int*)impulses;
                    }
                    *tmp |= ((int)station.TU[i] << shift);
                    shift += 3;
                }
            }
            return FrameParser.MakeFrame(answerBuffer, FrameParser.FrameType.Regular);
        }

        ///// <summary>
        ///// Сгенерировать событие на отправление импульса
        ///// </summary>
        ///// <param name="request">Запрос на отправление импульса</param>
        ///// <param name="error">Ошибка отправления</param>
        ///// <param name="channelName">Название канала, в который записан импульс</param>
        //private unsafe void RaiseOnImpulseSentEvent(byte[] commandBuffer,
        //    string serverName, RequestError error, string channelName)
        //{
        //    if (OnImpulseSent == null)
        //        return;
        //    if (commandBuffer == null)
        //        return;

        //    ImpulseSendArgs args = new ImpulseSendArgs();
        //    args.Source = channelName;
        //    args.Server = serverName;
        //    args.Result = error;
        //    args.SendTime = Program.Settings.CurrentTime;

        //    fixed (byte* pCommand = commandBuffer)
        //    {
        //        CommandRequest* request = (CommandRequest*)pCommand;
        //        args.ImpulseName = PointerToString(request->CommandID, CommandRequest.CommandIDLength, Encoding.Unicode);
        //        args.Sender = PointerToString(request->SenderID, CommandRequest.SenderIDLength, Encoding.Unicode);
        //        args.StationCode = request->StationID;
        //    }

        //    OnImpulseSent(this, args);
        //}

        /// <summary>
        /// Скопировать массив байт
        /// </summary>
        /// <param name="pSrc">Исходный массив байт</param>
        /// <param name="pDst">Результирующий массив байт</param>
        /// <param name="count">Количество копируемых байт</param>
        private static unsafe void CopyBytes(byte* pSrc, byte* pDst, int count)
        {
            byte* ps = pSrc;
            byte* pd = pDst;
            // Loop over the count in blocks of 4 bytes, copying an integer (4 bytes) at a time:
            for (int i = 0; i < count / 4; i++)
            {
                *((int*)pd) = *((int*)ps);
                pd += 4;
                ps += 4;
            }

            // Complete the copy by moving any bytes that weren't moved in blocks of 4:
            for (int i = 0; i < count % 4; i++)
            {
                *pd = *ps;
                pd++;
                ps++;
            }
        }

        /// <summary>
        /// Преобразовать указатель в строку
        /// </summary>
        /// <param name="pointer">Указатель на массив байт</param>
        /// <param name="length">Длинна массива</param>
        /// <param name="encode">Кодировка символов</param>
        /// <returns>Преобразованная строка</returns>
        public static unsafe string PointerToString(byte* pointer, int length, Encoding encode)
        {
            byte[] tmp = new byte[length];
            fixed (byte* ptmp = tmp)
            {
                CopyBytes(pointer, ptmp, length);
            }
            return encode.GetString(tmp).Trim('\0');

        }
        #endregion
    }
}
