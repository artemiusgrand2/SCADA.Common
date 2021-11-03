using System;
using System.Collections.Generic;
using SCADA.Common.Enums;

namespace SCADA.Common.SaveElement
{
    [Serializable]
    /// <summary>
    /// класс описывающий возможные состояния элементов и комбинации импульсов при которых они возможны
    /// </summary>
    public class StateElement
    {
        /// <summary>
        /// режим отрисовки
        /// </summary>
        public ViewElementDraw ViewControlDraw { get; set; }
        private DateTime _lastUpdate = DateTime.Now;
        /// <summary>
        /// время последнего изменения состояния
        /// </summary>
        public DateTime LastUpdate { get { return _lastUpdate; } set { _lastUpdate = value; } }
        /// <summary>
        /// состояние элемента
        /// </summary>
        public StatesControl state { get; set; }
        /// <summary>
        /// состояние элемента
        /// </summary>
        public StatesControl stateActiv { get; set; }
        /// <summary>
        /// комбинация импульсов
        /// </summary>
        public string Impuls { get; set; }
        /// <summary>
        /// название режима
        /// </summary>
        public Viewmode Name { get; set; }
        /// <summary>
        /// Было ли обновлено состояние
        /// </summary>
        public bool Update { get; set; }

        private IDictionary<StatesControl, string> m_messages = new Dictionary<StatesControl, string>();
        /// <summary>
        /// Диагностические сообщения
        /// </summary>
        public IDictionary<StatesControl, string> Messages
        {
            get
            {
                return m_messages;
            }
        }

        public StateElement()
        {
            Impuls = string.Empty;
        }

        public StateElement(IDictionary<StatesControl, string> messages)
        {
            Impuls = string.Empty;
            m_messages = messages;
        }
    }
}
