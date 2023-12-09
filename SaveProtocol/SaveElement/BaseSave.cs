using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SCADA.Common.Enums;

namespace SCADA.Common.SaveElement
{
     [XmlInclude(typeof(AreaSave)), XmlInclude(typeof(ArrowMoveSave)), XmlInclude(typeof(ButtonCommandSave)), XmlInclude(typeof(LightTrainSave)), XmlInclude(typeof(LineHelpSave)),
      XmlInclude(typeof(LinePeregonSave)), XmlInclude(typeof(NameStationSave)), XmlInclude(typeof(NumberTrainSave)), XmlInclude(typeof(RoadStation)), XmlInclude(typeof(TextHelpSave)),
       XmlInclude(typeof(NameSwitchSave)),
      XmlInclude(typeof(TimeSave))]
    [Serializable]
    public class BaseSave
    {
         private bool isvisible = true;

         public bool IsVisible
         {
             get
             {
                 return isvisible;
             }

             set
             {
                 isvisible = value;
             }
         }

         private double m_currentScroll = 1;
        /// <summary>
        /// текущий маштаб
        /// </summary>
         public double CurrencyScroll
         {
             get
             {
                 return m_currentScroll;
             }
             set
             {
                 m_currentScroll = value;
             }
         }
        /// <summary>
        /// Тип разъеденителя
        /// </summary>
        public TypeDisconnectors TypeDisconnector { get; set; }
        /// <summary>
        /// Вид элемента
        /// </summary>
        public ViewElement ViewElement { get; set; }
        /// <summary>
        /// индеск слоя
        /// </summary>
        public int ZIndex { get; set; }
        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пояснения по объекту
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// Номер станции которой принадлежит
        /// </summary>
        public int StationNumber { get; set; }

        /// <summary>
        /// номер станции справа
        /// </summary>
        public int StationNumberRight { get; set; }

        private List<Figure> figure = new List<Figure>(); 
        /// <summary>
        /// Геометрия объекта
        /// </summary>
        public List<Figure> Figures
        {
            get
            {
                return figure;
            }
            set
            {
                figure = value;
            }
        }
        /// <summary>
        /// Путь к файлу для запуска по нажатию мыши
        /// </summary>
        public string FileForClick { get; set; } 
    }
}
