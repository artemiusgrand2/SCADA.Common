using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SCADA.Common.Enums;
using SCADA.Common.SaveElement;
using SCADA.Common.ImpulsClient;
using SCADA.Common.ImpulsClient.requests;

using SCADA.Common.Log;

namespace SCADA.Common.ImpulsClient
{
    /// <summary>
    /// Служит для получения значения импульсов с сервера.
    /// </summary>
    public class ImpulsesClientTCP : ImpulsClientCommon
    {

        /// <summary>
        /// Буфер запроса на получение таблиц импульсов
        /// </summary>
        private byte[] m_impulsesRequest = null;
        /// <summary>
        /// Буфер запроса на получение времени сервера
        /// </summary>
        private byte[] m_timeRequest = null;

        /// <summary>
        /// Соединение через порт или сокет
        /// </summary>
        private UniConnection m_client;

        /// <summary>
        /// спрашивать ли текущее время сервера
        /// </summary>
        private bool m_isrequestTime = false;


        public ImpulsesClientTCP(StationRecord[] inp_station_records, string server_address, string tables_path,   bool isrequestTime = false, Dictionary<int, StationTableServiceTs> serviceImpulses = null)
            : base(inp_station_records, tables_path, serviceImpulses)
        {
            m_client = new UniConnection();
            _connectionString = server_address;
            m_isrequestTime = isrequestTime;
        }

        public ImpulsesClientTCP(StationRecord[] inp_station_records, string server_address, string tables_path, int intervalUpdate)
           : base(inp_station_records, tables_path, null, intervalUpdate)
        {
            m_client = new UniConnection();
            _connectionString = server_address;
        }



        /// <summary>
        /// Запуск работы.
        /// </summary>
        /// <param name="obj">некий объект</param>
        protected override void go()
        {
            System.Console.WriteLine("Старт получения данных с сервера.");

            _stateDescription = "Старт получения данных с сервера.";
            Set_changing_state();
            //
            if (_data.Stations.Count < 1)
            {
                Console.WriteLine("Найдено 0 станций. Выход из потока получения данных с сервера.");
                return;
            }
            //Формирую запрос на таблицы
            m_impulsesRequest = new byte[TablesListRequestHeader.Size + _data.Stations.Count * 4];
            int count_st = 0;
            unsafe
            {
                TablesListRequestHeader* impulsesRequestHeader;
                fixed (byte* pRequest = m_impulsesRequest)
                {
                    impulsesRequestHeader = (TablesListRequestHeader*)pRequest;
                }
                impulsesRequestHeader->Header.PacketSize = (short)m_impulsesRequest.Length;
                impulsesRequestHeader->Header.PacketType = (int)RequestType.ListOfTables;
                impulsesRequestHeader->StationsCount = (short)_data.Stations.Count;
                int* station = (int*)&impulsesRequestHeader[1];
                foreach (KeyValuePair<int, Station> st in _data.Stations)
                {
                    *station = st.Key;
                    station++;
                    count_st++;
                }
            }

            Console.WriteLine("Request for {0} stations size={1}", count_st, m_impulsesRequest.Length);

            m_impulsesRequest = FrameParser.MakeFrame(m_impulsesRequest, FrameParser.FrameType.Regular);
            //Формирую запрос времени
            m_timeRequest = new byte[RequestHeader.Size];
            unsafe
            {
                RequestHeader* timeRequestHeader;
                fixed (byte* pRequest = m_timeRequest)
                {
                    timeRequestHeader = (RequestHeader*)pRequest;
                }
                //
                timeRequestHeader->PacketSize = (short)m_timeRequest.Length;
                timeRequestHeader->PacketType = (int)RequestType.Time;
            }
            m_timeRequest = FrameParser.MakeFrame(m_timeRequest, FrameParser.FrameType.Regular);
            //
            _workTimer.Change(0, _interval);
        }

        protected override void GetImpulsesTimerFunc(object obj)
        {
            try
            {
                if (_isTimerInWork)
                {
                    System.Console.Error.WriteLine("Попытка запуска второго экземпляра функции таймера." +
                                                    " Предыдущий запуск неуспел отработать.");
                    _stateDescription = "Переподключение клиента ";
                    Set_changing_state();
                    return;
                }

                _isTimerInWork = true;

                if (_closed)
                {
                    _workTimer.Change(Timeout.Infinite, _interval);
                    m_client.Close();
                    //m_workTimer = null;
                    System.Console.WriteLine("Конец получения данных с сервера.");
                    _stateDescription = "Клиент завершил работу";
                    Set_changing_state();
                    _isTimerInWork = false;
                    return;
                }

                if (_reConnect)
                {

                    m_client.Close();
                    System.Threading.Thread.Sleep(100);
                    _reConnect = false;
                }

                //если потерял связь с сервером, то надо заново соединятся
                if (!m_client.IsOpen)
                {
                    try
                    {
                        m_client.Open(_connectionString);
                        //
                        if (!Connect)
                        {
                            Connect = true;
                            EventConnectDisconnect();
                        }
                        //
                        System.Console.WriteLine("Соединение с сервером импульсов {0} установлено", _connectionString);
                        _stateDescription = string.Format("Соединение с {0} установлено", _connectionString);
                        Set_changing_state();
                    }
                    catch
                    {
                        if (_firstconnect)
                        {
                            Connect = false;
                            _last_disconnect_server = DateTime.Now;
                            _firstconnect = false;
                            EventConnectDisconnect();
                        }
                        else
                        {
                            if (Connect)
                            {
                                Connect = false;
                                _last_disconnect_server = DateTime.Now;
                                EventConnectDisconnect();
                            }
                        }
                    }
                }

                //читаю данные
                if (m_client.IsOpen)
                {
                    GetImpulses();
                    if (m_isrequestTime)
                        GetTime();
                }

                _isTimerInWork = false;

                //m_logger.DebugFormat("Время работы цикла: {0}", DateTime.Now - start_time);
            }
            catch (Exception error)
            {
                Logger.LogCommon.Error(error);
            }
            finally
            {
                _isTimerInWork = false;
            }
        }

        private void GetTime()
        {
            if (m_timeRequest == null)
                return;

            //отправляю запрос
            try
            {
                if (m_client.Write(m_timeRequest, 0, m_timeRequest.Length) == 0)
                {
                    // закрывать соединение не надо, т.к. 0 это не ошибка,
                    // ошибки генерируют исключение
                    return;
                }
            }
            catch (UniConnectionException e)
            {
                if (e.Error != UniConnectionError.TimedOut)
                {
                    m_client.Close();
                }
                return;
            }
            //читаю ответ
            byte[] buffer = new byte[32 * 1024];
            FrameParser parser = new FrameParser();
            int readed = 0;
            try
            {
                readed = m_client.Read(buffer, 0, buffer.Length);
                _date = DateTime.Now;
            }
            catch (UniConnectionException e)
            {
                if (e.Error != UniConnectionError.TimedOut)
                    m_client.Close();
                return;
            }
            // обрабатываю полученные данные
            for (int i = 0; i < readed; i++)
            {
                parser.Parse(buffer[i]);
                if (parser.IsFrameReady)
                {
                    _TimeImpuls = TimeParser.ParseTablesAnswer(parser.GetFrame());
                }
            }
        }

        /// <summary>
        /// 
        /// Прочитать таблицы импульсов
        /// </summary>
        private void GetImpulses()
        {
            if (m_impulsesRequest == null)
                return;

            //отправляю запрос
            try
            {
                if (m_client.Write(m_impulsesRequest, 0, m_impulsesRequest.Length) == 0)
                {
                    // закрывать соединение не надо, т.к. 0 это не ошибка,
                    // ошибки генерируют исключение
                    return;
                }
            }
            catch (UniConnectionException e)
            {
                //				DiagnosticManager.Instance.new_message_impulses_server(string.Format("Ошибка отправки запроса {0}", e.ToString()));
                if (e.Error != UniConnectionError.TimedOut)
                {
                    m_client.Close();
                }
                return;
            }
            //читаю ответ
            byte[] buffer = new byte[32 * 1024];
            FrameParser parser = new FrameParser();
            int receivedStations = 0;
            ManualTimer readTimeout = new ManualTimer(5000);
            do
            {
                if (!m_client.IsOpen)
                    break;

                int readed = 0;
                try
                {
                    readed = m_client.Read(buffer, 0, buffer.Length);
                    _date = DateTime.Now;
                }
                catch (UniConnectionException e)
                {
                    //					DiagnosticManager.Instance.new_message_impulses_server(string.Format("Ошибка чтения импульсов {0}", e.ToString()));
                    if (e.Error != UniConnectionError.TimedOut)
                        m_client.Close();
                    return;
                }

                // если долго не было данных, то прервать ожидание
                // это нужно, т.к. Read может вернуть 0 (для посл. порта)
                //if(readTimeout.Timeout)
                //    break;

                // обрабатываю полученные данные
                for (int i = 0; i < readed; i++)
                {
                    parser.Parse(buffer[i]);
                    if (parser.IsFrameReady)
                    {
                        ParseTablesAnswer(parser.GetFrame());
                        receivedStations++;
                        // сбрасываю только при получении станции
                        readTimeout.Reset();
                    }
                }
                //m_logger.DebugFormat("Кол-во полученных станций при запросе: {0}", receivedStations);
            }
            while (receivedStations < _data.Stations.Count);
            //обновляем значения служебных импульсов
            UpdateValueServiceImpulses();
            OnGetNewData();
            //сбросить таймаут
            readTimeout.Reset();
        }

        /// <summary>
        /// Преобразовать указатель в строку
        /// </summary>
        /// <param name="pointer">Указатель на массив байт</param>
        /// <param name="length">Длинна массива</param>
        /// <param name="encode">Кодировка символов</param>
        /// <returns>Преобразованная строка</returns>
        private unsafe string PointerToString(byte* pointer, int length, Encoding encode)
        {
            byte[] tmp = new byte[length];
            fixed (byte* ptmp = tmp)
            {
                CopyBytes(pointer, ptmp, length);
            }
            return encode.GetString(tmp).Trim('\0');

        }

        /// <summary>
        /// Скопировать массив байт
        /// </summary>
        /// <param name="pSrc">Исходный массив байт</param>
        /// <param name="pDst">Результирующий массив байт</param>
        /// <param name="count">Количество копируемых байт</param>
        private unsafe void CopyBytes(byte* pSrc, byte* pDst, int count)
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
        /// Заполнить состояния импульсов принятыми
        /// значениями из пакета.
        /// </summary>
        /// <param name="answerTable">Пакет байт с значениями импульсов.</param>
        private void ParseTablesAnswer(byte[] answerTable)
        {
            ImpulsesAnswer answer = TableParser.ParseTablesAnswer(answerTable);

            if (answer == null)
                return;

            ImpulseState[] states_ts = new ImpulseState[answer.Header.TSCount];
            ImpulseState[] states_tu = new ImpulseState[answer.Header.TUCount];

            if (_data.Stations.ContainsKey(answer.Header.StationID))
            {
                for (int k = 0; k < answer.Header.TSCount; k++)
                {
                    states_ts[k] = (ImpulseState)answer.TsImpulses[k];
                }
                for (int k = 0; k < answer.Header.TUCount; k++)
                {
                    states_tu[k] = (ImpulseState)answer.TuImpulses[k];
                }

                _data.Stations[answer.Header.StationID].SetImpulsesStates(states_ts, _date, ImpulsesTableType.TS);
                _data.Stations[answer.Header.StationID].SetImpulsesStates(states_tu, _date, ImpulsesTableType.TU);
            }
        }

        /// <summary>
        /// Отправить импульс ТУ.
        /// </summary>
        /// <param name="request">Буфер с запросом на отправку импульса</param>
        /// <returns>Буфер с результатом отправки импульса</returns>
        public unsafe RequestError SendImpulse(string impulse_name, int st_code, ImpulseState state)
        {
            if (!m_client.IsOpen)
            {
                return RequestError.IOError;// MakeCommandAnswer(impulse_name, st_code, RequestError.IOError);
            }

            //			//получаю название импульса
            //			string impulse = impulse_name;

            //проверяю есть ли такая станция
            TableImpulses table = null;
            foreach (Station st in _data.Stations.Values)
            {
                if (st.Code == st_code)
                {
                    table = st.TU;
                    break;
                }
            }

            if (table == null)
            {
                return RequestError.UnknownStation;// MakeCommandAnswer(impulse_name, st_code, RequestError.UnknownStation);
            }


            bool is_finded_imp = false;
            //ищу нужный импульс
            foreach (string name in table.Names)
            {
                if (name == impulse_name)
                {
                    is_finded_imp = true;
                    break;
                }
            }

            if (!is_finded_imp)
                return RequestError.UnknownCommand;// MakeCommandAnswer(impulse_name, st_code, RequestError.UnknownCommand);


            byte[] requestBuffer = new byte[CommandRequest.Size];
            CommandRequest* pCommandRequest;
            fixed (byte* pRequest = requestBuffer)
            {
                pCommandRequest = (CommandRequest*)pRequest;
            }
            pCommandRequest->Header.PacketSize = CommandRequest.Size;
            pCommandRequest->Header.PacketType = (int)RequestType.Command; // Command type
            pCommandRequest->StationID = st_code;
            pCommandRequest->CommandValue = (short)state;

            byte[] comand_id = Encoding.Unicode.GetBytes(impulse_name);
            int count = (comand_id.Length < CommandRequest.CommandIDLength) ? (comand_id.Length) : (CommandRequest.CommandIDLength);
            for (int i = 0; i < count; i++)
                pCommandRequest->CommandID[i] = comand_id[i];

            // в качестве идентификатора использую имя компьютера
            byte[] comp_id = Encoding.Unicode.GetBytes(System.Net.Dns.GetHostName());
            count = (comp_id.Length < CommandRequest.SenderIDLength) ? (comp_id.Length) : (CommandRequest.SenderIDLength);
            for (int i = 0; i < count; i++)
                pCommandRequest->SenderID[i] = comp_id[i];


            // отправляю запрос
            byte[] answer = null;
            answer = SendRequest(requestBuffer, true, st_code);

            if (answer == null)
            {
                return RequestError.IOError;// MakeCommandAnswer(impulse_name, st_code, RequestError.IOError);
            }
            else
            {
                fixed (byte* pBuff = answer)
                {
                    CommandRequest* pAnswer = (CommandRequest*)pBuff;
                    return (RequestError)pAnswer->CommandValue;// MakeCommandAnswer(impulse_name, st_code, (RequestError)pAnswer->CommandValue);
                }
            }
        }

        /// <summary>
        /// Послать запрос
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="waitAnswer">Ждать ли ответа</param>
        /// <returns>Ответ на запрос, либо null если ответ не получен или его не надо ждать</returns>
        public byte[] SendRequest(byte[] request, bool waitAnswer, int code)
        {
            if (!m_client.IsOpen)
                return null;
            //
            byte[] frame = FrameParser.MakeFrame(request, FrameParser.FrameType.Regular);
            //очищаю входной буфер
            //отправляю запрос
            int written = 0;
            try
            {
                written = m_client.Write(frame, 0, frame.Length);
            }
            catch
            {
                return null;
            }

            if (written < frame.Length)
            {
                return null;
            }

            if (!waitAnswer)
            {
                return null;
            }

            //читаю только один пакет
            byte[] inBuffer = new byte[4 * 1024];
            ManualTimer readTimeout = new ManualTimer(5000);

            int readed = 0;
            FrameParser parser = new FrameParser();
            while (!readTimeout.Timeout)
            {
                try
                {
                    readed = m_client.Read(inBuffer, 0, inBuffer.Length);
                }
                catch
                {
                    return null;
                }
                if (readed != 0)
                    //сбросить таймаут
                    readTimeout.Reset();

                //обрабатываю полученные данные
                for (int i = 0; i < readed; i++)
                {
                    parser.Parse(inBuffer[i]);
                    if (parser.IsFrameReady)
                        return parser.GetFrame();
                }
            }
            return null;
        }

        private unsafe string MakeCommandAnswer(string command_id, int station_id, RequestError result)
        {
            return String.Format("{0} - {1}: {2}", station_id, command_id, result);

        }
    }
}
