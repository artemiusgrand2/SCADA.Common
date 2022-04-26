using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

using SCADA.Common.HelpCommon;
using SCADA.Common.Enums;
using System.Runtime.CompilerServices;

namespace SCADA.Common.Models
{
    public class GraficElementModel : BaseModel
    {
        double _strokeThickness;
        public double StrokeThickness
        {
            get
            {
                return _strokeThickness;
            }
            set
            {
                _strokeThickness = value;
                OnPropertyChanged("StrokeThickness");
            }
        }

        Brush _stroke = null;

        public Brush Stroke
        {
            get
            {
                return _stroke;
            }
            set
            {
                _stroke = value;
                OnPropertyChanged("Stroke");
            }
        }

        Brush _fill = null;

        public Brush Fill
        {
            get
            {
                return _fill;
            }
            set
            {
                _fill = value;
                OnPropertyChanged("Fill");
            }
        }

        public IList<int>  ColorsInt
        {
            get
            {
                var _colorsInt = new List<int>() { -1, -1 };
                if (_fill != null)
                    _fill.Dispatcher.Invoke(new Action(() => { _colorsInt[0] = HelpFuctions.RGBtoInt((_fill as SolidColorBrush).Color); }));
                //
                if (_stroke != null)
                    _stroke.Dispatcher.Invoke(new Action(() => { _colorsInt[1] = HelpFuctions.RGBtoInt((_stroke as SolidColorBrush).Color); }));
                //
                return _colorsInt;
            }
        }

        Visibility _visibility;
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        int _zIndex;
        public int ZIndex
        {
            get
            {
                return _zIndex;
            }
            set
            {
                _zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }


        public ViewElement ViewElement { get; private set; }

        public string Name { get; private set; }


        public int StationNumber { get; private set; }

        public TypeView TypeView { get; private set; }

        public string InfoTestProtocol
        {
            get
            {
                return $"{StationNumber}:{(byte)TypeView}:{(byte)ViewElement}:{Name}";
            }
        }

        public GraficElementModel(string name, int stationNumber, ViewElement view, TypeView typeView)
        {
            Name = name;
            StationNumber = stationNumber; 
             ViewElement = view;
            TypeView = typeView;
        }

        public GraficElementModel()
        {
            Name = string.Empty;
            StationNumber = 0;
            ViewElement = ViewElement.none;
        }
    }
}
