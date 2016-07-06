using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common;
using Rogue.NET.Model;
using Rogue.NET.Model.Scenario;

namespace Rogue.NET.Model
{
    public static class DataHelper
    {
        //Configuration Helper
        public static Point Cell2UI(int x, int y)
        {
            return new Point(x * ScenarioConfiguration.CELLWIDTH, y * ScenarioConfiguration.CELLHEIGHT);
        }
        public static Point Cell2UI(Point p)
        {
            return new Point(p.X * ScenarioConfiguration.CELLWIDTH, p.Y * ScenarioConfiguration.CELLHEIGHT);
        }
        public static Rect Cell2UIRect(Point p, bool addCellOffset)
        {
            if (!addCellOffset)
                return new Rect(Cell2UI(p), new Size(ScenarioConfiguration.CELLWIDTH, ScenarioConfiguration.CELLHEIGHT));
            else
            {
                Point pt = Cell2UI(p);
                pt.X += ScenarioConfiguration.CELLWIDTH / 2;
                pt.Y += ScenarioConfiguration.CELLHEIGHT / 2;
                return new Rect(pt, new Size(ScenarioConfiguration.CELLWIDTH, ScenarioConfiguration.CELLHEIGHT));
            }
        }
        public static Rect Cell2UIRect(Rect r)
        {
            Point p = Cell2UI((int)r.Location.X, (int)r.Location.Y);
            int w = (int)r.Width * ScenarioConfiguration.CELLWIDTH;
            int h = (int)r.Height * ScenarioConfiguration.CELLHEIGHT;
            return new Rect(p, new Size(w, h));
        }

        //Cell Helpers
        /// <summary>
        /// Creates path figure to display walls
        /// </summary>
        /// <param name="p">Cell Point</param>
        /// <param name="n_search">north search counter</param>
        /// <param name="s_search">south search counter</param>
        /// <param name="e_search">east search counter</param>
        /// <param name="w_search">west search counter</param>
        /// <returns></returns>
        public static PathFigure CreateWallsPathFigure(Point p,
            Compass visibleDoors,
            Compass walls)
        {
            Rect r = Cell2UIRect(p, false);
            PathFigure pf = new PathFigure();
            pf.StartPoint = r.TopLeft;
            pf.Segments.Add(new LineSegment(r.TopRight, (walls & Compass.N) != 0 || (visibleDoors & Compass.N) != 0));
            pf.Segments.Add(new LineSegment(r.BottomRight, (walls & Compass.E) != 0 || (visibleDoors & Compass.E) != 0));
            pf.Segments.Add(new LineSegment(r.BottomLeft, (walls & Compass.S) != 0 || (visibleDoors & Compass.S) != 0));
            pf.Segments.Add(new LineSegment(r.TopLeft, (walls & Compass.W) != 0 || (visibleDoors & Compass.W) != 0));
            //pf.Segments.Add(new LineSegment(r.TopRight, (walls & Compass.N) != 0));
            //pf.Segments.Add(new LineSegment(r.BottomRight, (walls & Compass.E) != 0));
            //pf.Segments.Add(new LineSegment(r.BottomLeft, (walls & Compass.S) != 0));
            //pf.Segments.Add(new LineSegment(r.TopLeft, (walls & Compass.W) != 0));
            return pf;
        }
        /// <summary>
        /// Creates path figure to display walls
        /// </summary>
        /// <param name="p">Cell Point</param>
        /// <param name="n_search">north search counter</param>
        /// <param name="s_search">south search counter</param>
        /// <param name="e_search">east search counter</param>
        /// <param name="w_search">west search counter</param>
        /// <returns></returns>
        public static PathFigure CreateDoorsPathFigure(Point p,
            Compass visibleDoors)
        {
            Rect r = Cell2UIRect(p, false);
            PathFigure pf = new PathFigure();
            pf.StartPoint = r.TopLeft;
            pf.Segments.Add(new LineSegment(r.TopRight, (visibleDoors & Compass.N) != 0));
            pf.Segments.Add(new LineSegment(r.BottomRight, (visibleDoors & Compass.E) != 0));
            pf.Segments.Add(new LineSegment(r.BottomLeft, (visibleDoors & Compass.S) != 0));
            pf.Segments.Add(new LineSegment(r.TopLeft, (visibleDoors & Compass.W) != 0));
            return pf;
        }

        public static Compass GetRandomDirection(Random r)
        {
            double d = r.NextDouble();
            if (d < 1 / 8.0D)
                return Compass.E;

            else if (d < 1 / 4.0D)
                return Compass.N;

            else if (d < 3 / 8.0D)
                return Compass.NE;

            else if (d < 1 / 2.0D)
                return Compass.NW;

            else if (d < 5 / 8.0D)
                return Compass.S;

            else if (d < 6 /8.0D)
                return Compass.SE;

            else if (d < 7 / 8.0D)
                return Compass.SW;

            return Compass.W;
        }

        public static int GetZIndex(ScenarioObject obj)
        {
            if (obj.GetType() == typeof(Character))
                return 3;

            else if (obj.GetType() == typeof(Item))
                return 2;

            else
                return 1;
        }
    }
}
