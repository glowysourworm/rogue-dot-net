using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class LayoutTemplateViewModel : TemplateViewModel
    {
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _roomDivCellHeight;
        private int _roomDivCellWidth;
        private int _numberExtraWallRemovals;
        private double _hiddenDoorProbability;
        private double _generationRatio;
        private LayoutType _type;
        private RangeViewModel<int> _levelRange;

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
        public int RoomDivCellHeight
        {
            get { return _roomDivCellHeight; }
            set { this.RaiseAndSetIfChanged(ref _roomDivCellHeight, value); }
        }
        public int RoomDivCellWidth
        {
            get { return _roomDivCellWidth; }
            set { this.RaiseAndSetIfChanged(ref _roomDivCellWidth, value); }
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
        public LayoutType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public RangeViewModel<int> Level
        {
            get { return _levelRange; }
            set { this.RaiseAndSetIfChanged(ref _levelRange, value); }
        }

        public LayoutTemplateViewModel() : base()
        {
            this.Type = LayoutType.Normal;
            this.Level = new RangeViewModel<int>(1, 1, 100, 100);
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