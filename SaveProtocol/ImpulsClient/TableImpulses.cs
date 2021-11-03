using System;
using System.Linq;
using System.Collections.Generic;
using SCADA.Common.Enums;

namespace SCADA.Common.ImpulsClient
{
	/// <summary>
	/// Таблица импульсов.
	/// </summary>
	public class TableImpulses 
    {
		/// <summary>
		/// Время последнего изменения.
		/// </summary>
		private DateTime _timeChanged;


        public ImpulseState this[int index]
        {
            get
            {
                return _impulses[index].State;
            }
            set
            {
                _impulses[index].State = value;
            }
        }



        private readonly List<Impulse> _impulses;

        public IList<Impulse> Impulses
        {
            get
            {
                return _impulses;
            }
        }

        /// <summary>
        /// номер станции
        /// </summary>
        private readonly int _stCode;
        /// <summary>
        /// Заполняет таблицу импульсов.
        /// </summary>
        /// <param name="impulses">
        /// Массив записей с конфигурационном файле об импульсах. <see cref="Impulse[]"/>
        /// </param>
        public TableImpulses(Impulse[] impulses, int stCode)
		{
			_timeChanged = DateTime.Now;
            _impulses = new List<Impulse>();
            _stCode = stCode;
			for(int i = 0; i < impulses.Length; i++)
			{
                if (_impulses.Where(x => x.Name == impulses[i].Name).FirstOrDefault() == null)
                    _impulses.Add(new Impulse(impulses[i].Name, impulses[i].Type, impulses[i].ToolTip));
                else
                {
                    System.Console.Error.WriteLine("Дублированное имя импульса '{0}'({1}[{2}])",
                                                                            impulses[i].Name, stCode, i);
                    _impulses.Add(new Impulse($"{impulses[i].Name}_{i}", impulses[i].Type, impulses[i].ToolTip));
                }
			}
		}

        public List<string> Names
		{
			get
			{
				return _impulses.Select(x=>x.Name).ToList();
			}
		}



        public int Count
        {
            get
            {
                return _impulses.Count;
            }
        }

        public  int LastCountReceivingImp { get;  set; }

        public TableImpulses Clone()
        {
            var cloneTable = new TableImpulses(_impulses.ToArray(), _stCode);
            //
            return cloneTable;
        }

        /// <summary>
        /// Задать состояния импульсов.
        /// </summary>
        /// <param name="states">
        /// A <see cref="ImpulseState[]"/>
        /// </param>
        public void SetStates(ImpulseState[] states, DateTime time_changed)
        {
            int min_index = 0;
            min_index = _impulses.Count > states.Length ? states.Length : _impulses.Count;

            // ERROR Тут есть проблема - объем импульсов не совпадает с приходящим.
            _timeChanged = time_changed;

            //if(m_impulseStates.Length != states.Length)
            //	m_logger.DebugFormat("{0}!={1}", m_impulseStates.Length, states.Length);

            for (int i = 0; i < min_index; i++)
            {
                _impulses[i].State = states[i];
            }
            //
            LastCountReceivingImp = states.Length;
        }


        public void SetAllStates(ImpulseState state, DateTime time_changed)
        {
            _impulses.ForEach(x => x.State = state);
            _timeChanged = time_changed;
        }

        public bool AddImpuls(string name)
        {
            if (!Contains(name))
            {
                _impulses.Add(new Impulse(name));
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Установить состояние импульса с определенным именем
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="state">
        /// A <see cref="ImpulseState"/>
        /// </param>
        /// <param name="timeChange">
        /// A <see cref="DateTime"/>
        /// </param>
        public void SetState(string name, ImpulseState state, DateTime timeChange)
        {
            //if (ImpulsesClient.Closed)
            //{
            var findImpulse = _impulses.Where(x => x.Name == name).FirstOrDefault();
            if (findImpulse != null)
            {
                Console.WriteLine("impulse {0} changed state to {1}", name, state.ToString());
                findImpulse.State = state;
                _timeChanged = timeChange;
            }
            else
            {
                Console.WriteLine("Can't find impulse {0} on the station", name);
            }
            //}
        }

        public void SetState(string name, ImpulseState state)
        {
            //if (ImpulsesClient.Closed)
            //{
            SetState(name, state, DateTime.Now);
            //}
        }
				
		/// <summary>
		/// Получить состояние импульса.
		/// </summary>
		/// <param name="index">
		/// Номер импульса <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// Текущее состояние импульса <see cref="ImpulseState"/>
		/// </returns>
		public ImpulseState GetState(int index)
		{
            if (index >= 0 && index < _impulses.Count)
                return _impulses[index].State;
            else
                return ImpulseState.UncontrolledState;
		}

        /// <summary>
        /// Получить состояние импульса.
        /// </summary>
        /// <param name="name">
        /// Имя импульса <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// Текущее состояние импульса <see cref="ImpulseState"/>
        /// </returns>
        public ImpulseState GetState(string name)
        {
            var findImpulse = _impulses.Where(x => x.Name == name).FirstOrDefault();
            if (findImpulse != null)
                return findImpulse.State;
            else return ImpulseState.UncontrolledState;
        }

        public StatesControl GetStatesControl(int index)
        {
            if (index >= 0 && index < _impulses.Count)
                return Impulse.GetStateShort(_impulses[index].State);
            else
                return StatesControl.nocontrol;
        }

        public int GetCountStateImpuls(ImpulseState state)
        {
            return _impulses.Where(x => x.State == state).Count();
        }

        /// <summary>
        /// Содержит ли станция импульс
        /// </summary>
        /// <param name="name">название импульса</param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            if (_impulses.Where(x => x.Name == name).FirstOrDefault() != null)
                return true;
            else return false;
        }
		
		/// <summary>
		/// Получить номер импульса.
		/// </summary>
		/// <param name="name">
		/// Имя импульса <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Номер импульса <see cref="System.Int32"/>
		/// </returns>
		public int GetImpulseIndex(string name)
		{
            var findImpulse = _impulses.Where(x => x.Name == name).FirstOrDefault();
            if (findImpulse != null)
                return _impulses.IndexOf(findImpulse);
            else
                return -1;
		}
		
		/// <summary>
		/// Гет время получения последних данных
		/// </summary>
		/// <returns>
		/// Время получения последних данных <see cref="DateTime"/>
		/// </returns>
		public DateTime get_time()
		{
			return _timeChanged;
		}


	}
}
