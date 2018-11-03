using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [Export(typeof(LevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _resourceService;

        ObservableCollection<FrameworkElement> _drawings;

        [ImportingConstructor]
        public LevelCanvasViewModel(
            IScenarioResourceService resourceService, 
            IEventAggregator eventAggregator, 
            IModelService modelService)
        {
            _resourceService = resourceService;

            this.Drawings = new ObservableCollection<FrameworkElement>();

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                DrawLevel(modelService.CurrentLevel, modelService.Player);
            }, true);
        }

        public ObservableCollection<FrameworkElement> Drawings
        {
            get { return _drawings; }
            set
            {
                _drawings = value;
                OnPropertyChanged("Drawings");
            }
        }

        // TODO: move this
        private void DrawLevel(Level level, Player player)
        {
            this.Drawings.Clear();

            var geometry = new StreamGeometry();
            using (var stream = geometry.Open())
            {
                foreach (var cell in level.Grid.GetCells())
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

            foreach (var scenarioObject in level.GetContents())
            {
                var image = new Image();
                image.Source = _resourceService.GetImageSource(scenarioObject);

                if (scenarioObject is DoodadBase)
                    Canvas.SetZIndex(image, 1);

                else if (scenarioObject is ItemBase)
                    Canvas.SetZIndex(image, 2);

                else if (scenarioObject is Character)
                    Canvas.SetZIndex(image, 3);

                var point = DataHelper.Cell2UI(scenarioObject.Location);

                Canvas.SetLeft(image, point.X);
                Canvas.SetTop(image, point.Y);

                this.Drawings.Add(image);
            }

            var playerImage = new Image();
            var playerLocation = DataHelper.Cell2UI(player.Location);
            playerImage.Source = _resourceService.GetImageSource(player);

            Canvas.SetLeft(playerImage, playerLocation.X);
            Canvas.SetTop(playerImage, playerLocation.Y);
            Canvas.SetZIndex(playerImage, 3);

            this.Drawings.Add(playerImage);
        }
    }
}
