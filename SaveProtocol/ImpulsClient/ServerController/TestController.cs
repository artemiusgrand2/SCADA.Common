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
using SCADA.Common.Models;
using SCADA.Common.SyncCollections;

namespace SCADA.Common.ImpulsClient.ServerController
{
    public delegate void ServiceCommandHandler(int stationNumber, ViewServiceCommand typeCommand);
    public class TestController : ICommunicationController
    {
        readonly TimeSpan _echoMessageTimeout = TimeSpan.FromSeconds(1000000000);
        DateTime _lastCommunicatedTime;
        bool _isStop;
        readonly Thread _thread;
        readonly TcpClient _client;
        public event ErrorHandler<ICommunicationController, Exception> OnError;
        public event ServiceCommandHandler OnServiceCommand;
        ImpulsesClientTCP _sourceImpulsServer;
        DataContainer _impContainer;
        ThreadSafeList<GraficElementModel> _grafics;
        string _bufferStr = string.Empty;
        readonly string _patterGETTS = @"\s*GET:([0-9]+):(.+)\s*\n";
        readonly string _patterGETGR = @"\s*GET:([0-9]+):([0-9]+):([0-9]+):(.+)\s*\n";
        readonly string _patterCMDTU = @"\s*CMD:([0-9]+):(.+):(0|1)\s*\n";
        readonly string _patterCMDTS = @"\s*CMD:([0-9]+):@(.+):(0|1|2|3)\s*\n";
        readonly string _patterCMDST = @"\s*CMD:([0-9]+):open\s*\n";
        readonly Encoding _encoding =  Encoding.GetEncoding(1251);
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

        public TestController(TcpClient client, DataContainer impContainer, ImpulsesClientTCP sourceImpulsServer, ThreadSafeList<GraficElementModel> grafics = null)
        {
            _lastCommunicatedTime = DateTime.Now;
            _sourceImpulsServer = sourceImpulsServer;
            _impContainer = impContainer;
            _client = client;
            _grafics = grafics;
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
            //
            var match = Regex.Match(_bufferStr, _patterGETGR);
            if (match.Success)
            {
                _bufferStr = string.Empty;
                int numStation;
                byte view, typeView;
                var nameGr = match.Groups[4].Value.Trim();
                if (int.TryParse(match.Groups[1].Value, out numStation) && byte.TryParse(match.Groups[2].Value, out typeView)
                    && byte.TryParse(match.Groups[3].Value, out view) && _grafics != null)
                {
                    var findGrafics = _grafics.Where(x => x.StationNumber == numStation && x.TypeView == (TypeView)typeView && x.Name == nameGr && x.ViewElement == (ViewElement)view).ToList();
                    if (findGrafics.Count > 0)
                    {
                        var strBuilderAnswer = new StringBuilder();
                        strBuilderAnswer.Append($"{numStation}:{typeView}:{view}:{nameGr}");
                        var colorId = 0;
                        foreach (var color in findGrafics.SelectMany(x => x.ColorsInt))
                        {
                            strBuilderAnswer.Append($":{colorId},{color}");
                            colorId++;
                        }
                        Write($"{strBuilderAnswer.ToString()}\n");
                    }
                    else
                        Write($"{numStation}:{typeView}:{view}:{nameGr}:NO\n");
                }
                else
                    Write($"{match.Groups[1].Value}:{match.Groups[2].Value}:{match.Groups[3].Value}:{nameGr}:NO\n");
            }
            else
            {
                match = Regex.Match(_bufferStr, _patterGETTS);
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
                        var isAllImp = nameImp.Trim().ToUpper() == "ALL";
                        if (int.TryParse(match.Groups[1].Value, out numStation) && _impContainer.Stations.ContainsKey(numStation) 
                            && (_impContainer.Stations[numStation].TS.Contains(nameImp)|| isAllImp ))
                        {
                            var setValueImp = (ImpulseState)(int.Parse(match.Groups[3].Value.Trim()));
                            if (isAllImp)
                                _impContainer.Stations[numStation].TS.SetAllStates(setValueImp, DateTime.Now);
                            else
                                _impContainer.Stations[numStation].TS.SetState(nameImp, setValueImp);
                            //
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
                            var nameCommand = match.Groups[2].Value;
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
                        else
                        {
                            match = Regex.Match(_bufferStr, _patterCMDST);
                            if (match.Success)
                            {
                                _bufferStr = string.Empty;
                                int numStation;
                                if (int.TryParse(match.Groups[1].Value, out numStation))
                                {
                                    Write($"{numStation}:open:OK\n");
                                    if (OnServiceCommand != null)
                                        OnServiceCommand(numStation, ViewServiceCommand.openDetailView);
                                }
                                else
                                    Write($"{numStation}:open:NO\n");
                            }
                        }
                    }
                }
            }
            //
            if (_bufferStr.IndexOf("\n") != -1)
            {
                _bufferStr = string.Empty;
                Write("ERR_FORMAT\n");
            }
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
