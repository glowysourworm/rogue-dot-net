using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    public class LayoutTemplate : Template
    {
        private int _width;
        private int _height;
        private int _roomHeightLimit;
        private int _roomWidthLimit;
        private int _roomHeightMin;
        private int _roomWidthMin;
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _rectangularGridPadding;
        private int _randomRoomCount;
        private int _randomRoomSpread;
        private int _numberExtraWallRemovals;
        private double _hiddenDoorProbability;
        private double _generationRatio;
        private double _cellularAutomataFillRatio;
        private LayoutType _type;
        private LayoutCellularAutomataType _cellularAutomataType;
        private LayoutRoomPlacementType _roomPlacementType;
        private LayoutConnectionType _connectionType;
        private LayoutConnectionGeometryType _connectionGeometryType;
        private LayoutCorridorGeometryType _corridorGeometryType;
        private Range<int> _levelRange;
        private SymbolDetailsTemplate _wallSymbol;
        private SymbolDetailsTemplate _doorSymbol;


        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged("Width");
                }
            }
        }
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged("Height");
                }
            }
        }
        public int RoomWidthLimit
        {
            get { return _roomWidthLimit; }
            set
            {
                if (_roomWidthLimit != value)
                {
                    _roomWidthLimit = value;
                    OnPropertyChanged("RoomWidthLimit");
                }
            }
        }
        public int RoomHeightLimit
        {
            get { return _roomHeightLimit; }
            set
            {
                if (_roomHeightLimit != value)
                {
                    _roomHeightLimit = value;
                    OnPropertyChanged("RoomHeightLimit");
                }
            }
        }
        public int RoomWidthMin
        {
            get { return _roomWidthMin; }
            set
            {
                if (_roomWidthMin != value)
                {
                    _roomWidthMin = value;
                    OnPropertyChanged("RoomWidthMin");
                }
            }
        }
        public int RoomHeightMin
        {
            get { return _roomHeightMin; }
            set
            {
                if (_roomHeightMin != value)
                {
                    _roomHeightMin = value;
                    OnPropertyChanged("RoomHeightMin");
                }
            }
        }
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
        public int RectangularGridPadding
        {
            get { return _rectangularGridPadding; }
            set
            {
                if (_rectangularGridPadding != value)
                {
                    _rectangularGridPadding = value;
                    OnPropertyChanged("RectangularGridPadding");
                }
            }
        }
        public int RandomRoomCount
        {
            get { return _randomRoomCount; }
            set
            {
                if (_randomRoomCount != value)
                {
                    _randomRoomCount = value;
                    OnPropertyChanged("RandomRoomCount");
                }
            }
        }
        public int RandomRoomSpread
        {
            get { return _randomRoomSpread; }
            set
            {
                if (_randomRoomSpread != value)
                {
                    _randomRoomSpread = value;
                    OnPropertyChanged("RandomRoomSpread");
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
        public double CellularAutomataFillRatio
        {
            get { return _cellularAutomataFillRatio; }
            set
            {
                if (_cellularAutomataFillRatio != value)
                {
                    _cellularAutomataFillRatio = value;
                    OnPropertyChanged("CellularAutomataFillRatio");
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
        public LayoutCellularAutomataType CellularAutomataType
        {
            get { return _cellularAutomataType; }
            set
            {
                if (_cellularAutomataType != value)
                {
                    _cellularAutomataType = value;
                    OnPropertyChanged("CellularAutomataType");
                }
            }
        }
        public LayoutRoomPlacementType RoomPlacementType
        {
            get { return _roomPlacementType; }
            set
            {
                if (_roomPlacementType != value)
                {
                    _roomPlacementType = value;
                    OnPropertyChanged("RoomPlacementType");
                }
            }
        }
        public LayoutConnectionType ConnectionType
        {
            get { return _connectionType; }
            set
            {
                if (_connectionType != value)
                {
                    _connectionType = value;
                    OnPropertyChanged("ConnectionType");
                }
            }
        }
        public LayoutConnectionGeometryType ConnectionGeometryType
        {
            get { return _connectionGeometryType; }
            set
            {
                if (_connectionGeometryType != value)
                {
                    _connectionGeometryType = value;
                    OnPropertyChanged("ConnectionGeometryType");
                }
            }
        }
        public LayoutCorridorGeometryType CorridorGeometryType
        {
            get { return _corridorGeometryType; }
            set
            {
                if (_corridorGeometryType != value)
                {
                    _corridorGeometryType = value;
                    OnPropertyChanged("CorridorGeometryType");
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
        public SymbolDetailsTemplate WallSymbol
        {
            get { return _wallSymbol; }
            set
            {
                if (_wallSymbol != value)
                {
                    _wallSymbol = value;
                    OnPropertyChanged("WallSymbol");
                }
            }
        }
        public SymbolDetailsTemplate DoorSymbol
        {
            get { return _doorSymbol; }
            set
            {
                if (_doorSymbol != value)
                {
                    _doorSymbol = value;
                    OnPropertyChanged("DoorSymbol");
                }
            }
        }

        public LayoutTemplate() : base()
        {
            this.Width = 600;
            this.Height = 400;
            this.Type = LayoutType.ConnectedRectangularRooms;
            this.RoomPlacementType = LayoutRoomPlacementType.RectangularGrid;
            this.ConnectionType = LayoutConnectionType.CorridorWithDoors;
            this.Level = new Range<int>(1, 100);
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.NumberExtraWallRemovals = 200;
            this.HiddenDoorProbability = 0.2;
            this.GenerationRate = 0.5;

            this.WallSymbol = new SymbolDetailsTemplate();
            this.DoorSymbol = new SymbolDetailsTemplate();
        }

        /// <summary>
        /// Calculates a simulated number of steps that the player will use to traverse the layout
        /// </summary>
        public int GetPathLength()
        {
            switch (this.Type)
            {
                default:
                    throw new Exception("Unknown Layout Type - LayoutTemplate.GetPathLength");

                case LayoutType.Maze:
                case LayoutType.ConnectedCellularAutomata:
                    // Measure = Made up :) 
                    return 4 * this.Width * this.Height;

                case LayoutType.ConnectedRectangularRooms:
                    // Measure  = # of traversals * length of traversal for
                    //            a single pass only * 1;
                    return this.Width * this.Height * 1;
            }
        }
    }
}