using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net.Sockets;

using SCADA.Common.ImpulsClient;
using SCADA.Common.ImpulsClient.requests;
using SCADA.Common.ImpulsClient.Interface;
using SCADA.Common.Enums;
using SCADA.Common.Log;

namespace SCADA.Common.ImpulsClient.ServerController
{
    public class ISController : ICommunicationController
    {
        private readonly TimeSpan _echoMessageTimeout = TimeSpan.FromSeconds(7);

        private DateTime _lastCommunicatedTime;
        private bool _isStop;
        private readonly Thread _thread;
        private readonly TcpClient _client;
        /// <summary>
        /// Обработчик пакетов данных
        /// </summary>
        private FrameParser _parser;

        private readonly RequestHandler _requestHandler;

        public event ErrorHandler<ICommunicationController, Exception> OnError;

        public string ClientInfo
        {
            get
            {
                return $"{_client.Client.RemoteEndPoint.ToString()} ImpulsServer";
            }
        }

        public ViewController View
        {
            get
            {
                return ViewController.ISController;
            }
        }

        public ISController(TcpClient client, DataContainer impContainer)
        {
            _lastCommunicatedTime = DateTime.Now;
            _client = client;
            _requestHandler = new RequestHandler(impContainer);
            _thread = new Thread(Work);
            _parser = new FrameParser();
            Logger.LogCommon.Info($"Подключился клиент {ClientInfo}");
        }

        public void Start()
        {
            if (!_isStop)
            {
                _isStop = false;
                _thread.Start();
            }
        }

        public void Stop()
        {
            _isStop = true;
            _thread.Join();
        }

        public void Dispose()
        {
            _client.Close();
        }

        private void ProcessRequest(byte[] request)
        {
            byte[] sendBuffer = new byte[4 * 1024];
            int bufferOffset = 0;
            IList<byte[]> answers = _requestHandler.HandleRequest(request);

            for (int i = 0; i < answers.Count; i++)
            {
                int answerOffset = 0;
                // надо заполнить весь буфер перед отправкой
                while (answerOffset < answers[i].Length)
                {
                    int count = 0;
                    if (sendBuffer.Length - bufferOffset <= answers[i].Length)
                    {
                        count = sendBuffer.Length - bufferOffset;
                    }
                    else
                    {
                        count = answers[i].Length - answerOffset;
                    }
                    Buffer.BlockCopy(answers[i], answerOffset, sendBuffer, bufferOffset, count);
                    answerOffset += count;
                    bufferOffset += count;
                    if (bufferOffset == sendBuffer.Length)
                    {
                        if (Write(sendBuffer, 0, bufferOffset) == 0)
                            return;
                        bufferOffset = 0;
                    }
                }
            }
            if (bufferOffset != 0)
                Write(sendBuffer, 0, bufferOffset);
        }

        private int Write(byte[] buffer, int offset, int size)
        {
            int result = 0;
            if (_client.Client != null)
            {
                try
                {
                    result = _client.Client.Send(buffer, offset, size, SocketFlags.None);
                }
                catch (ObjectDisposedException)
                {
                    return 0;
                }
                catch (SocketException)
                {
                    //Close();
                    return 0;
                }
            }
            return result;
        }

        private void Work()
        {
            try
            {
                while (!_isStop)
                {

                    if (_client.Available > 0)
                    {
                        _lastCommunicatedTime = DateTime.Now;
                        // Buffer  для чтения данных
                        _parser.Reset();
                        var bytes = new byte[_client.Available];
                        var readCount = _client.Client.Receive(bytes, 0, bytes.Length, SocketFlags.None);
                        for (int i = 0; i < readCount; i++)
                        {
                            _parser.Parse(bytes[i]);
                            if (_parser.IsFrameReady)
                                ProcessRequest(_parser.GetFrame());
                        }
                    }
                    else
                    {
                        if (DateTime.Now - _lastCommunicatedTime > _echoMessageTimeout)
                        {
                            _isStop = true;
                            if (OnError != null)
                                OnError(this, new Exception(string.Format("Client {0} disconnected", _client.Client.RemoteEndPoint.ToString())));
                        }
                        else
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError(this, e);
                }
            }
        }

    }
}
