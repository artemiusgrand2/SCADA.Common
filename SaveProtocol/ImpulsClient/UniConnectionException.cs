using System;
using System.Collections.Generic;
using System.Text;

namespace SCADA.Common.ImpulsClient
{
	enum UniConnectionError
	{
		ErrorSuccess = 0,
		IOError = 2,
		ConnectionClosed = 3,
		PortBusy = 4,
		AccessDenied = 5,
		UnexpectedError = 6,
		PortNotFound = 7,
		TimedOut = 8
	}

	sealed class UniConnectionException: SystemException
	{
		private UniConnectionError m_clientError;

		public UniConnectionException(UniConnectionError error)
		{
			m_clientError = error;
		}

		public UniConnectionException(UniConnectionError error, SystemException innerException):
			base(string.Empty, innerException)
		{
			m_clientError = error;
		}

		public UniConnectionError Error
		{
			get
			{
				return m_clientError;
			}
		}

		public override string Message
		{
			get
			{
				switch(m_clientError)
				{
				case UniConnectionError.ConnectionClosed:
					return "Соединение закрыто";
				case UniConnectionError.ErrorSuccess:
					return "Операция завершена успешно";
				case UniConnectionError.PortBusy:
					return "Порт занят";
				case UniConnectionError.IOError:
					if(InnerException != null)
						return string.Format("Ошибка ввода вывода", InnerException.Message);
					else
						return string.Format("Ошибка ввода вывода", "unknown");
				case UniConnectionError.AccessDenied:
					return "Доступ запрещен";
				case UniConnectionError.UnexpectedError:
					return "Неожижанный эррор";
				case UniConnectionError.PortNotFound:
					return "Точка подключения не найдена";
				case UniConnectionError.TimedOut:
					return "Превышено время ожидания";
				default:
					return "Неизвестная ошибка";
				}
			}
		}

	}
}
