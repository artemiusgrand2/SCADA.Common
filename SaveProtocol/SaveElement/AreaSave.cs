using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SCADA.Common.Enums;

namespace SCADA.Common.SaveElement
{
    [Serializable]
    public class AreaSave : BaseSave
    {
        /// <summary>
        /// вид области
        /// </summary>
        public ViewArea View { get; set; }
        /// <summary>
        /// Путь к какому - либо ресурсу
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Угол поворота
        /// </summary>
        public double Angle { get; set; }

        public double ZoomLevelIncrement { get; set; } = 0.1;

        public double ZoomLevel { get; set; }

    }
}
