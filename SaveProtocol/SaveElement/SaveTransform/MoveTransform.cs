using System;

namespace SCADA.Common.SaveElement.SaveTransform
{
    [Serializable]
    public class MoveTransform : Transform
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
