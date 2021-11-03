using System;
using System.Collections.Generic;
using System.Timers;
using System.Net.Sockets;
using System.Net;
using SCADA.Common.ImpulsClient.Interface;

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

        readonly IList<ICommunicationController> clientControllers;

        #endregion

        public Lister(int portLister, DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
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
                    var newISController = new ISController(listener.AcceptTcpClient(), _dataContainer);
                    newISController.OnError += ClientControllerOnOnError;
                    clientControllers.Add(newISController);
                    newISController.Start();
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
                // Logger.Log.LogError("Can't free client. {0}", e);
            }
        }
    }
}
