
namespace SCADA.Common.Enums
{
    public enum Viewmode
    {
        /// <summary>
        ///  //нет никаного контроля
        /// </summary>
        none = 0,
        /// <summary>
        /// Сезонное управление
        /// </summary>
        seasonal_management,
        /// <summary>
        /// Передача на сезонное управление
        /// </summary>
        start_seasonal_management,
        /// <summary>
        /// Резервное управление
        /// </summary>
        reserve_control,
        /// <summary>
        /// Диспетчерское управление
        /// </summary>
        supervisory_control,
        /// <summary>
        /// Нет связи со станцией
        /// </summary>
        not_station,
        /// <summary>
        /// Пожар
        /// </summary>
        fire,
        /// <summary>
        /// автономное управление
        /// </summary>
        autonomous_control,
        /// <summary>
        /// станция не входит в диспетчерский круг
        /// </summary>
        not_supervisory_control,
        /// <summary>
        /// Отправление
        /// </summary>
        departure,
        /// <summary>
        /// Разрешение отправления
        /// </summary>
        resolution_of_origin,
        /// <summary>
        /// Ожидание отправления
        /// </summary>
        waiting_for_departure,
        /// <summary>
        /// Проезд
        /// </summary>
        passage,
        /// <summary>
        /// Поездной сигнал
        /// </summary>
        signal,
        /// <summary>
        /// Пригласительный сигнал
        /// </summary>
        invitational,
        /// <summary>
        /// Замыкание поездное
        /// </summary>
        locking,
        /// <summary>
        /// Замыкание маневровое
        /// </summary>
        lockingM,
        /// <summary>
        /// Замыкание аварийное
        /// </summary>
        lockingY,
        /// <summary>
        /// Установка
        /// </summary>
        installation,
        /// <summary>
        /// Ограждение
        /// </summary>
        fencing,
        /// <summary>
        /// Авто действие пути
        /// </summary>
        auto_run,
        /// <summary>
        /// Электрификация пути
        /// </summary>
        electrification,
        /// <summary>
        /// Имеется ли платформа на  пути
        /// </summary>
        pass,
        /// <summary>
        /// Закрытие переезда
        /// </summary>
        closing,
        /// <summary>
        /// Закрытие переезда кнопкой
        /// </summary>
        closing_button,
        /// <summary>
        /// Срабатывание контрольного объекта
        /// </summary>
        play_control_object,
        /// <summary>
        /// Занятие
        /// </summary>
        occupation,
        /// <summary>
        /// Неисправность
        /// </summary>
        fault,
        /// <summary>
        /// Авария
        /// </summary>
        accident,
        /// <summary>
        /// Маневровый сигнал
        /// </summary>
        shunting,
        /// <summary>
        /// Контроль Белый
        /// </summary>
        controlWhite,
        /// <summary>
        /// Контроль Красный
        /// </summary>
        controlRed,
        /// <summary>
        /// Контроль Желтый
        /// </summary>
        controlYellow,
        /// <summary>
        /// Контроль Красный мигающий
        /// </summary>
        controlRedF,
        /// <summary>
        /// Контроль Желтый мигающий
        /// </summary>
        controlYellowF,
        /// <summary>
        /// разделка
        /// </summary>
        cutting,
        /// <summary>
        /// возможен ли автопилот для данной станции
        /// </summary>
        auto_pilot,
        /// <summary>
        /// проверка можно ли отправлять команду на установку маршрута
        /// </summary>
        check_route,
        /// <summary>
        /// проверка проезда (стоят ли стрелки в нужном положении, открыт сигнал и маршрут замкнут)
        /// </summary>
        passage_route,
        /// <summary>
        /// задание команды (ее получение)
        /// </summary>
        assignment_command,
        /// <summary>
        /// служебный импульс астивный
        /// </summary>
        impuls_activ,
        /// <summary>
        /// импульс пассивный
        /// </summary>
        impuls_pasiv,
        /// <summary>
        /// голова поезда слева
        /// </summary>
        head_left,
        /// <summary>
        /// голова позда справа
        /// </summary>
        head_right,
        /// <summary>
        /// аналоговая индикация
        /// </summary>
        indication,
        /// <summary>
        /// длина условных вагонах
        /// </summary>
        lenghtVagon,
        /// <summary>
        /// габарит
        /// </summary>
        sizeLimit,
        /// <summary>
        /// опасный груз
        /// </summary>
        dangerous_cargo,
        /// <summary>
        /// контроль зеленого
        /// </summary>
        controlGreen,
        /// <summary>
        /// Белый мигающий
        /// </summary>
        controlWhiteF,
        /// <summary>
        /// Диспетчерское управление без упр
        /// </summary>
        supervisory_without_control,
        /// <summary>
        /// контроль зеленый мигающий
        /// </summary>
        controlGreenF,
        /// <summary>
        /// контроль черный для рамки
        /// </summary>
        controlVioletStroke,
        /// <summary>
        //минусовое положение стрелки
        /// </summary>
        minusSwitch,
        /// <summary>
        //плюсовое положение стрелки
        /// </summary>
        plusSwitch
    }
}
