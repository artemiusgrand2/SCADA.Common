using System;

namespace SCADA.Common.SaveElement.SaveTransform
{
    [Serializable]
    public class ScrollTransform : Transform
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
    }
}
