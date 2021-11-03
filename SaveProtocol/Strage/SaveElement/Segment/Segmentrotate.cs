using System;
using SCADA.Common.Enums;

namespace SCADA.Common.Strage.SaveElement.Segment
{
    [Serializable]
    public class Segmentrotate : Segment
    {
        /// <summary>
        /// радиус поворота
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// направление поворота
        /// </summary>
        public DirectionRotate Direction { get; set; }
    }
}
