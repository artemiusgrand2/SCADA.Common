using System;
using SCADA.Common.Enums;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    /// <summary>
    /// класс описывающий блок участок
    /// </summary>
    public class BlockPlotProject
    {
        /// <summary>
        /// название блок участка
        /// </summary>
        public string NameBlock { get; set; }
        /// <summary>
        /// название блок участка
        /// </summary>
        public ViewBlock Position { get; set; }
        /// <summary>
        /// станция которой принадлежит элемент
        /// </summary>
        public int Station { get; set; }
        /// <summary>
        /// Километр начала блок участка слево направо
        /// </summary>
        public double StartKilometr { get; set; }
        /// <summary>
        /// Километр окончания блок участка слево направо
        /// </summary>
        public double EndKilometr { get; set; }
    }
}
