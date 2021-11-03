using System;
using System.Collections.Generic;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения вспомагательных линий
    /// </summary>
    [Serializable]
    public class LineHelpSave : BaseSave
    {
        /// <summary>
        /// Имя используемого цвета
        /// </summary>
        public string NameColor { get; set; }
        /// <summary>
        /// цвет оттенок красного
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// цвет оттенок зеленого
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// цвет оттенок синего
        /// </summary>
        public byte B { get; set; }
        /// <summary>
        /// Толщина вспомагательной линии
        /// </summary>
        public double WeightStroke { get; set; }

        /// <summary>
        /// Заливать ли внутри
        /// </summary>
        public bool IsFillInside { get; set; }

        List<double> strokedasharray = new List<double>();
        /// <summary>
        /// тип штрихов
        /// </summary>
        public List<double> StrokeDashArray
        {
            get
            {
                return strokedasharray;
            }
            set
            {
                strokedasharray = value;
            }
        }
    }
}
