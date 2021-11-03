using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using SCADA.Common.ImpulsClient;

namespace SCADA.Common.ImpulsClient.PlayerTS
{
    public class ImpulsPipe : ImpulsClientCommon
    {
        NamedPipeClientStream m_pipeClient = null;

        IList<byte[]> requests = null;

        public ImpulsPipe(StationRecord[] inp_station_records, string tables_path, Dictionary<int, StationTableServiceTs> serviceImpulses): base(inp_station_records, tables_path, serviceImpulses)
        {
            m_pipeClient = new NamedPipeClientStream(".", "PlayerTS", PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation);
            requests = new List<byte[]>();
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
            //
            requests.Add(BitConverter.GetBytes(0x10000));
            foreach (var station in _data.Stations)
                requests.Add(BitConverter.GetBytes(station.Key));
            //
            _workTimer.Change(0, _interval);
        }

        protected override void GetImpulsesTimerFunc(object obj)
        {
            try
            {
                if (_isTimerInWork)
                {
                    _stateDescription = "Переподключение клиента ";
                    Set_changing_state();
                    return;
                }

                _isTimerInWork = true;

                if (_closed)
                {
                    _workTimer.Change(Timeout.Infinite, _interval);
                    m_pipeClient.Close();
                    System.Console.WriteLine("Конец получения данных с сервера.");
                    _stateDescription = "Клиент завершил работу";
                    Set_changing_state();
                    _isTimerInWork = false;
                    return;
                }

                //если потерял связь с сервером, то надо заново соединятся
                if (!m_pipeClient.IsConnected)
                {
                    try
                    {
                        m_pipeClient.Connect();
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
                if (m_pipeClient.IsConnected)
                    GetData();

                _isTimerInWork = false;

                //m_logger.DebugFormat("Время работы цикла: {0}", DateTime.Now - start_time);
            }
            catch (Exception error) { }
            finally
            {
                _isTimerInWork = false;
            }
        }

        private void GetData()
        {
            var index = 0;
            foreach (var request in requests)
            {
                byte[] buffer = new byte[1024];
                m_pipeClient.Write(request, 0, request.Length);
          //      var countRead = m_pipeClient.Read(
                if (index == 0)
                {

                }
                //
                index++;
            }
        }
    }
}
