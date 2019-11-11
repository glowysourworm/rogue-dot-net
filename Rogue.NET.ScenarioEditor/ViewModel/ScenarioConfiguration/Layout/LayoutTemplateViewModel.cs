using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
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
        private int _roomHeightLimit;
        private int _roomWidthLimit;
        private int _roomHeightMin;
        private int _roomWidthMin;
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
        private RangeViewModel<int> _levelRange;
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
        public int RoomWidthLimit
        {
            get { return _roomWidthLimit; }
            set { this.RaiseAndSetIfChanged(ref _roomWidthLimit, value); }
        }
        public int RoomHeightLimit
        {
            get { return _roomHeightLimit; }
            set { this.RaiseAndSetIfChanged(ref _roomHeightLimit, value); }
        }
        public int RoomWidthMin
        {
            get { return _roomWidthMin; }
            set { this.RaiseAndSetIfChanged(ref _roomWidthMin, value); }
        }
        public int RoomHeightMin
        {
            get { return _roomHeightMin; }
            set { this.RaiseAndSetIfChanged(ref _roomHeightMin, value); }
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
        public int NumberExtraWallRemovals
        {
            get { return _numberExtraWallRemovals; }
            set { this.RaiseAndSetIfChanged(ref _numberExtraWallRemovals, value); }
        }
        public double HiddenDoorProbability
        {
            get { return _hiddenDoorProbability; }
            set { this.RaiseAndSetIfChanged(ref _hiddenDoorProbability, value); }
        }
        public double GenerationRate
        {
            get { return _generationRatio; }
            set { this.RaiseAndSetIfChanged(ref _generationRatio, value); }
        }
        public double CellularAutomataFillRatio
        {
            get { return _cellularAutomataFillRatio; }
            set { this.RaiseAndSetIfChanged(ref _cellularAutomataFillRatio, value); }
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
        public LayoutRoomPlacementType RoomPlacementType
        {
            get { return _roomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _roomPlacementType, value); }
        }
        public LayoutConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { this.RaiseAndSetIfChanged(ref _connectionType, value); }
        }
        public LayoutConnectionGeometryType ConnectionGeometryType
        {
            get { return _connectionGeometryType; }
            set { this.RaiseAndSetIfChanged(ref _connectionGeometryType, value); }
        }
        public LayoutCorridorGeometryType CorridorGeometryType
        {
            get { return _corridorGeometryType; }
            set { this.RaiseAndSetIfChanged(ref _corridorGeometryType, value); }
        }
        public RangeViewModel<int> Level
        {
            get { return _levelRange; }
            set { this.RaiseAndSetIfChanged(ref _levelRange, value); }
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

        public LayoutTemplateViewModel() : base()
        {
            this.Width = 80;
            this.Height = 50;
            this.Type = LayoutType.ConnectedRectangularRooms;
            this.CellularAutomataType = LayoutCellularAutomataType.Open;
            this.RoomPlacementType = LayoutRoomPlacementType.RectangularGrid;
            this.ConnectionType = LayoutConnectionType.CorridorWithDoors;
            this.ConnectionGeometryType = LayoutConnectionGeometryType.Rectilinear;
            this.CorridorGeometryType = LayoutCorridorGeometryType.Linear;
            this.Level = new RangeViewModel<int>(1, 100);
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.NumberExtraWallRemovals = 200;
            this.HiddenDoorProbability = 0.2;
            this.GenerationRate = 0.5;

            this.WallSymbol = new SymbolDetailsTemplateViewModel();
            this.DoorSymbol = new SymbolDetailsTemplateViewModel();
            this.CellSymbol = new SymbolDetailsTemplateViewModel();

            this.LightingAmbient1 = new LightAmbientTemplateViewModel();
            this.LightingAmbient2 = new LightAmbientTemplateViewModel();
            this.LightingThreshold = 1.0;
        }
    }
}