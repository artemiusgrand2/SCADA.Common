using System;
using System.Collections.Generic;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения поля номеров поездов
    /// </summary>
    [Serializable]
    public class NumberTrainSave : BaseSave
    {
        /// <summary>
        /// название граничных участов
        /// </summary>
        public string Graniza { get; set; }
        /// <summary>
        /// угол поворота текста
        /// </summary>
        public double RotateText { get; set; }
    }
}
