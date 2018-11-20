using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.Events.Content;
using Rogue.NET.Scenario.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class CompassCtrl : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        List<Rectangle> _canvasPoints = new List<Rectangle>();
        List<Line> _canvasLines = new List<Line>();
        int currentLevelNumber = -1;

        [ImportingConstructor]
        public CompassCtrl(IEventAggregator eventAggregator, IScenarioUIGeometryService scenarioUIGeometryService, IModelService modelService, PlayerViewModel playerViewModel)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = playerViewModel;

            InitializeComponent();

            // subscribe to events
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                Update(modelService, scenarioUIGeometryService);
            }, ThreadOption.UIThread, true);

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                if (update.LevelUpdateType == LevelUpdateType.PlayerLocation)
                    Update(modelService, scenarioUIGeometryService);
            }, ThreadOption.UIThread, true);
        }

        private void Update(IModelService modelService, IScenarioUIGeometryService scenarioUIGeometryService)
        {
            foreach (var rectangle in _canvasPoints)
                this.GlobeCanvas.Children.Remove(rectangle);

            foreach (var content in modelService.Level.GetContents().Where(z => z.IsExplored))
            {
                var contentMarker = new Rectangle();
                contentMarker.StrokeThickness = 0;
                contentMarker.Fill = content is Enemy ? Brushes.Red : Brushes.LightBlue;
                contentMarker.Fill = content is ItemBase ? Brushes.YellowGreen : contentMarker.Fill;

                var levelUIBounds = scenarioUIGeometryService.Cell2UIRect(modelService.Level.Grid.GetBounds());

                var xneg = false;
                var yneg = false;
                Point compassLocation = TranslatePoint(
                    scenarioUIGeometryService.Cell2UI(modelService.Player.Location),
                    scenarioUIGeometryService.Cell2UI(content.Location),
                    levelUIBounds.Width,
                    levelUIBounds.Height,
                    out xneg,
                    out yneg);

                double maxWidth = 4;
                double dist = Math.Sqrt(Math.Pow(compassLocation.X, 2) + Math.Pow(compassLocation.Y, 2));
                contentMarker.Width = (((1 - maxWidth) / 50) * dist) + maxWidth;
                contentMarker.Height = contentMarker.Width;

                if (yneg)
                    Canvas.SetTop(contentMarker, 50 - compassLocation.Y);
                else
                    Canvas.SetTop(contentMarker, compassLocation.Y + 48);

                if (xneg)
                    Canvas.SetLeft(contentMarker, 50 - compassLocation.X);
                else
                    Canvas.SetLeft(contentMarker, compassLocation.X + 48);

                _canvasPoints.Add(contentMarker);
                this.GlobeCanvas.Children.Add(contentMarker);
            }
        }

        private void DrawLevelLines(Level level)
        {
            //currentLevelNumber = level.Number;

            //double radius = 50;
            //double max = Math.Max(level.RenderSize.Width * ModelConstants.CELLWIDTH, level.RenderSize.Height * ModelConstants.CELLHEIGHT);
            //double c = ((radius / Math.Sqrt(2)) / Math.Log(max + 1));
            //double stepDist = (level.RenderSize.Height == max) ? ModelConstants.CELLHEIGHT : ModelConstants.CELLWIDTH;

            //foreach (Line l in _canvasLines)
            //    this.GlobeCanvas.Children.Remove(l);

            //for (int i = 0; i <= 3; i++)
            //{
            //    Line xLine1 = new Line();
            //    double x = c * Math.Log(i * stepDist + 1);
            //    double dy = Math.Sqrt((radius * radius) - Math.Pow(x, 2));

            //    xLine1.X1 = 50 + x;
            //    xLine1.X2 = 50 + x;
            //    xLine1.Y1 = 50 - dy;
            //    xLine1.Y2 = 50 + dy;

            //    Line yLine1 = new Line();
            //    yLine1.Y1 = 50 + x;
            //    yLine1.Y2 = 50 + x;
            //    yLine1.X1 = 50 - dy;
            //    yLine1.X2 = 50 + dy;

            //    Line xLine2 = new Line();
            //    xLine2.X1 = 50 - x;
            //    xLine2.X2 = 50 - x;
            //    xLine2.Y1 = 50 - dy;
            //    xLine2.Y2 = 50 + dy;

            //    Line yLine2 = new Line();
            //    yLine2.Y1 = 50 - x;
            //    yLine2.Y2 = 50 - x;
            //    yLine2.X1 = 50 - dy;
            //    yLine2.X2 = 50 + dy;

            //    double opacity = ((-1 / 50.0D) * (i * 10)) + 1;

            //    xLine1.Stroke = Brushes.White;
            //    xLine1.StrokeThickness = 0.5;
            //    xLine1.Opacity = opacity;

            //    xLine2.Stroke = Brushes.White;
            //    xLine2.StrokeThickness = 0.5;
            //    xLine2.Opacity = opacity;

            //    yLine1.Stroke = Brushes.White;
            //    yLine1.StrokeThickness = 0.5;
            //    yLine1.Opacity = opacity;

            //    yLine2.Stroke = Brushes.White;
            //    yLine2.StrokeThickness = 0.5;
            //    yLine2.Opacity = opacity;

            //    _canvasLines.Add(xLine1);
            //    _canvasLines.Add(xLine2);
            //    _canvasLines.Add(yLine1);
            //    _canvasLines.Add(yLine2);

            //    this.GlobeCanvas.Children.Add(xLine1);
            //    this.GlobeCanvas.Children.Add(xLine2);
            //    this.GlobeCanvas.Children.Add(yLine1);
            //    this.GlobeCanvas.Children.Add(yLine2);
            //}
        }

        //Translates point to point on globe canvas
        private Point TranslatePoint(Point map1, Point map2, double maxDistX, double maxDistY, out bool negativeX, out bool negativeY)
        {
            double radius = 50;                                                         //See radius of globe ellipse
            double max = Math.Max(maxDistX, maxDistY);                                  //circumscribed square
            double c = ((radius / Math.Sqrt(2)) / Math.Log(max + 1));                   //Constant for natural log mapping functin f = c * ln (x + 1)
            double xTrans = c * Math.Log(Math.Abs(map2.X - map1.X) + 1);                //dx
            double yTrans = c * Math.Log(Math.Abs(map2.Y - map1.Y) + 1);                //dy

            //Distances are now translated to a quarter of the sphere - return bools to specify the quadrant
            negativeX = map2.X < map1.X;
            negativeY = map2.Y < map1.Y;
            return new Point(Math.Min(xTrans, 50), Math.Min(yTrans, 50));
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Up);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Down);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Left);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Right);
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.CenterOnPlayer);
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GlobeCanvas.Visibility == Visibility.Collapsed)
            {
                this.GlobeCanvas.Visibility = Visibility.Visible;
                this.WidgetBorder.Visibility = Visibility.Visible;
            }
            else
            {
                this.WidgetBorder.Visibility = Visibility.Collapsed;
                this.GlobeCanvas.Visibility = Visibility.Collapsed;
            }
        }
    }
}
