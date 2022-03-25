using System;
using System.Collections.Generic;
using System.Timers;
using System.Net.Sockets;
using System.Net;

using SCADA.Common.ImpulsClient.Interface;
using SCADA.Common.Enums;
using SCADA.Common.Log;

namespace SCADA.Common.ImpulsClient.ServerController
{
    public class Lister
    {
        #region Переменные и свойства
        /// <summary>
        /// таймер сервера импульсов
        /// </summary>
        readonly Timer timerWork;
        //сервер 
        readonly TcpListener listener = null;
        /// максимальное количество подключенных клиентов
        /// </summary>
        readonly int m_maxCountClient = 50;
        readonly DataContainer _dataContainer;
        readonly ImpulsesClientTCP _sourceImpulsServer;
        readonly IList<ICommunicationController> clientControllers;

        readonly ViewController _viewController;

        #endregion

        public Lister(int portLister, DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
            _viewController = ViewController.ISController;
            listener = new TcpListener(IPAddress.Any, portLister);
            timerWork = new System.Timers.Timer();
            timerWork.Interval = 100;
            timerWork.Elapsed += Work;
            clientControllers = new List<ICommunicationController>();
        }

        public Lister(int portLister, DataContainer dataContainer, ImpulsesClientTCP sourceImpulsServer)
        {
            _dataContainer = dataContainer;
            _sourceImpulsServer = sourceImpulsServer;
            _viewController = ViewController.TestController;
            listener = new TcpListener(IPAddress.Any, portLister);
            timerWork = new System.Timers.Timer();
            timerWork.Interval = 100;
            timerWork.Elapsed += Work;
            clientControllers = new List<ICommunicationController>();
        }

        public void Start()
        {
            //Старт связи с источником
            listener.Start();
            timerWork.Start();
        }

        public void Stop()
        {
            timerWork.Stop();
            listener.Stop();
            //останвливаем связб с клиентами
            foreach (var communicationController in clientControllers)
            {
                communicationController.Stop();
                DisposeController(communicationController);
            }
        }

        private void Work(object sender, ElapsedEventArgs e)
        {
            ServerStart();
        }

        void ServerStart()
        {
            if (listener.Pending())
            {
                if (clientControllers.Count < m_maxCountClient)
                {
                    ICommunicationController newController = CreateController();
                    newController.OnError += ClientControllerOnOnError;
                    clientControllers.Add(newController);
                    newController.Start();
                }

            }
        }

        private void ClientControllerOnOnError(ICommunicationController sender, Exception value)
        {
            lock (clientControllers)
            {
                DisposeController(sender);
                clientControllers.Remove(sender);
            }
        }

        private void DisposeController(ICommunicationController controller)
        {
            try
            {
                controller.Dispose();
            }
            catch (Exception e)
            {
                Logger.LogCommon.Info($"Отключился клиент {controller.ClientInfo}");
            }
        }

        private ICommunicationController CreateController()
        {
            switch (_viewController)
            {
                case ViewController.TestController:
                    return new TestController(listener.AcceptTcpClient(), _dataContainer, _sourceImpulsServer);
                default:
                    return new ISController(listener.AcceptTcpClient(), _dataContainer);
            }
        }
    }
}
