using System;

namespace SCADA.Common.Enums
{
    /// <summary>
    /// виды разъеденителей
    /// </summary>
    [Serializable]
    public enum TypeDisconnectors
    {
        /// <summary>
        /// Без нормального положения 
        /// </summary>
        notNormal = 0,
        /// <summary>
        /// Нормально включенный 
        /// </summary>
        normalOn,
        /// <summary>
        /// Нормально отключенный 
        /// </summary>
        normalOff
    }
}
