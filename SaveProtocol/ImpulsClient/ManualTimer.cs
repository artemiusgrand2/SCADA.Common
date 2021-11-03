namespace SCADA.Common.ImpulsClient
{
	/// <summary>
	/// Класс для определения таймаутов
	/// </summary>
	/// <remarks>Разрешение таймаута не менее 500 мс.</remarks>
	public class ManualTimer
	{
		/// <summary>
		/// Количество тиков после которого срабатывает таймаут
		/// </summary>
		private uint m_start;

		/// <summary>
		/// Интервал таймаута в мс
		/// </summary>
		private uint m_interval;

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
		/// <remarks>Таймаут равен 0 мс.</remarks>
		public ManualTimer() : this(0)
		{
		
		}
		
		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="milliseconds">Интервал таймаута в мс.</param>
		/// <remarks>Разрешение таймаута не менее 500 мс.</remarks>
		public ManualTimer(int milliseconds) 
		{ 
			Set(milliseconds); 
		}

		/// <summary>
		/// Таймаут
		/// </summary>
		public bool Timeout
		{
			get
			{
				uint ticks = (uint)(System.Environment.TickCount);
				bool result = false;
				//если ticks пересекли 0 (счёт пошёл о кругу)
				if(ticks < m_start)
				{
					//если прибавлением интервала 0 не пересекается, то таймаут
					if(m_start + m_interval > m_start)
						result = true;
					else
						//если 0 пересекается, то надо проверить на таймаут
						if(m_start + m_interval > ticks)
							result = false;
						else
							result = true;
					//восстанавливаю очерёдность m_start и ticks 
					//(m_start должен быть меньше ticks)
					m_start = ticks - m_interval;
				}
				else
				{
					if(m_start + m_interval <= ticks)
						result = true;
					else
						result = false;
				}
				return result;
			}
		}

		/// <summary>
		/// Установить таймаут.
		/// </summary>
		/// <param name="milliseconds">Интервал таймаута в мс.</param>
		/// <remarks>Разрешение таймаута не менее 500 мс.</remarks>
		public void Set(int milliseconds) 
		{ 
			m_start = (uint)(System.Environment.TickCount);
			m_interval = (uint)milliseconds;
		}
		
		/// <summary>
		/// Сбросить таймаут
		/// </summary>
		public void Reset() 
		{ 
			Set((int)m_interval); 
		}
	}
}
