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
        private int _width;
        private int _height;
        private int _numberRoomRows;
        private int _numberRoomCols;
        private RangeViewModel<int> _regionWidthRange;
        private RangeViewModel<int> _regionHeightRange;
        private int _rectangularGridPadding;
        private int _randomRoomCount;
        private int _randomRoomSpread;
        private bool _makeSymmetric;
        private double _mazeHorizontalVerticalBias;
        private double _mazeWallRemovalRatio;
        private double _openWorldElevationFrequency;
        private RangeViewModel<double> _openWorldElevationRegionRange;
        private double _hiddenDoorProbability;
        private double _cellularAutomataFillRatio;
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

        public int Width
        {
            get { return _width; }
            set { this.RaiseAndSetIfChanged(ref _width, value); }
        }
        public int Height
        {
            get { return _height; }
            set { this.RaiseAndSetIfChanged(ref _height, value); }
        }
        public RangeViewModel<int> RegionWidthRange
        {
            get { return _regionWidthRange; }
            set { this.RaiseAndSetIfChanged(ref _regionWidthRange, value); }
        }
        public RangeViewModel<int> RegionHeightRange
        {
            get { return _regionHeightRange; }
            set { this.RaiseAndSetIfChanged(ref _regionHeightRange, value); }
        }
        public int NumberRoomRows
        {
            get { return _numberRoomRows; }
            set { this.RaiseAndSetIfChanged(ref _numberRoomRows, value); }
        }
        public int NumberRoomCols
        {
            get { return _numberRoomCols; }
            set { this.RaiseAndSetIfChanged(ref _numberRoomCols, value); }
        }
        public int RectangularGridPadding
        {
            get { return _rectangularGridPadding; }
            set { this.RaiseAndSetIfChanged(ref _rectangularGridPadding, value); }
        }
        public int RandomRoomCount
        {
            get { return _randomRoomCount; }
            set { this.RaiseAndSetIfChanged(ref _randomRoomCount, value); }
        }
        public int RandomRoomSpread
        {
            get { return _randomRoomSpread; }
            set { this.RaiseAndSetIfChanged(ref _randomRoomSpread, value); }
        }
        public bool MakeSymmetric
        {
            get { return _makeSymmetric; }
            set { this.RaiseAndSetIfChanged(ref _makeSymmetric, value); }
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
        public double OpenWorldElevationFrequency
        {
            get { return _openWorldElevationFrequency; }
            set { this.RaiseAndSetIfChanged(ref _openWorldElevationFrequency, value); }
        }
        public RangeViewModel<double> OpenWorldElevationRegionRange
        {
            get { return _openWorldElevationRegionRange; }
            set { this.RaiseAndSetIfChanged(ref _openWorldElevationRegionRange, value); }
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
            this.Width = 80;
            this.Height = 50;
            this.RegionHeightRange = new RangeViewModel<int>();
            this.RegionWidthRange = new RangeViewModel<int>();
            this.Type = LayoutType.RectangularRegion;
            this.CellularAutomataType = LayoutCellularAutomataType.Open;
            this.ConnectionType = LayoutConnectionType.Corridor;
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.HiddenDoorProbability = 0.2;

            this.MazeHorizontalVerticalBias = 0.5;
            this.MazeWallRemovalRatio = 0.5;
            this.OpenWorldElevationFrequency = 0.1;
            this.OpenWorldElevationRegionRange = new RangeViewModel<double>(0.5, 1);

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