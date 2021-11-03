using System;

namespace SCADA.Common.Strage.SaveElement.Segment
{
    [Serializable]
    public abstract class Segment
    {
        /// <summary>
        /// километр начала учаска
        /// </summary>
        public double Location { get; set; }
        /// <summary>
        /// длина участка
        /// </summary>
        public double Lenght { get; set; }
    }
}
