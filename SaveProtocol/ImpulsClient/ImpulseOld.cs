//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace SCADA.Common.ImpulsClient
//{
//	/// <summary>
//	/// Класс импульса
//	/// </summary>
//	public class Impulse
//	{
//		#region Данные
		
//		/// <summary>
//		/// Название импульса
//		/// </summary>
//		private string m_Name;

//		/// <summary>
//		/// Порядковый номер в таблице
//		/// </summary>
//		private int m_Number;
		
//		/// <summary>
//		/// Признак. Рассмотреть на надобность.
//		/// </summary>
//		private bool m_changed;
		
//		/// <summary>
//		/// Признак того что состояние импульса изменилось
//		/// </summary>
//		/// private bool m_changed;

//		/// <summary>
//		/// Состояние импульса
//		/// </summary>
//		private ImpulseState m_State;
		
//		/// <summary>
//		/// Состояние импульса
//		/// </summary>
//		// private ImpulseState m_prevState;
				
//		/// <summary>
//		/// Признак импульса ТУ
//		/// </summary>
//		private bool m_isTuImpulse;
//		#endregion

//		/// <summary>
//		/// Конструктор
//		/// </summary>
//		/// <param name="name">Название импульса.</param>
//		/// <param name="number">Номер импульса в таблице.</param>
//		/// <param name="matrix">Номер матрицы.</param>
//		/// <param name="box">Номер коробки.</param>
//		/// <param name="contact">Номер контакта.</param>
//		/// <param name="isTUImpulse">Признак того, что создаётся импульс ТУ</param>
//		public Impulse(string name, int number, bool isTUImpulse)
//		{
//			m_Number = number;
//			m_Name = name;
//			if(isTUImpulse)
//			{
//				m_State = ImpulseState.Taken;
//			}
//			else
//			{
//				m_State = ImpulseState.UncontrolledState;
//			}
//			m_changed = true;
//			m_isTuImpulse = isTUImpulse;
//		}

//		/// <summary>
//		/// Копирующий конструктор.
//		/// </summary>
//		/// <param name="imp"></param>
//		public Impulse(Impulse imp)
//		{
//			m_Number = imp.m_Number;
//			m_Name = imp.m_Name;
//			m_State = ImpulseState.UncontrolledState;
//			m_changed = true;
//			m_isTuImpulse = imp.m_isTuImpulse;
//		}

//		#region Свойства
//		/// <summary>
//		/// Название импульса.
//		/// </summary>
//		public string Name
//		{
//			get
//			{
//				return m_Name;
//			}
//		}
		
//		/// <summary>
//		/// Состояние импульса
//		/// </summary>
//		public ImpulseState State
//		{
//			get
//			{
//				return m_State;
//			}
//		}
		
//		/// <summary>
//		/// Предыдущее состояние импульса.
//		/// </summary>
//		public ImpulseState PreviousState
//		{
//			get
//			{
//				return m_State;
//			}
//		}
		
//		/// <summary>
//		/// Порядковый номер импульса в таблице
//		/// </summary>
//		public int Number
//		{
//			get
//			{
//				return m_Number;
//			}
//		}
		
//		/// <summary>
//		/// Признак того что состояние импульса изменилось
//		/// </summary>
//		public bool Changed
//		{
//			get
//			{
//				return m_changed;
//			}
//			set
//			{
//				m_changed = value;
//			}
//		}

//		/// <summary>
//		/// Признак импульса ТУ
//		/// </summary>
//		public bool IsTuImpulse
//		{
//			get 
//			{ 
//				return m_isTuImpulse; 
//			}
//			set 
//			{ 
//				m_isTuImpulse = value; 
//			}
//		}
//		#endregion

//		#region Методы
//		/// <summary>
//		/// Устанавливает состояние импульса.
//		/// </summary>
//		/// <param name="newState">Новое состояние.</param>
//		/// <returns>true если состояние изменилось, иначе - false.</returns>
//		/// <remarks>
//		/// <para>Если состояние импульса определяется состоянием другого импульса, 
//		/// то переданное состояние игнорируется и берётся состояние другого импульса</para>
//		/// </remarks>
//		public bool SetState(ImpulseState newState)
//		{
//			if(m_State == newState)
//				return false;

//			m_State = newState;
//			//сигнализирую об изменении состояния только в импульсах ТС
//			if(!m_isTuImpulse)
//				m_changed = true;
//			return true;
//		}

//		/// <summary>
//		/// Преобразовать к строке
//		/// </summary>
//		/// <returns>Название импульса</returns>
//		public override string ToString()
//		{
//			return m_Name;
//		}
//		#endregion
//	}
//}
