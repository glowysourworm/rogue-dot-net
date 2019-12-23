using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;
using System.Windows.Media;
using LayoutViewType = Rogue.NET.ScenarioEditor.Views.Assets.Layout;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    [UIType(DisplayName = "Layout",
            Description = "Specification for the layout of a level of the game",
            ViewType = typeof(LayoutViewType),
            BaseType = UITypeAttributeBaseType.Asset)]
    public class LayoutTemplateViewModel : TemplateViewModel
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
        private SymbolDetailsTemplateViewModel _wallSymbol;
        private SymbolDetailsTemplateViewModel _doorSymbol;
        private SymbolDetailsTemplateViewModel _cellSymbol;
        private LightAmbientTemplateViewModel _lightingAmbient1;
        private LightAmbientTemplateViewModel _lightingAmbient2;
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
        public SymbolDetailsTemplateViewModel WallSymbol
        {
            get { return _wallSymbol; }
            set { this.RaiseAndSetIfChanged(ref _wallSymbol, value); }
        }
        public SymbolDetailsTemplateViewModel DoorSymbol
        {
            get { return _doorSymbol; }
            set { this.RaiseAndSetIfChanged(ref _doorSymbol, value); }
        }
        public SymbolDetailsTemplateViewModel CellSymbol
        {
            get { return _cellSymbol; }
            set { this.RaiseAndSetIfChanged(ref _cellSymbol, value); }
        }
        public LightAmbientTemplateViewModel LightingAmbient1
        {
            get { return _lightingAmbient1; }
            set { this.RaiseAndSetIfChanged(ref _lightingAmbient1, value); }
        }
        public LightAmbientTemplateViewModel LightingAmbient2
        {
            get { return _lightingAmbient2; }
            set { this.RaiseAndSetIfChanged(ref _lightingAmbient2, value); }
        }
        public double LightingThreshold
        {
            get { return _lightingThreshold; }
            set { this.RaiseAndSetIfChanged(ref _lightingThreshold, value); }
        }

        public ObservableCollection<TerrainLayerGenerationTemplateViewModel> TerrainLayers { get; set; }

        public LayoutTemplateViewModel() : base()
        {
            this.Type = LayoutType.RectangularRegion;
            this.CellularAutomataType = LayoutCellularAutomataType.Open;
            this.ConnectionType = LayoutConnectionType.Corridor;
            this.HiddenDoorProbability = 0.2;

            this.MazeHorizontalVerticalBias = 0.5;
            this.MazeWallRemovalRatio = 0.5;

            this.WallSymbol = new SymbolDetailsTemplateViewModel();
            this.DoorSymbol = new SymbolDetailsTemplateViewModel();
            this.CellSymbol = new SymbolDetailsTemplateViewModel();

            this.LightingAmbient1 = new LightAmbientTemplateViewModel();
            this.LightingAmbient2 = new LightAmbientTemplateViewModel();
            this.LightingThreshold = 1.0;

            this.TerrainLayers = new ObservableCollection<TerrainLayerGenerationTemplateViewModel>();
        }
    }
}