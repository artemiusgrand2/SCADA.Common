using System;
using System.Collections.Generic;
using SCADA.Common.Enums;


namespace SCADA.Common.Strage.SaveElement
{
   
    /// <summary>
    /// Класс описывающий  светофоры
    /// </summary>
    [Serializable]
    public class LightsProject: StrageObject
    {
        #region Переменные и свойства

        /// <summary>
        /// ориетрирование светофора (вверху или внизу расположен)
        /// </summary>
        public LandmarksLights Landmarks { get; set; }
        /// <summary>
        /// Вид светофора
        /// </summary>
        public ViewLights View { get; set; }
        /// <summary>
        /// Отабражать ли светофор
        /// </summary>
        public VisiblityLights Visible { get; set; }

        public bool SectionArrive { get; set; }

        public bool SectionLeave { get; set; }

        public bool NotDispatcherStation { get; set; }

        /// <summary>
        /// километр следующего светофора
        /// </summary>
        public double LocationNext { get; set; }
        /// <summary>
        /// контролирует ли светофор участок приближения
        /// </summary>
        public bool SectionNext { get; set; }
        /// <summary>
        /// станция контроля входного
        /// </summary>
        public int StationInputLight { get; set; }
        /// <summary>
        /// формула контроля входного
        /// </summary>
        public string ImpulsInputLight { get; set; }
        /// <summary>
        ///  название входного светофора
        /// </summary>
        public string NameInputLight { get; set; }

        private List<string> nameblock = new List<string>();
        /// <summary>
        /// Название блоков
        /// </summary>
        public List<string> NamesBlock
        {
            get
            {
                return nameblock;
            }

            set
            {
                nameblock = value;
            }
        }

        #endregion
    }
}
