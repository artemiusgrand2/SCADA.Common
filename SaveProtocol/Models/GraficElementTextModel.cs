using System.Windows;
using System.Windows.Media;
using SCADA.Common.Enums;

namespace SCADA.Common.Models
{

    public class GraficElementTextModel : GraficElementModel
    {

        string text = string.Empty;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value != null)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        Brush foreground = null;

        public Brush Foreground
        {
            get
            {
                return foreground;
            }
            set
            {
                foreground = value;
                OnPropertyChanged("Foreground");
            }
        }

        double fontsize;

        public double FontSize
        {
            get
            {
                return fontsize;
            }
            set
            {
                fontsize = value;
                OnPropertyChanged("FontSize");
            }
        }

        Thickness margin;

        public Thickness Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
                OnPropertyChanged("Margin");
            }
        }

        Transform renderTransform;

        public Transform RenderTransform
        {
            get
            {
                return renderTransform;
            }
            set
            {
                renderTransform = value;
                OnPropertyChanged("RenderTransform");
            }
        }

        FontWeight fontWeight;

        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }
            set
            {
                fontWeight = value;
                OnPropertyChanged("FontWeight");
            }
        }

        TextAlignment textAlignment;

        public TextAlignment TextAlignment
        {
            get
            {
                return textAlignment;
            }
            set
            {
                textAlignment = value;
                OnPropertyChanged("TextAlignment");
            }
        }


        TextWrapping textWrapping;

        public TextWrapping TextWrapping
        {
            get
            {
                return textWrapping;
            }
            set
            {
                textWrapping = value;
                OnPropertyChanged("textWrapping");
            }
        }

        Visibility textVisibility;
        public Visibility TextVisibility
        {
            get
            {
                return textVisibility;
            }
            set
            {
                textVisibility = value;
                OnPropertyChanged("TextVisibility");
            }
        }

        int textZIndex;
        public int TextZIndex
        {
            get
            {
                return textZIndex;
            }
            set
            {
                textZIndex = value;
                OnPropertyChanged("TextZIndex");
            }
        }

        FontFamily fontFamily = new FontFamily();
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
                OnPropertyChanged("FontFamily");
            }
        }

        FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                fontStyle = value;
                OnPropertyChanged("FontStyle");
            }
        }

        FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get
            {
                return fontStretch;
            }
            set
            {
                fontStretch = value;
                OnPropertyChanged("FontStretch");
            }
        }

        public GraficElementTextModel(string name, int stationNumber, ViewElement view, TypeView typeView) : base(name, stationNumber, view, typeView)
        {

        }
        public GraficElementTextModel():base()
        {

        }


    }
}
