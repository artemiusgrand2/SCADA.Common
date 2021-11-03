using System;

namespace SCADA.Common.SaveElement
{
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// точка вставки форма рисования графики по оси X
        /// </summary>
        public double StartGrafic_X { get; set; }
        /// <summary>
        /// точка вставки форма рисования графики по оси Y
        /// </summary>
        public double StartGrafic_Y { get; set; }
        /// <summary>
        /// ширина форма рисования графики
        /// </summary>
        public double StartGrafic_Widht { get; set; }
        /// <summary>
        ///  высота форма рисования графики
        /// </summary>
        public double StartGrafic_Height { get; set; }


        //------------------------------------------

        /// <summary>
        /// точка вставки форма командования  по оси X
        /// </summary>
        public double StartCommand_X { get; set; }
        /// <summary>
        /// точка вставки форма командования  по оси Y
        /// </summary>
        public double StartCommand_Y { get; set; }
        /// <summary>
        /// ширина форма командования графики
        /// </summary>
        public double StartCommand_Width { get; set; }
        /// <summary>
        ///  ширина форма командования графики
        /// </summary>
        public double StartCommand_Height { get; set; }

        //------------------------------------------

        /// <summary>
        /// точка вставки форма командования  по оси X
        /// </summary>
        public double StartSettings_X { get; set; }
        /// <summary>
        /// точка вставки форма командования  по оси Y
        /// </summary>
        public double StartSettings_Y { get; set; }
        /// <summary>
        /// ширина форма командования графики
        /// </summary>
        public double StartSettings_Width { get; set; }
        /// <summary>
        ///  ширина форма командования графики
        /// </summary>
        public double StartSettings_Height { get; set; }
    }
}
