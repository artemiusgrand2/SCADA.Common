using System;

namespace SCADA.Common.Strage.SaveElement.Segment
{
    [Serializable]
    public class Segmentheight: Segment
    {
        /// <summary>
        /// приведенный на 1000 метров тангенс угла наклона
        /// </summary>
        public double Tg { get; set; }
        /// <summary>
        /// Высота
        /// </summary>
        public double Height { get; set; }
    }
}
