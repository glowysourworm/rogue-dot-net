using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class LayoutTemplate : Template
    {
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _roomDivCellHeight;
        private int _roomDivCellWidth;
        private int _numberExtraWallRemovals;
        private int _numberHallwayPoints;
        private double _hiddenDoorProbability;
        private double _generationRatio;
        private LayoutType _type;
        private Range<int> _levelRange;
        private string _wallColor;
        private string _doorColor;

        [ProtoMember(1)]
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
        [ProtoMember(2)]
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
        [ProtoMember(3)]
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
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
        public int NumberHallwayPoints
        {
            get { return _numberHallwayPoints; }
            set
            {
                if (_numberHallwayPoints != value)
                {
                    _numberHallwayPoints = value;
                    OnPropertyChanged("NumberHallwayPoints");
                }
            }
        }
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
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
        [ProtoMember(11)]
        public string WallColor
        {
            get { return _wallColor; }
            set
            {
                if (_wallColor != value)
                {
                    _wallColor = value;
                    OnPropertyChanged("WallColor");
                }
            }
        }
        [ProtoMember(12)]
        public string DoorColor
        {
            get { return _doorColor; }
            set
            {
                if (_doorColor != value)
                {
                    _doorColor = value;
                    OnPropertyChanged("DoorColor");
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
            this.NumberHallwayPoints = 10;
            this.HiddenDoorProbability = 0.2;
            this.GenerationRate = 0.5;

            this.WallColor = Colors.Blue.ToString();
            this.DoorColor = Colors.Fuchsia.ToString();
        }

        /// <summary>
        /// Calculates a simulated number of steps that the player will use to traverse the layout
        /// </summary>
        public int GetPathLength()
        {
            switch (this.Type)
            {
                default:
                case LayoutType.Normal:
                case LayoutType.Teleport:
                case LayoutType.TeleportRandom:
                case LayoutType.Hall:
                case LayoutType.BigRoom:

                    // Measure  = # of traversals * length of traversal for
                    //            a single pass only * 4;
                    return this.RoomDivCellHeight * this.NumberRoomRows *
                           this.NumberRoomCols * 4;
                case LayoutType.Maze:

                    // Measure = Made up :) 
                    return 100 * this.NumberRoomCols * this.RoomDivCellWidth *
                                 this.NumberRoomRows * this.RoomDivCellHeight;
            }
        }
    }
}