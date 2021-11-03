using System;

namespace SCADA.Common.Enums
{
    /// <summary>
    /// возможные врианты направления поворота пути
    /// </summary>
    [Serializable]
    public enum DirectionRotate
    {
        none = 0,
        //влево поворот
        left,
        //вправо поворот
        right
    }
}
