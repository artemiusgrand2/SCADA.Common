using System;
using System.Windows;

namespace SCADA.Common.SaveElement
{
   [Serializable]
    public class Segment 
    {
       public Point Point { get; set; }
       /// <summary>
       /// радиус по оси X
       /// </summary>
        public double RadiusX { get; set; }
       /// <summary>
       /// радиус по оси У
       /// </summary>
        public double RadiusY { get; set; }
    }
}
