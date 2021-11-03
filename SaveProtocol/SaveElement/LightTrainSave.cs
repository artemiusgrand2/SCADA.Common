using System;
using System.Collections.Generic;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения данных поездного светофора
    /// </summary>
    [Serializable]
    public class LightTrainSave : BaseSave
    {
        /// <summary>
        /// является светофор входным
        /// </summary>
        public bool IsInput { get; set; }
    }
}
