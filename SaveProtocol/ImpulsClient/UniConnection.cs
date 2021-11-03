using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace SCADA.Common.ImpulsClient
{
	delegate void ClientDisconnectDelegate(object sender, EventArgs args);

	class UniConnection
	{
		#region Данные
		/// <summary>
		/// Клиентский сокет
		/// </summary>
		private Socket m_socket;

		/// <summary>
		/// Признак закрытого соединения
		/// </summary>
		private bool m_closed = true;

		/// <summary>
		/// Название объекта
		/// </summary>
		private string m_name;
		#endregion

		public UniConnection()
		{
			m_socket = null;
			m_closed = true;
			m_name = "unknown";
		}

		public UniConnection(object layer)
		{
			if(layer is Socket)
			{
				m_socket = (Socket)layer;
				m_name = m_socket.RemoteEndPoint.ToString();
				m_socket.ReceiveTimeout = 1000;
			}
			m_closed = false;
		}

		public bool IsOpen
		{
			get
			{
				return !m_closed;
			}
		}

		/// <summary>
		/// Количество байт доступных для чтения
		/// </summary>
		/// <exception cref="UniConnectionException">Произошла ошибка</exception>
		public int Available
		{
			get
			{
				try
				{
					if(m_socket != null)
						return m_socket.Available;
				}
				catch(SocketException e)
				{
					throw new UniConnectionException(UniConnectionError.IOError, e);
				}
				catch(ObjectDisposedException e)
				{
					throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
				}

				throw new UniConnectionException(UniConnectionError.ConnectionClosed);
			}
		}

		/// <summary>
		/// Событие на отключение клиента
		/// </summary>
		public event ClientDisconnectDelegate OnDisconnect;

		/// <summary>
		/// Открыть клиентское соединение
		/// </summary>
		/// <param name="uri">Идентификатор подключения</param>
		/// <exception cref="ClientException">Если возникло исключение при открытии порта или подключении через сокет</exception>
		/// <exception cref="ArgumentException">Если строка подключения имеет неправильный формат</exception>
		public void Open(string uri)
		{
			if(!m_closed)
				Close();

			if(uri == null)
				throw new ArgumentNullException("uri");
			if(uri.Length == 0)
				throw new ArgumentNullException("uri");

			System.Text.RegularExpressions.Match match = 
				System.Text.RegularExpressions.Regex.Match(uri, @"^(\w+)://(\S+):(\d+)$");
			
			if(!match.Success)
				throw new ArgumentException("Неверный формат адреса сервера импульсов: " + uri);

			string host;
			int port = 0;
			bool isTCP = true;

			if(string.Compare(match.Groups[1].Value, "tcp", true) == 0)
				isTCP = true;
			if(string.Compare(match.Groups[1].Value, "serial", true) == 0)
				isTCP = false;

			host = match.Groups[2].Value;
			if(!int.TryParse(match.Groups[3].Value, out port))
				throw new ArgumentException("Неверный формат адреса сервера импульсов: " + uri);
		
			if(isTCP)
			{
				try
				{
					m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//					DiagnosticManager.Instance.new_message_impulses_server("Создал сокет");
				}
				catch(SystemException e)
				{
//					DiagnosticManager.Instance.new_message_impulses_server("Ошибка при создании сокета - " + e.ToString());
					//не установлен tcpip
					System.Console.WriteLine(string.Format("new Socket(...): {0}", e.Message));
					throw new UniConnectionException(UniConnectionError.UnexpectedError, e);
					
				}
				m_socket.ReceiveTimeout = 5000;
				try
				{
					m_socket.Connect(host, port);
//					DiagnosticManager.Instance.new_message_impulses_server("соединяюсь");
				}
				catch(SocketException e)
				{
//					DiagnosticManager.Instance.new_message_impulses_server(string.Format("ошибка в соединении {0}", e.ToString()));
					m_socket.Close();
					m_socket = null;
					UniConnectionError error = UniConnectionError.UnexpectedError;
					switch(e.SocketErrorCode)
					{
					case SocketError.AccessDenied:
						error = UniConnectionError.AccessDenied;
						break;
					case SocketError.AddressNotAvailable:
						//удалённый компьютер не найден
						error = UniConnectionError.PortNotFound;
						break;
					case SocketError.NetworkUnreachable:
						//удалённый компьютер не найден
						error = UniConnectionError.PortNotFound;
						break;
					}
					throw new UniConnectionException(error, e);
				}
				catch(ObjectDisposedException)
				{
//					DiagnosticManager.Instance.new_message_impulses_server("Ошибка при подключении сокета - " + e.ToString());
					//сокет был закрыт, непонятно как это исключение может здесь возникнуть
					System.Diagnostics.Debug.WriteLine("???ObjectDisposedException in Client.Open.m_socket.Connect");
					//throw new UniConnectionException(UniConnectionError.UnexpectedError, e);
					
				}
				catch(ArgumentOutOfRangeException)
				{
					m_socket.Close();
					m_socket = null;
					//указан неправильный порт
					throw;
				}
				Console.WriteLine(string.Format("Client successfully connected to {0}", m_socket.RemoteEndPoint));
//				DiagnosticManager.Instance.new_message_impulses_server(string.Format("Client successfully connected to {0}", m_socket.RemoteEndPoint));
				m_name = m_socket.RemoteEndPoint.ToString();
			}
			m_closed = false;
		}

		/// <summary>
		/// Закрыть соединение
		/// </summary>
		public void Close()
		{
			if(!m_closed)
			{
				m_closed = true;
				if(m_socket != null)
				{
					m_socket.Shutdown(SocketShutdown.Send);
					m_socket.Close();
				}
				if(OnDisconnect != null)
					OnDisconnect(this, null);
				else
					System.Diagnostics.Debug.WriteLine(string.Format("Client {0} disconnected", m_name));
			}
		}

		/// <summary>
		/// Записать данные
		/// </summary>
		/// <param name="buffer">Буфер для записи</param>
		/// <param name="offset">Смещение в буфере</param>
		/// <param name="size">Количество записываемых байт</param>
		/// <returns>Количество записанных байт</returns>
		/// <exception cref="UniConnectionException">Если соединение закрыто, возникла ошибка
		/// или превышен таймаут.</exception>
		public int Write(byte[] buffer, int offset, int size)
		{
			if(m_socket != null)
			{
				try
				{
					return m_socket.Send(buffer, offset, size, SocketFlags.None);
				}
				catch(ObjectDisposedException)
				{
					throw new UniConnectionException(UniConnectionError.UnexpectedError);
				}
				catch(SocketException e)
				{
					switch(e.SocketErrorCode)
					{
					case SocketError.NetworkReset:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed);
					case SocketError.NotConnected:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed);
					case SocketError.Shutdown:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed);
					case SocketError.HostUnreachable:
						throw new UniConnectionException(UniConnectionError.PortNotFound);
					case SocketError.ConnectionAborted:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed);
					case SocketError.ConnectionReset:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed);
					case SocketError.TimedOut:
						throw new UniConnectionException(UniConnectionError.TimedOut);
					default:
						throw new UniConnectionException(UniConnectionError.UnexpectedError);
					}
				}
			}
			throw new UniConnectionException(UniConnectionError.UnexpectedError);
		}

		/// <summary>
		/// Прочитать данные
		/// </summary>
		/// <param name="buffer">Буффер для чтения</param>
		/// <param name="offset">Смещение в буфере</param>
		/// <param name="count">Количество байт для чтения</param>
		/// <returns>Количество прочитанных байт</returns>
		/// <exception cref="UniConnectionException">Если соединение закрыто, 
		/// возникла ошибка или превышен таймаут.</exception>
		public int Read(byte[] buffer, int offset, int count)
		{
			if(m_socket != null)
			{
				int readed = 0;
				try
				{
                    m_socket.ReceiveTimeout = 3000;
					readed = m_socket.Receive(buffer, offset, count, SocketFlags.None);
					//m_logger.Debug("Кол-во принятых байт: " + readed);
				}
				catch(SocketException e)
				{
					switch(e.SocketErrorCode)
					{
					case SocketError.NetworkReset:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
					case SocketError.NotConnected:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
					case SocketError.Shutdown:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
					case SocketError.HostUnreachable:
						throw new UniConnectionException(UniConnectionError.PortNotFound, e);
					case SocketError.ConnectionAborted:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
					case SocketError.ConnectionReset:
						throw new UniConnectionException(UniConnectionError.ConnectionClosed, e);
					case SocketError.TimedOut:
						throw new UniConnectionException(UniConnectionError.TimedOut, e);
					case SocketError.WouldBlock:
						throw new UniConnectionException(UniConnectionError.TimedOut, e);
					default:
						throw new UniConnectionException(UniConnectionError.UnexpectedError, e);
					}
				}
				catch(SystemException)
				{
					throw new UniConnectionException(UniConnectionError.UnexpectedError);
				}

				if(readed == 0)
					throw new UniConnectionException(UniConnectionError.ConnectionClosed);
				else
					return readed;
			}

			throw new UniConnectionException(UniConnectionError.UnexpectedError);
		}

		public override string ToString()
		{
			return m_name;
		}
	}
}
