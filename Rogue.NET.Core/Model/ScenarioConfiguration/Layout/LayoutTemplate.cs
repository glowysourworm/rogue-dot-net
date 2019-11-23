using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    public class LayoutTemplate : Template
    {
        private int _width;
        private int _height;
        private Range<int> _regionWidthRange;
        private Range<int> _regionHeightRange;
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _rectangularGridPadding;
        private int _randomRoomCount;
        private int _randomRoomSpread;
        private double _mazeHorizontalVerticalBias;
        private double _mazeWallRemovalRatio;
        private double _openWorldElevationFrequency;
        private Range<double> _openWorldElevationRegionRange;
        private double _hiddenDoorProbability;
        private double _cellularAutomataFillRatio;
        private bool _makeSymmetric;
        private LayoutType _type;
        private LayoutCellularAutomataType _cellularAutomataType;
        private LayoutConnectionType _connectionType;
        private LayoutSymmetryType _symmetryType;
        private SymbolDetailsTemplate _wallSymbol;
        private SymbolDetailsTemplate _doorSymbol;
        private SymbolDetailsTemplate _cellSymbol;
        private LightAmbientTemplate _lightingAmbient1;
        private LightAmbientTemplate _lightingAmbient2;
        private double _lightingThreshold;

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
        public Range<int> RegionWidthRange
        {
            get { return _regionWidthRange; }
            set
            {
                if (_regionWidthRange != value)
                {
                    _regionWidthRange = value;
                    OnPropertyChanged("RegionWidthRange");
                }
            }
        }
        public Range<int> RegionHeightRange
        {
            get { return _regionHeightRange; }
            set
            {
                if (_regionHeightRange != value)
                {
                    _regionHeightRange = value;
                    OnPropertyChanged("RegionHeightRange");
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
        public bool MakeSymmetric
        {
            get { return _makeSymmetric; }
            set
            {
                if (_makeSymmetric != value)
                {
                    _makeSymmetric = value;
                    OnPropertyChanged("MakeSymmetric");
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
        public double MazeHorizontalVerticalBias
        {
            get { return _mazeHorizontalVerticalBias; }
            set
            {
                if (_mazeHorizontalVerticalBias != value)
                {
                    _mazeHorizontalVerticalBias = value;
                    OnPropertyChanged("MazeHorizontalVerticalBias");
                }
            }
        }
        public double MazeWallRemovalRatio
        {
            get { return _mazeWallRemovalRatio; }
            set
            {
                if (_mazeWallRemovalRatio != value)
                {
                    _mazeWallRemovalRatio = value;
                    OnPropertyChanged("MazeWallRemovalRatio");
                }
            }
        }
        public double OpenWorldElevationFrequency
        {
            get { return _openWorldElevationFrequency; }
            set
            {
                if (_openWorldElevationFrequency != value)
                {
                    _openWorldElevationFrequency = value;
                    OnPropertyChanged("OpenWorldElevationFrequency");
                }
            }
        }
        public Range<double> OpenWorldElevationRegionRange
        {
            get { return _openWorldElevationRegionRange; }
            set
            {
                if (_openWorldElevationRegionRange != value)
                {
                    _openWorldElevationRegionRange = value;
                    OnPropertyChanged("OpenWorldElevationRegionRange");
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
        public LayoutSymmetryType SymmetryType
        {
            get { return _symmetryType; }
            set
            {
                if (_symmetryType != value)
                {
                    _symmetryType = value;
                    OnPropertyChanged("SymmetryType");
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
        public SymbolDetailsTemplate CellSymbol
        {
            get { return _cellSymbol; }
            set
            {
                if (_cellSymbol != value)
                {
                    _cellSymbol = value;
                    OnPropertyChanged("CellSymbol");
                }
            }
        }
        public LightAmbientTemplate LightingAmbient1
        {
            get { return _lightingAmbient1; }
            set
            {
                if (_lightingAmbient1 != value)
                {
                    _lightingAmbient1 = value;
                    OnPropertyChanged("LightingAmbient1");
                }
            }
        }
        public LightAmbientTemplate LightingAmbient2
        {
            get { return _lightingAmbient2; }
            set
            {
                if (_lightingAmbient2 != value)
                {
                    _lightingAmbient2 = value;
                    OnPropertyChanged("LightingAmbient2");
                }
            }
        }
        public double LightingThreshold
        {
            get { return _lightingThreshold; }
            set
            {
                if (_lightingThreshold != value)
                {
                    _lightingThreshold = value;
                    OnPropertyChanged("LightingThreshold");
                }
            }
        }

        public List<TerrainLayerGenerationTemplate> TerrainLayers { get; set; }

        public LayoutTemplate() : base()
        {
            this.Width = 600;
            this.Height = 400;
            this.RegionHeightRange = new Range<int>();
            this.RegionWidthRange = new Range<int>();
            this.Type = LayoutType.RectangularRegion;
            this.ConnectionType = LayoutConnectionType.Corridor;
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.HiddenDoorProbability = 0.2;

            this.MazeHorizontalVerticalBias = 0.5;
            this.MazeWallRemovalRatio = 0.5;
            this.OpenWorldElevationFrequency = 0.1;
            this.OpenWorldElevationRegionRange = new Range<double>(0.5, 1);

            this.WallSymbol = new SymbolDetailsTemplate();
            this.DoorSymbol = new SymbolDetailsTemplate();
            this.CellSymbol = new SymbolDetailsTemplate();

            this.LightingAmbient1 = new LightAmbientTemplate();
            this.LightingAmbient2 = new LightAmbientTemplate();
            this.LightingThreshold = 1.0;

            this.TerrainLayers = new List<TerrainLayerGenerationTemplate>();
        }
    }
}