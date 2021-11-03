using System;

namespace SCADA.Common.Enums
{
    /// <summary>
    /// виды комманд выполнения
    /// </summary>
    [Serializable]
    public enum ViewCommand
    {
        none = 0,
        diagnostics,
        numbertrain,
        sound,
        filter_train,
        help,
        style,
        train_odd,
        train_even,
        train_unknow,
        content_help,
        content_exchange,
        show_control,
        show_table_train,
        viewtrain,
        update_style,
        numbertrack,
        pass,
        electro,
        exit,
        run_auto_supervisory,
        show_command_not_auto_supervisory
    }
}
