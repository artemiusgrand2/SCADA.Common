using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using SCADA.Common.ImpulsClient.Interface;
using SCADA.Common.Enums;
using SCADA.Common.Log;
using SCADA.Common.HelpCommon;

namespace SCADA.Common.ImpulsClient.ServerController
{
    public class TestController : ICommunicationController
    {
        readonly TimeSpan _echoMessageTimeout = TimeSpan.FromSeconds(700);
        DateTime _lastCommunicatedTime;
        bool _isStop;
        readonly Thread _thread;
        readonly TcpClient _client;
        public event ErrorHandler<ICommunicationController, Exception> OnError;
        ImpulsesClientTCP _sourceImpulsServer;
        DataContainer _impContainer;
        string _bufferStr = string.Empty;
        readonly string _patterGET = @"\s*GET:([0-9]+):(.+)\s*\n";
        readonly string _patterCMDTU = @"\s*CMD:([0-9]+):(.+):(0|1)\s*\n";
        readonly string _patterCMDTS = @"\s*CMD:([0-9]+):@(.+):(0|1|2|3)\s*\n";
        readonly Encoding _encoding = Encoding.UTF8;
        public ViewController View
        {
            get
            {
                return ViewController.TestController;
            }
        }
        public string ClientInfo
        {
            get
            {
                return $"{_client.Client.RemoteEndPoint.ToString()} TESTER";
            }
        }
        public TestController(TcpClient client, DataContainer impContainer, ImpulsesClientTCP sourceImpulsServer)
        {
            _lastCommunicatedTime = DateTime.Now;
            _sourceImpulsServer = sourceImpulsServer;
            _impContainer = impContainer;
            _client = client;
            _thread = new Thread(Work);
          //  Write("start\n");
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

        private void ProcessRequest(string data)
        {
            _bufferStr += data;
            Logger.LogCommon.Info($"Из канал связи '{ClientInfo}' считана строка - '{data}'");
            //проверяю ping
            var match = Regex.Match(_bufferStr, _patterGET);
            if (match.Success)
            {
                _bufferStr = string.Empty;
                int numStation;
                var nameImp = match.Groups[2].Value.Trim();
                if ((int.TryParse(match.Groups[1].Value, out numStation) && _impContainer.Stations.ContainsKey(numStation) && _impContainer.Stations[numStation].TS.Contains(nameImp)))
                    Write($"{numStation}:{nameImp}:{(int)_impContainer.Stations[numStation].TS.GetState(nameImp)}\n");
                else
                    Write($"{numStation}:{nameImp}:NO\n");
            }
            else
            {
                match = Regex.Match(_bufferStr, _patterCMDTS);
                if (match.Success)
                {
                    _bufferStr = string.Empty;
                    int numStation;
                    var nameImp = match.Groups[2].Value;
                    if ((int.TryParse(match.Groups[1].Value, out numStation) && _impContainer.Stations.ContainsKey(numStation) && _impContainer.Stations[numStation].TS.Contains(nameImp)))
                    {
                        _impContainer.Stations[numStation].TS.SetState(nameImp, (ImpulseState)(int.Parse(match.Groups[3].Value.Trim())));
                        Write($"{numStation}:{nameImp}:OK\n");
                    }
                    else
                        Write($"{numStation}:{nameImp}:NO\n");
                }
                else
                {
                    match = Regex.Match(_bufferStr, _patterCMDTU);
                    if (match.Success)
                    {
                        _bufferStr = string.Empty;
                        int numStation;
                        var nameCommand= match.Groups[2].Value;
                        if ((int.TryParse(match.Groups[1].Value, out numStation) && _impContainer.Stations.ContainsKey(numStation) && _impContainer.Stations[numStation].TU.Contains(nameCommand)))
                        {
                            var answerServer = _sourceImpulsServer.SendImpulse(nameCommand, numStation, (ImpulseState)(int.Parse(match.Groups[3].Value.Trim())));
                            Logger.LogCommon.Info(HelpFuctions.GetDiagnostInfoForAnswerCommand(answerServer, numStation, nameCommand));
                            if (answerServer == requests.RequestError.Successful)
                                Write($"{numStation}:{nameCommand}:OK\n");
                            else
                                
                                Write($"{numStation}:{nameCommand}:NO\n");
                        }
                        else
                            Write($"{numStation}:{nameCommand}:NO\n");
                    }
                }
            }
            //
            if (_bufferStr.Length > 40)
                _bufferStr = string.Empty;
        }

        private void Write(string dataStr)
        {
            var data = _encoding.GetBytes(dataStr);
            _client.Client.Send(data, 0, data.Length, SocketFlags.None);
            Logger.LogCommon.Info($"В канал связи '{ClientInfo}' записана строка - '{dataStr}'");
            _lastCommunicatedTime = DateTime.Now;
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
                        var bytes = new byte[_client.Available];
                        var readCount = _client.Client.Receive(bytes, 0, bytes.Length, SocketFlags.None);
                        ProcessRequest(_encoding.GetString(bytes));
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
