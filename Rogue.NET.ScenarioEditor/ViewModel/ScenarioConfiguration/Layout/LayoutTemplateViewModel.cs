using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
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
        private string _wallColor;
        private string _doorColor;

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
        public string WallColor
        {
            get { return _wallColor; }
            set { this.RaiseAndSetIfChanged(ref _wallColor, value); }
        }
        public string DoorColor
        {
            get { return _doorColor; }
            set { this.RaiseAndSetIfChanged(ref _doorColor, value); }
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
        }
    }
}