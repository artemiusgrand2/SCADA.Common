using System;
using System.Collections.Generic;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    /// <summary>
    /// клас описывающий участок
    /// </summary>
    public class Plot
    {
        private string nameplot = string.Empty;
        private List<StragePath> moves = new List<StragePath>();

        /// <summary>
        /// название учатка
        /// </summary>
        public string NamePlot
        {
            get { return nameplot; }
            set { nameplot = value; }
        }
        /// <summary>
        /// Список перегонов с описанием
        /// </summary>
        public List<StragePath> Moves
        {
            get { return moves; }
            set { moves = value; }
        }
    }
}
