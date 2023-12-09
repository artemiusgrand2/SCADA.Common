using System;

namespace SCADA.Common.Enums
{
    /// <summary>
    /// виды графических элементов
    /// </summary>
    [Serializable]
    public enum ViewElement
    {
        none = 0,
        area,
        area_message,
        buttonstation,
        namestation,
        numbertrain,
        arrowmove,
        chiefroad,
        signal,
        kgu,
        ktcm,
        move,
        ramka,
        line,
        otrezok,
        time,
        buttoncommand,
        lightShunting,
        lightTrain,
        texthelp,
        help_element,
        tablenumbertrain,
        area_station,
        tableautopilot,
        area_picture,
        disconnectors,
        analogCell,
        diagnostikCell,
        webBrowser,
        nameSwitch
    }
}
