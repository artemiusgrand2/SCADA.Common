using System;
using System.Collections.Generic;

namespace SCADA.Common.Strage.SaveElement
{
    /// <summary>
    /// описание перегона
    /// </summary>
    public class StrageAll
    {
        /// <summary>
        /// станция слева
        /// </summary>
        public int StationLeft { get; set; }
        /// <summary>
        /// станция справа
        /// </summary>
        public int StationRight { get; set; }
        Dictionary<string, StragePath> stragepaths = new Dictionary<string, StragePath>();
        /// <summary>
        /// пути перегона
        /// </summary>
        public Dictionary<string, StragePath> StragePaths
        {
            get { return stragepaths; }
            set { stragepaths = value; }
        }
    }
}
