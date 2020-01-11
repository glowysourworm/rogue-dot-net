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
		private SymbolDetailsTemplate _wallLightSymbol;
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
			set { this.RaiseAndSetIfChanged(ref _widthRatio, value); }
		}
		public double HeightRatio
		{
			get { return _heightRatio; }
			set { this.RaiseAndSetIfChanged(ref _heightRatio, value); }
		}
		public double RoomColumnRatio
		{
			get { return _roomColumnRatio; }
			set { this.RaiseAndSetIfChanged(ref _roomColumnRatio, value); }
		}
		public double RoomRowRatio
		{
			get { return _roomRowRatio; }
			set { this.RaiseAndSetIfChanged(ref _roomRowRatio, value); }
		}
		public double FillRatioRooms
		{
			get { return _fillRatioRooms; }
			set { this.RaiseAndSetIfChanged(ref _fillRatioRooms, value); }
		}
		public double FillRatioCorridors
		{
			get { return _fillRatioCorridors; }
			set { this.RaiseAndSetIfChanged(ref _fillRatioCorridors, value); }
		}
		public double RoomSize
		{
			get { return _roomSize; }
			set { this.RaiseAndSetIfChanged(ref _roomSize, value); }
		}
		public double RoomSizeErradicity
		{
			get { return _roomSizeErradicity; }
			set { this.RaiseAndSetIfChanged(ref _roomSizeErradicity, value); }
		}
		public double RandomRoomSpacing
		{
			get { return _randomRoomSpacing; }
			set { this.RaiseAndSetIfChanged(ref _randomRoomSpacing, value); }
		}
		public double RandomRoomSeparationRatio
		{
			get { return _randomRoomSeparationRatio; }
			set { this.RaiseAndSetIfChanged(ref _randomRoomSeparationRatio, value); }
		}
		public double MazeHorizontalVerticalBias
		{
			get { return _mazeHorizontalVerticalBias; }
			set { this.RaiseAndSetIfChanged(ref _mazeHorizontalVerticalBias, value); }
		}
		public double MazeWallRemovalRatio
		{
			get { return _mazeWallRemovalRatio; }
			set { this.RaiseAndSetIfChanged(ref _mazeWallRemovalRatio, value); }
		}
		public double ElevationFrequency
		{
			get { return _elevationFrequency; }
			set { this.RaiseAndSetIfChanged(ref _elevationFrequency, value); }
		}
		public double ElevationSelector
		{
			get { return _elevationSelector; }
			set { this.RaiseAndSetIfChanged(ref _elevationSelector, value); }
		}
		public double HiddenDoorProbability
		{
			get { return _hiddenDoorProbability; }
			set { this.RaiseAndSetIfChanged(ref _hiddenDoorProbability, value); }
		}
		public double CellularAutomataFillRatio
		{
			get { return _cellularAutomataFillRatio; }
			set { this.RaiseAndSetIfChanged(ref _cellularAutomataFillRatio, value); }
		}
		public bool MakeSymmetric
		{
			get { return _makeSymmetric; }
			set { this.RaiseAndSetIfChanged(ref _makeSymmetric, value); }
		}
		public LayoutType Type
		{
			get { return _type; }
			set { this.RaiseAndSetIfChanged(ref _type, value); }
		}
		public LayoutCellularAutomataType CellularAutomataType
		{
			get { return _cellularAutomataType; }
			set { this.RaiseAndSetIfChanged(ref _cellularAutomataType, value); }
		}
		public LayoutConnectionType ConnectionType
		{
			get { return _connectionType; }
			set { this.RaiseAndSetIfChanged(ref _connectionType, value); }
		}
		public LayoutSymmetryType SymmetryType
		{
			get { return _symmetryType; }
			set { this.RaiseAndSetIfChanged(ref _symmetryType, value); }
		}
		public SymbolDetailsTemplate WallSymbol
		{
			get { return _wallSymbol; }
			set { this.RaiseAndSetIfChanged(ref _wallSymbol, value); }
		}
		public SymbolDetailsTemplate WallLightSymbol
		{
			get { return _wallLightSymbol; }
			set { this.RaiseAndSetIfChanged(ref _wallLightSymbol, value); }
		}
		public SymbolDetailsTemplate DoorSymbol
		{
			get { return _doorSymbol; }
			set { this.RaiseAndSetIfChanged(ref _doorSymbol, value); }
		}
		public SymbolDetailsTemplate CellSymbol
		{
			get { return _cellSymbol; }
			set { this.RaiseAndSetIfChanged(ref _cellSymbol, value); }
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
			set { this.RaiseAndSetIfChanged(ref _lightingThreshold, value); }
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
			this.WallLightSymbol = new SymbolDetailsTemplate();
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