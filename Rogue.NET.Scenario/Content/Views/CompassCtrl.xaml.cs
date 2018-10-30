using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario;
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
        bool _eventsHooked = false;

        List<Rectangle> _canvasPoints = new List<Rectangle>();
        List<Line> _canvasLines = new List<Line>();
        int currentLevelNumber = -1;

        [ImportingConstructor]
        public CompassCtrl(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
            InitializeEvents();
        }

        public void InitializeEvents()
        {
            if (_eventsHooked)
                return;

            _eventsHooked = true;

            // subscribe to events
            _eventAggregator.GetEvent<UserCommandEvent>().Subscribe((e) =>
            {
                var data = this.DataContext as LevelData;
                if (data != null)
                    Update(data);
            });
        }

        private void Update(LevelData data)
        {
            //if (data.Level.Number != currentLevelNumber)
            //    DrawLevelLines(data.Level);

            foreach (Rectangle r in _canvasPoints)
                this.GlobeCanvas.Children.Remove(r);

            //foreach (var o in data.Level.GetEverything().Where(z => z.IsExplored || (z.Visibility == System.Windows.Visibility.Visible)))
            //{
            //    Rectangle r = new Rectangle();
            //    r.StrokeThickness = 0;
            //    r.Fill = o is Enemy ? Brushes.Red : Brushes.LightBlue;
            //    r.Fill = o is Item ? Brushes.YellowGreen : r.Fill;

            //    bool xneg = false;
            //    bool yneg = false;
            //    Point p = TranslatePoint(
            //        data.Player.Location.ToPoint(),
            //        o.Location.ToPoint(),
            //        data.Level.RenderSize.Width * ScenarioConfiguration.CELLWIDTH,
            //        data.Level.RenderSize.Height * ScenarioConfiguration.CELLHEIGHT,
            //        out xneg,
            //        out yneg);

            //    double maxWidth = 4;
            //    double dist = Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));
            //    r.Width = (((1 - maxWidth) / 50) * dist) + maxWidth;
            //    r.Height = r.Width;

            //    if (yneg)
            //        Canvas.SetTop(r, 50 - p.Y);
            //    else
            //        Canvas.SetTop(r, p.Y + 48);

            //    if (xneg)
            //        Canvas.SetLeft(r, 50 - p.X);
            //    else
            //        Canvas.SetLeft(r, p.X + 48);

            //    _canvasPoints.Add(r);
            //    this.GlobeCanvas.Children.Add(r);
            //}
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
    }
}
