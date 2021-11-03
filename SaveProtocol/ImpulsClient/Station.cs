using System;
using System.Collections.Generic;
using System.Text;

namespace SCADA.Common.ImpulsClient
{
	/// <summary>
	/// Станция
	/// </summary>
	public class Station
	{
		#region Данные
		/// <summary>
		/// Код станции
		/// </summary>
		private int _code;

		/// <summary>
		/// Название станции
		/// </summary>
		private string _name;

        /// <summary>
        /// Список импульсов ТС станции.
        /// </summary>
        private TableImpulses _ts;
		
		/// <summary>
        /// Список импульсов ТУ станции.
        /// </summary>
        private TableImpulses _tu;
		#endregion

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="name">Название станции</param>
		/// <param name="code">Код станции</param>
		public Station(string name, int code)
		{
			_name = name;
			_code = code;
            _ts = null;
		}

		#region Свойства
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public int Code
		{
			get
			{
				return _code;
			}
		}

        public TableImpulses TS
        {
            get
            {
                return _ts;
            }
        }

		public TableImpulses TU
        {
            get
            {
                return _tu;
            }
        }


        public IList<Impulse> Impulses
        {
            get
            {
                var result = new List<Impulse>(_ts.Impulses);
                result.AddRange(_tu.Impulses);
                return result;
            }
        }

        #endregion

		#region Методы

		public override string ToString()
		{
			return string.Format("{0} ({1})", _name, _code);
		}

		/// <summary>
		/// Загрузить объекты станции.
		/// </summary>
		public void LoadData(Impulse[] ts_impulses, Impulse[] tu_impulses)
		{
            #region загружаю таблицу импульсов для станции
            if (ts_impulses != null)
                _ts = new TableImpulses(ts_impulses, _code);
            else
                _ts = new TableImpulses(new Impulse[0], _code);
            //
            if (tu_impulses != null)
                _tu = new TableImpulses(tu_impulses, _code);
            else
                _tu = new TableImpulses(new Impulse[0], _code);
            #endregion
        }

        public Station Clone()
        {
            var clonestation = new Station(_name, _code);
            clonestation._ts = _ts.Clone();
            clonestation._tu = _tu.Clone();
            //
            return clonestation;
        }

		/// <summary>
		/// Установить значение импульса.
		/// </summary>
		/// <param name="impulse">Название импульса.</param>
		/// <param name="state">Состояние импульса.</param>
		/// <returns>true - если импульс найден и состояние установлено, иначе false.</returns>
		public void SetImpulsesStates(ImpulseState[] states, DateTime time_changed, ImpulsesTableType type)
		{
			if (type == ImpulsesTableType.TS && _ts != null)
				_ts.SetStates(states, time_changed);
			else if (type == ImpulsesTableType.TU && _tu != null)
				_tu.SetStates(states, time_changed);
		}

		/// <summary>
		/// Установить одинаковые значение импульсов.
		/// </summary>
		/// <param name="states">Состояния импульса.</param>
		/*public void SetImpulsesStates(ImpulseState state, DateTime time_changed)
		{
			m_ti.SetStates(state, time_changed);
		}*/
		
		/// <summary>
		/// Гет время получения последних данных
		/// </summary>
		/// <returns>
		/// Время получения последних данных <see cref="DateTime"/>
		/// </returns>
		public DateTime get_time_changed()
		{
			return TS.get_time();
		}


		
		#endregion
	}
}
