using System;
using System.Collections.Generic;
using SCADA.Common.Strage.SaveElement.Segment;

namespace SCADA.Common.Strage.SaveElement
{
    /// <summary>
    /// класс описывающй справочные данные по перегону безактивных элементов (ктсм)
    /// </summary>
    [Serializable]
    public class StrageInfoProject
    {
        private List<double> _collectionheight = new List<double>();
        private List<Segmentrotate> _collectionrotate = new List<Segmentrotate>();
        /// <summary>
        /// Название станции слева
        /// </summary>
        public string Stationnameleft { get; set; }
        /// <summary>
        /// Номер станции слева
        /// </summary>
        public int Stationnumberleft { get; set; }
        /// <summary>
        /// Название станции справа
        /// </summary>
        public string Stationnameright { get; set; }
        /// <summary>
        /// Номер станции справа
        /// </summary>
        public int Stationnumberright { get; set; }
        /// <summary>
        /// Название перегонного пути
        /// </summary>
        public string NameMove { get; set; }
        /// <summary>
        /// начало перегона (координата)
        /// </summary>
        public double Start { get; set; }
        /// <summary>
        /// окончание перегона (координата)
        /// </summary>
        public double End { get; set; }
        /// <summary>
        /// коллекция подъемов и спусков
        /// </summary>
        public List<double> CollectionHeight
        {
            get
            {
                return _collectionheight;
            }
            set
            {
                _collectionheight = value;
            }
        }

        /// <summary>
        /// коллекция горизонтальных поворотов
        /// </summary>
        public List<Segmentrotate> CollectionRotate
        {
            get
            {
                return _collectionrotate;
            }
            set
            {
                _collectionrotate = value;
            }
        }

        public void Sort()
        {
            if (Start < End)
                _collectionrotate.Sort(SortDeck);
            else _collectionrotate.Sort(SortAck);
        }

        private int SortDeck(Segment.Segment x, Segment.Segment y)
        {
            if (x.Location > y.Location)
                return 1;
            if (x.Location < y.Location)
                return -1;
            return 0;
        }

        private int SortAck(Segment.Segment x, Segment.Segment y)
        {
            if (x.Location > y.Location)
                return -1;
            if (x.Location < y.Location)
                return 1;
            return 0;
        }
    }
}
