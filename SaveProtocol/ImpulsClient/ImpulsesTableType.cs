
namespace SCADA.Common.ImpulsClient
{
    /// <summary>
    /// Тип таблицы импульсов
    /// </summary>
    public enum ImpulsesTableType
    {
        /// <summary>
        /// Таблица ТС
        /// </summary>
        TS = 0,

        /// <summary>
        /// Таблица ТУ
        /// </summary>
        TU = 1,

        /// <summary>
        /// Таблица состояния каналов
        /// </summary>
        Channels = 2,

        /// <summary>
        /// Таблица состояния блоков
        /// </summary>
        Blocks = 3
    }
}
