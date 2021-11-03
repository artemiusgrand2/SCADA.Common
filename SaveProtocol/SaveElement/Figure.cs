using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SCADA.Common.SaveElement
{
    [Serializable]
    public class Figure
    {
        private List<Segment> _segment = new List<Segment>();
        /// <summary>
        /// точка тачала риосвания
        /// </summary>
        public Point StartPoint { get; set; }
        /// <summary>
        /// коллекция сегментов
        /// </summary>
        public List<Segment> Segments 
        {
            get
            {
                return _segment;
            }
            set
            {
                _segment = value;
            }
        }
    }

    public enum ViewSave
    {
        save = 0,
        copy = 1
    }
}
