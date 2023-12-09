using System;
namespace SCADA.Common.SaveElement
{
    [Serializable]
    public class NameSwitchSave : BaseSave
    {
        /// <summary>
        /// точка вставки координата Х
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// точка вставки координата У
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// Высота элемента
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// Ширина элемента
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// угол повотора текста
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// Высота теста
        /// </summary>
        public double FontSize { get; set; }
    }
}
