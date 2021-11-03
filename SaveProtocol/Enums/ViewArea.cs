using System;

namespace SCADA.Common.Enums
{
    /// <summary>
    /// виды областей
    /// </summary>
    [Serializable]
    public enum ViewArea
    {
        table_train = 0,
        area_station,
        area_message,
        table_autopilot,
        area_picture,
        webBrowser
    }
}
