using System;
using System.Collections.Generic;
using SCADA.Common.SaveElement.SaveTransform;

namespace SCADA.Common.SaveElement
{

    [Serializable]
    /// <summary>
    /// класc описывающий участок графики
    /// </summary>
   public  class StrageProject
    {
        /// <summary>
        /// Коэффициент масштаба
        /// </summary>
        public double Scroll { get; set; }

        /// <summary>
        /// номер станции
        /// </summary>
        public int CurrentStation { get; set; }

        /// <summary>
        /// Ширина таблицы поездной информации
        /// </summary>
        public double WightTrainInfo { get; set; }

        /// <summary>
        /// Вымота таблицы поездной информации
        /// </summary>
        public double HeightTrainInfo { get; set; }

        /// <summary>
        /// Список графических элементов
        /// </summary>
        public List<BaseSave> GraficObjects { get; set; } = new List<BaseSave>();

        /// <summary>
        /// Список трансформаций
        /// </summary>
        public List<Transform> Transform { get; set; } = new List<Transform>();
    }
}
