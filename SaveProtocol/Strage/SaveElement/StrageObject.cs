using System;
using System.Collections.Generic;
using SCADA.Common.SaveElement;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    /// <summary>
    /// класс, описывающий объект перегона
    /// </summary>
    public class StrageObject
    {
        private List<StateElement> _impulses = new List<StateElement>();
        /// <summary>
        /// Код станции к которой принадлежит объекту перегона
        /// </summary>
        public int Station { get; set; }
        /// <summary>
        /// Название объекта перегона
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// километр на котором находится объект перегона
        /// </summary>
        public double Location { get; set; }
        /// </summary>
        /// коллекция возможных состояний элемента объекта перегона
        /// </summary>
        public List<StateElement> Impulses
        {
            get
            {
                return _impulses;
            }
            set
            {
                _impulses = value;
            }
        }
    }
}
