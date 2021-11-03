using System;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    public class TrainElementProject
    {
        /// <summary>
        /// Шестизначый номер станции
        /// </summary>
        public int Station { get; set; }
        /// <summary>
        /// коллекция возможных состояний элемента блок участок
        /// </summary>
        public string Impulses { get; set; }
    }
}
