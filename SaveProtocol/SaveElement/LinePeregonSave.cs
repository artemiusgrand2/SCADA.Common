using System;
using System.Collections.Generic;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения линий перегона
    /// </summary>
    [Serializable]
    public class LinePeregonSave : BaseSave
    {
        /// <summary>
        /// название граничных участов
        /// </summary>
        public string Graniza { get; set; }
        /// <summary>
        /// ключ для каждого отрезка путь
        /// </summary>
        public string Key { get; set; }
    }
}
