using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [Export(typeof(LevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel
    {
        ObservableCollection<Path> _drawings;


        public ObservableCollection<Path> Drawings
        {
            get { return _drawings; }
            set
            {
                _drawings = value;
                OnPropertyChanged("Drawings");
            }
        }

        [ImportingConstructor]
        public LevelCanvasViewModel(IEventAggregator eventAggregator, IModelService modelService)
        {
            this.Drawings = new ObservableCollection<Path>();

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                DrawLevelGrid(modelService.CurrentLevel.Grid);
            }, true);
        }

        // TODO: move this
        private void DrawLevelGrid(LevelGrid grid)
        {
            this.Drawings.Clear();

            var geometry = new StreamGeometry();
            using (var stream = geometry.Open())
            {
                foreach (var cell in grid.GetCells())
                {
                    var rect = DataHelper.Cell2UIRect(cell.Location, false);
                    stream.BeginFigure(rect.TopLeft, false, false);
                    stream.LineTo(rect.TopRight, (cell.Walls & Compass.N) != 0 || (cell.VisibleDoors & Compass.N) != 0, true);
                    stream.LineTo(rect.BottomRight, (cell.Walls & Compass.E) != 0 || (cell.VisibleDoors & Compass.E) != 0, true);
                    stream.LineTo(rect.BottomLeft, (cell.Walls & Compass.S) != 0 || (cell.VisibleDoors & Compass.S) != 0, true);
                    stream.LineTo(rect.TopLeft, (cell.Walls & Compass.W) != 0 || (cell.VisibleDoors & Compass.W) != 0, true);
                }
            }

            var path = new Path();
            path.Data = geometry;
            path.Fill = Brushes.LightBlue;
            path.Stroke = Brushes.Blue;
            path.StrokeThickness = 2;

            this.Drawings.Add(path);
        }
    }
}
