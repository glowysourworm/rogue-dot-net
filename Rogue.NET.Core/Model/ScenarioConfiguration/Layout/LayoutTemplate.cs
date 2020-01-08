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
        private double _widthRatio;
        private double _heightRatio;
        private double _roomColumnRatio;
        private double _roomRowRatio;
        private double _fillRatioRooms;
        private double _fillRatioCorridors;
        private double _roomSize;
        private double _roomSizeErradicity;
        private double _randomRoomSpacing;
        private double _randomRoomSeparationRatio;
        private double _mazeHorizontalVerticalBias;
        private double _mazeWallRemovalRatio;
        private double _elevationFrequency;
        private double _elevationSelector;
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
        private LightAmbientTemplate _accentLighting;
        private LightTemplate _wallLight;
        private double _wallLightIntensity;
        private double _wallLightFillRatio;
        private bool _hasWallLights;
        private double _lightingThreshold;

        public double WidthRatio
        {
            get { return _widthRatio; }
            set
            {
                if (_widthRatio != value)
                {
                    _widthRatio = value;
                    OnPropertyChanged("WidthRatio");
                }
            }
        }
        public double HeightRatio
        {
            get { return _heightRatio; }
            set
            {
                if (_heightRatio != value)
                {
                    _heightRatio = value;
                    OnPropertyChanged("HeightRatio");
                }
            }
        }
        public double RoomColumnRatio
        {
            get { return _roomColumnRatio; }
            set
            {
                if (_roomColumnRatio != value)
                {
                    _roomColumnRatio = value;
                    OnPropertyChanged("RoomColumnRatio");
                }
            }
        }
        public double RoomRowRatio
        {
            get { return _roomRowRatio; }
            set
            {
                if (_roomRowRatio != value)
                {
                    _roomRowRatio = value;
                    OnPropertyChanged("RoomRowRatio");
                }
            }
        }
        public double FillRatioRooms
        {
            get { return _fillRatioRooms; }
            set
            {
                if (_fillRatioRooms != value)
                {
                    _fillRatioRooms = value;
                    OnPropertyChanged("FillRatioRooms");
                }
            }
        }
        public double FillRatioCorridors
        {
            get { return _fillRatioCorridors; }
            set
            {
                if (_fillRatioCorridors != value)
                {
                    _fillRatioCorridors = value;
                    OnPropertyChanged("FillRatioCorridors");
                }
            }
        }
        public double RoomSize
        {
            get { return _roomSize; }
            set
            {
                if (_roomSize != value)
                {
                    _roomSize = value;
                    OnPropertyChanged("RoomSize");
                }
            }
        }
        public double RoomSizeErradicity
        {
            get { return _roomSizeErradicity; }
            set
            {
                if (_roomSizeErradicity != value)
                {
                    _roomSizeErradicity = value;
                    OnPropertyChanged("RoomSizeErradicity");
                }
            }
        }
        public double RandomRoomSpacing
        {
            get { return _randomRoomSpacing; }
            set
            {
                if (_randomRoomSpacing != value)
                {
                    _randomRoomSpacing = value;
                    OnPropertyChanged("RandomRoomSpacing");
                }
            }
        }
        public double RandomRoomSeparationRatio
        {
            get { return _randomRoomSeparationRatio; }
            set
            {
                if (_randomRoomSeparationRatio != value)
                {
                    _randomRoomSeparationRatio = value;
                    OnPropertyChanged("RandomRoomSeparationRatio");
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
        public double ElevationFrequency
        {
            get { return _elevationFrequency; }
            set
            {
                if (_elevationFrequency != value)
                {
                    _elevationFrequency = value;
                    OnPropertyChanged("ElevationFrequency");
                }
            }
        }
        public double ElevationSelector
        {
            get { return _elevationSelector; }
            set
            {
                if (_elevationSelector != value)
                {
                    _elevationSelector = value;
                    OnPropertyChanged("ElevationSelector");
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
        public LightAmbientTemplate AccentLighting
        {
            get { return _accentLighting; }
            set { this.RaiseAndSetIfChanged(ref _accentLighting, value); }
        }
        public LightTemplate WallLight
        {
            get { return _wallLight; }
            set { this.RaiseAndSetIfChanged(ref _wallLight, value); }
        }
        public double WallLightIntensity
        {
            get { return _wallLightIntensity; }
            set { this.RaiseAndSetIfChanged(ref _wallLightIntensity, value); }
        }
        public double WallLightFillRatio
        {
            get { return _wallLightFillRatio; }
            set { this.RaiseAndSetIfChanged(ref _wallLightFillRatio, value); }
        }
        public bool HasWallLights
        {
            get { return _hasWallLights; }
            set { this.RaiseAndSetIfChanged(ref _hasWallLights, value); }
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
            this.Type = LayoutType.RectangularRegion;
            this.ConnectionType = LayoutConnectionType.Corridor;
            this.HiddenDoorProbability = 0.2;

            this.MazeHorizontalVerticalBias = 0.5;
            this.MazeWallRemovalRatio = 0.5;

            this.WallSymbol = new SymbolDetailsTemplate();
            this.DoorSymbol = new SymbolDetailsTemplate();
            this.CellSymbol = new SymbolDetailsTemplate();

            this.LightingThreshold = 1.0;
            this.AccentLighting = new LightAmbientTemplate();
            this.WallLight = new LightTemplate();
            this.HasWallLights = false;

            this.TerrainLayers = new List<TerrainLayerGenerationTemplate>();
        }
    }
}