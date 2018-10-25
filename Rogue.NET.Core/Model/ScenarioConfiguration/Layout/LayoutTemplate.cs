﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    public class LayoutTemplate : Template
    {
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _roomDivCellHeight;
        private int _roomDivCellWidth;
        private int _numberExtraWallRemovals;
        private double _hiddenDoorProbability;
        private double _generationRatio;
        private LayoutType _type;
        private Range<int> _levelRange;

        public int NumberRoomRows
        {
            get { return _numberRoomRows; }
            set
            {
                if (_numberRoomRows != value)
                {
                    _numberRoomRows = value;
                    OnPropertyChanged("NumberRoomRows");
                }
            }
        }
        public int NumberRoomCols
        {
            get { return _numberRoomCols; }
            set
            {
                if (_numberRoomCols != value)
                {
                    _numberRoomCols = value;
                    OnPropertyChanged("NumberRoomCols");
                }
            }
        }
        public int RoomDivCellHeight
        {
            get { return _roomDivCellHeight; }
            set
            {
                if (_roomDivCellHeight != value)
                {
                    _roomDivCellHeight = value;
                    OnPropertyChanged("RoomDivCellHeight");
                }
            }
        }
        public int RoomDivCellWidth
        {
            get { return _roomDivCellWidth; }
            set
            {
                if (_roomDivCellWidth != value)
                {
                    _roomDivCellWidth = value;
                    OnPropertyChanged("RoomDivCellWidth");
                }
            }
        }
        public int NumberExtraWallRemovals
        {
            get { return _numberExtraWallRemovals; }
            set
            {
                if (_numberExtraWallRemovals != value)
                {
                    _numberExtraWallRemovals = value;
                    OnPropertyChanged("NumberExtraWallRemovals");
                }
            }
        }
        public double HiddenDoorProbability
        {
            get { return _hiddenDoorProbability; }
            set
            {
                if (_hiddenDoorProbability != value)
                {
                    _hiddenDoorProbability = value;
                    OnPropertyChanged("HiddenDoorProbability");
                }
            }
        }
        public double GenerationRate
        {
            get { return _generationRatio; }
            set
            {
                if (_generationRatio != value)
                {
                    _generationRatio = value;
                    OnPropertyChanged("GenerationRate");
                }
            }
        }
        public LayoutType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public Range<int> Level
        {
            get { return _levelRange; }
            set
            {
                if (_levelRange != value)
                {
                    _levelRange = value;
                    OnPropertyChanged("Level");
                }
            }
        }

        public LayoutTemplate() : base()
        {
            this.Type = LayoutType.Normal;
            this.Level = new Range<int>(1, 1, 100, 100);
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.RoomDivCellHeight = 20;
            this.RoomDivCellWidth = 20;
            this.NumberExtraWallRemovals = 200;
            this.HiddenDoorProbability = 0.2;
            this.GenerationRate = 0.5;
        }
    }
}