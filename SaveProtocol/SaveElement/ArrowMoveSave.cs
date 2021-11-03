using System;
using System.Collections.Generic;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения стрелок перегона
    /// </summary>
    [Serializable]
    public class ArrowMoveSave :BaseSave
    {
        /// <summary>
        /// название граничных участов
        /// </summary>
        public string Graniza { get; set; }
    }
}
