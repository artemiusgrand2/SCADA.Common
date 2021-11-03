using System;

namespace SCADA.Common.SaveElement
{
    [Serializable]
    public class NameColors
    {
        /// <summary>
        /// Название цвета
        /// </summary>
        public string NameColor { get; set; }
        /// <summary>
        /// Оттенок красного
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// Оттенок зеленого
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// Оттенок синего
        /// </summary>
        public byte B { get; set; }
    }
}
