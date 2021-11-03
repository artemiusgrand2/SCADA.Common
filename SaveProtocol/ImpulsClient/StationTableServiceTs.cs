using System;
using System.Collections.Generic;
using System.Text;

using SCADA.Common.Enums;

namespace SCADA.Common.ImpulsClient
{
    /// <summary>
    /// класс описывающий весь набор служебных импульсов тс для станции 
    /// </summary>
    public class StationTableServiceTs
    {
        /// <summary>
        /// перечень объектов с описание состояниев
        /// </summary>
        public Dictionary<string, Dictionary<Viewmode, string>> NamesValue { get; set; }

        public StationTableServiceTs()
        {
            NamesValue = new Dictionary<string, Dictionary<Viewmode, string>>();
        }
    }
}
