using System;
using System.Xml.Serialization;

namespace SCADA.Common.SaveElement.SaveTransform
{
    [XmlInclude(typeof(MoveTransform)), XmlInclude(typeof(ScrollTransform))]
    [Serializable]
    public class Transform { }
}
