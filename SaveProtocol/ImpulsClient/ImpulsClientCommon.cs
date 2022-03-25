using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SCADA.Common.Enums;
using SCADA.Common.SaveElement;
using SCADA.Common.ImpulsClient;
using SCADA.Common.ImpulsClient.requests;

namespace SCADA.Common.ImpulsClient
{
    public abstract class ImpulsClientCommon
    {
        public delegate void GetNewDataEventHandler();

        public static event GetNewDataEventHandler NewData;

        public delegate void GetNewStateEventHandler();

        public event GetNewStateEventHandler NewState;
        /// <summary>
        /// событие на подключение к серверу или при потери связи
        /// </summary>
        public static event GetNewStateEventHandler ConnectDisconnectionServer;
        /// <summary>
        /// подключен ли сервер
        /// </summary>
        public static bool Connect
        {
            get
            {
                return _connect;
            }
            internal set
            {
                _connect = value;
            }
        }

        protected static DateTime _TimeImpuls = DateTime.MinValue;

        public static DateTime TimeImpuls
        {
            get
            {
                return _TimeImpuls;
            }
        }

        protected static DateTime _last_disconnect_server = DateTime.MaxValue;
        /// <summary>
        /// время последней потери связи с сервером импульсов
        /// </summary>
        public static DateTime LastDisconnectServer
        {
            get { return _last_disconnect_server; }
        }
        /// <summary>
        /// является ли подключение первым
        /// </summary>
        protected bool _firstconnect = true;

        protected static bool _connect = false;

        protected static string _stateDescription = null;

        protected DateTime _date;

        protected int _interval = 1000;

        /// <summary>
        /// Хранилище текущих данных !!!
        /// </summary>
        protected DataContainer _data;
        public DataContainer Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
		/// обработчик запросов
		/// </summary>
		private RequestHandler _requestHandler;

        /// <summary>
        /// Таймер вызова функции получения данных.
        /// </summary>
        protected Timer _workTimer;

        /// <summary>
        /// Признак работы таймера.
        /// </summary>
        protected bool _isTimerInWork;

        /// <summary>
        /// Признак закрытия программы.
        /// </summary>
        protected static bool _closed;

        /// <summary>
        /// адрес сеервера
        /// </summary>
        protected string _connectionString;


        protected Dictionary<int, StationTableServiceTs> _serviceImpulses = new Dictionary<int, StationTableServiceTs>();

        /// <summary>
        /// изменение строки подключения
        /// </summary>
        protected bool _reConnect;

        /// <summary>
        /// Обрабатывает файл конфигурации.
        /// Создает список станций. Читает таблицы импульсов.
        /// </summary>
        /// <param name="configFileName">
        /// A <see cref="System.String"/>
        /// </param>
        public ImpulsClientCommon(StationRecord[] inp_station_records, string tables_path, Dictionary<int, StationTableServiceTs> serviceImpulses, int intervalUpdate = 1000)
        {
            _data = new DataContainer();
            _data.LoadStationsData(inp_station_records, tables_path);
            _serviceImpulses = serviceImpulses;
            _workTimer = new Timer(GetImpulsesTimerFunc);
            _interval = intervalUpdate;
        }

        public void Start()
        {
            _closed = false;
            go();
        }

        protected void OnGetNewData()
        {
            SetNewData();
            _stateDescription = "Обновлены таблицы";
            Set_changing_state();
        }

        protected  void SetNewData()
        {
            if (NewData != null)
                NewData();
        }

        protected void Set_changing_state()
        {
            if (NewState != null)
                NewState();
        }

        protected void Wait()
        {
            while (_workTimer != null)
            {
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Кнопка стоп ! Почему-то тоже красненькая !
        /// </summary>
        public void Stop()
        {
            _closed = true;
            Connect = false;
            EventConnectDisconnect();
        }

        /// <summary>
        /// Кнопка стоп ! Почему-то тоже красненькая !
        /// </summary>
        public void ReConnect(string connectionString)
        {
            _connectionString = connectionString;
           _reConnect = true;
        }

        /// <summary>
        /// Запуск работы.
        /// </summary>
        /// <param name="obj">некий объект</param>
        protected abstract void go();

        protected abstract void GetImpulsesTimerFunc(object obj);

        /// <summary>
        /// обновляем значения служебных импульсов
        /// </summary>
        protected void UpdateValueServiceImpulses()
        {
            try
            {
                if (_serviceImpulses == null)
                    return;
                foreach (var station in _serviceImpulses)
                {
                    foreach (KeyValuePair<string, Dictionary<Viewmode, string>> impulses in station.Value.NamesValue)
                    {
                        StatesControl state_on = Data.GetStateControl(station.Key, impulses.Value[Viewmode.impuls_activ]);
                        StatesControl state_off = Data.GetStateControl(station.Key, impulses.Value[Viewmode.impuls_pasiv]);
                        //
                        if ((state_on == StatesControl.activ && state_off != StatesControl.activ) || (state_off == StatesControl.activ && state_on != StatesControl.activ))
                        {
                            if (state_on == StatesControl.activ && state_off != StatesControl.activ)
                                _data.Stations[station.Key].TS.SetState(impulses.Key, ImpulseState.ActiveState);
                            else
                                _data.Stations[station.Key].TS.SetState(impulses.Key, ImpulseState.PassiveState);
                        } 
                        else if(state_on == StatesControl.pasiv && state_off == StatesControl.pasiv)
                            _data.Stations[station.Key].TS.SetState(impulses.Key, ImpulseState.PassiveState);
                        //else
                        //    Connections.ClientImpulses.data.Stations[station.Key].TS.set_state(impulses.Key, ImpulseState.UncontrolledState);
                    }
                }
            }
            catch { }
        }

        protected void EventConnectDisconnect()
        {
            if (ConnectDisconnectionServer != null)
                ConnectDisconnectionServer();
        }

    }
}
