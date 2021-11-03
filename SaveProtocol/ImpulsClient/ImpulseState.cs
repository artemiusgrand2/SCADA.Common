namespace SCADA.Common.ImpulsClient
{
	/// <summary>
	/// возможные состояния импульса и поля контроля
	/// </summary>
	public enum ImpulseState
	{
		/// <summary>
		/// Импульс не приходит, поле контроля не определено
		/// </summary>
		UncontrolledState = 1,
		/// <summary>
		/// Импульс и поле контроля имеют пассивное состояние
		/// </summary>
		PassiveState = 2,
		/// <summary>
		/// Импульс и поле контроля имеют активное состояние
		/// </summary>
		ActiveState = 3,
		/// <summary>
		/// Команда забрана (неопределённое состояние импульса ТУ)
		/// </summary>
		Taken = 0,
		/// <summary>
		/// Выполнить команду
		/// </summary>
		Execute = 1,//выполнить команду или выполнено
		/// <summary>
		/// Прервать команду
		/// </summary>
		Break = 7,//прервать выполнение или прервано
		/// <summary>
		/// Команда выполняется (приходит со станции)
		/// </summary>
		Executing = 6,
		/// <summary>
		/// Подготовиться или открыто
		/// </summary>
		Ready = 2,
		/// <summary>
		/// Блокировать ключ или заблокировано
		/// </summary>
		Lock = 3,
		/// <summary>
		/// Снять блокировку или готов
		/// </summary>
		Work = 4,
		/// <summary>
		/// Неисправность
		/// </summary>
		Error = 5
	}
}