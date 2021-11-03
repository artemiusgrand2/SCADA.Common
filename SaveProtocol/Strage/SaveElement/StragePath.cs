using System;
using System.Collections.Generic;

namespace SCADA.Common.Strage.SaveElement
{
    [Serializable]
    /// <summary>
    /// полное описание конкретного пути перегона
    /// </summary>
    public class StragePath
    {

        #region Переменные

        private StrageInfoProject _infostrage = new StrageInfoProject();
        private List<BlockPlotProject> _blockplots = new List<BlockPlotProject>();
        private List<LightsProject> _lightsmoves = new List<LightsProject>();
        private Direction _left = new Direction();
        private Direction _right = new Direction();
        private TrainElementProject _trainumber = new TrainElementProject();
        private List<StrageObject> _moveelements = new List<StrageObject>();
        private List<StrageObject> _kguelements = new List<StrageObject>();
        private List<StrageObject> _ktcmelements = new List<StrageObject>();

        #endregion

        #region Свойства

        /// <summary>
        /// Название левого участка приближения
        /// </summary>
        public string NameLeftBlock { get; set; }
        /// <summary>
        /// Название правого участка приближения
        /// </summary>
        public string NameRightBlock { get; set; }
        /// <summary>
        /// класс описывающий неактивные элементы перегона
        /// </summary>
        public StrageInfoProject Infostrage
        {
            get
            {
                return _infostrage;
            }
            set
            {
                _infostrage = value;
            }
        }
        /// <summary>
        /// коллекция клок участков
        /// </summary>
        public List<BlockPlotProject> Blockplots
        {
            get
            {
                return _blockplots;
            }
            set
            {
                _blockplots = value;
            }
        }
        /// <summary>
        /// коллекция светофоров
        /// </summary>
        public List<LightsProject> Lightsmoves
        {
            get
            {
                return _lightsmoves;
            }
            set
            {
                _lightsmoves = value;
            }
        }
        /// <summary>
        /// показывает левый поворот перегна
        /// </summary>
        public Direction Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
            }
        }
        /// <summary>
        /// показывает правый поворот перегна
        /// </summary>
        public Direction Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
            }
        }
        /// <summary>
        /// показывает занятость перегона и номера поездов
        /// </summary>
        public TrainElementProject Trainumber
        {
            get
            {
                return _trainumber;
            }
            set
            {
                _trainumber = value;
            }
        }
        /// <summary>
        /// коллекция переездов
        /// </summary>
        public List<StrageObject> MoveElements
        {
            get
            {
                return _moveelements;
            }
            set
            {
                _moveelements = value;
            }
        }
        /// <summary>
        /// коллекция КГУ
        /// </summary>
        public List<StrageObject> KGUelements
        {
            get
            {
                return _kguelements;
            }
            set
            {
                _kguelements = value;
            }
        }
        /// <summary>
        /// коллекция КТСМ
        /// </summary>
        public List<StrageObject> KTCMelements
        {
            get
            {
                return _ktcmelements;
            }
            set
            {
                _ktcmelements = value;
            }
        }

        #endregion
    }
}
