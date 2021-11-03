using System;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    public class Direction
    {
        private bool constant_rotation = false;
        /// <summary>
        /// Шестизначый номер станции
        /// </summary>
        public int Station { get; set; }
        /// <summary>
        /// значение формулы для отправления
        /// </summary>
        public string ImpulsesDeparture { get; set; }
        /// <summary>
        /// значение формулы для ожидания отправления
        /// </summary>
        public string ImpulsesWaitDeparture { get; set; }
        /// <summary>
        /// значение формулы для разрешения отправления
        /// </summary>
        public string ImpulsesResolutionDeparture { get; set; }
        /// <summary>
        /// свойство показыает повернут ли перегон постоянно в одном направлении
        /// </summary>
        public bool ConstRotation
        {
            get
            {
                return constant_rotation;
            }
            set
            {
                constant_rotation = value;
            }
        }
    }
}
