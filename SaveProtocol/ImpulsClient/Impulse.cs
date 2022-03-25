using System;
using SCADA.Common.Enums;

namespace SCADA.Common.ImpulsClient
{

    public delegate void UpdateStateImpulsEventHandler(int stationNumber, string nameImpuls, TypeImpuls type, StatesControl newState);
    /// <summary>
    /// Класс для загрузки импульса из входн
    /// </summary>
    public class Impulse
    {
        public string Name { get; private set; }
        public int Box { get; internal set; }
        public int Contact { get; internal set; }
        public int Matrix { get; internal set; }
        /// <summary>
        /// Описание импульса
        /// </summary>
        public string ToolTip { get; internal set; }

        public TypeImpuls Type { get; private set; }

        private ImpulseState _state;

        public ImpulseState State
        {
            get
            {
                return _state;
            }
            set
            {
                if(_state != value)
                {
                    _state = value;
                    SetEventUpdateStateImpuls();
                }
            }
        }

        public StatesControl StateShort
        {
            get
            {
                return GetStateShort(_state);
            }
            set
            {
                _state = GetStateFull(value);
            }
        }

        public event UpdateStateImpulsEventHandler UpdateStateImpuls;

        int _stationNumber;

        public Impulse(string name, TypeImpuls type = TypeImpuls.ts)
        {
            SetDefult(name, type);
        }

        public Impulse(string name, TypeImpuls type, string toolTip, int stationNumber)
        {
            SetDefult(name, type);
            ToolTip = toolTip;
            _stationNumber = stationNumber;
        }

        private void SetDefult(string name, TypeImpuls type = TypeImpuls.ts)
        {
            Name = name;
            Type = type;
            //
            if (Type == TypeImpuls.ts)
                _state = ImpulseState.UncontrolledState;
            else
                _state = ImpulseState.Taken;
        }


        public static StatesControl GetStateShort(ImpulseState state)
        {
            switch (state)
            {
                case ImpulseState.ActiveState:
                    return StatesControl.activ;
                case ImpulseState.PassiveState:
                    return StatesControl.pasiv;
                default:
                    return StatesControl.nocontrol;
            }
        }

        public static ImpulseState GetStateFull(StatesControl state)
        {
            switch (state)
            {
                case StatesControl.activ:
                    return ImpulseState.ActiveState;
                case StatesControl.pasiv:
                    return ImpulseState.PassiveState;
                default:
                    return ImpulseState.UncontrolledState;
            }
        }

        private void SetEventUpdateStateImpuls()
        {
            if (UpdateStateImpuls != null)
                UpdateStateImpuls(_stationNumber, Name, Type, StateShort);
        }

    }
}
