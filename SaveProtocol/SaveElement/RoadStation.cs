using System;
using System.Collections.Generic;
using SCADA.Common.Enums;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения данных главного пути
    /// </summary>
    [Serializable]
    public class RoadStation : BaseSave
    {
        /// <summary>
        /// размер текста
        /// </summary>
        public double TextSize { get; set; }
        /// <summary>
        /// угол поворота текста
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// X координат точки вставки текста
        /// </summary>
        public double Xinsert { get; set; }
        /// <summary>
        /// Y координат точки вставки текста
        /// </summary>
        public double Yinsert { get; set; }
        /// <summary>
        /// вид элемента
        /// </summary>
        public ViewTrack View { get; set; }

        private double m_factor = 1;
        //
        public double Factor
        {
            get
            {
                return m_factor;
            }
            set
            {
                m_factor = value;
            }
        }

        private string m_format = "000.00";

        public string Format
        {
            get
            {
                return m_format;
            }
            set
            {
                m_format = value;
            }
        }
    }
}
