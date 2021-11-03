using System;
using System.Collections.Generic;
using SCADA.Common.Enums;

namespace SCADA.Common.SaveElement
{
    /// <summary>
    /// класс хранения данных главного пути
    /// </summary>
    [Serializable]
    public class ButtonCommandSave :BaseSave
    {
        /// <summary>
        /// размер текста
        /// </summary>
        public double TextSize { get; set; }
        /// <summary>
        /// справочный текст
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// угол поворота текста
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// X координат точки вставки текста
        /// </summary>
        public double Xinsert { get; set; }
        /// <summary>
        /// Y координат точки вставки текста
        /// </summary>
        public double Yinsert { get; set; }
        /// <summary>
        /// Вид управляющей команды
        /// </summary>
        public ViewCommand ViewCommand { get; set; }
        /// <summary>
        /// Тип кнопки (для какой панели предназначен)
        /// </summary>
        public ViewPanel ViewPanel { get; set; }
        /// <summary>
        /// Параметры 
        /// </summary>
        public string Parametrs { get; set; } = string.Empty;
    }
}
