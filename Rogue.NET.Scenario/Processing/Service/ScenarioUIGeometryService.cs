using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET.Scenario.Processing.Service
{
    [Export(typeof(IScenarioUIGeometryService))]
    public class ScenarioUIGeometryService : IScenarioUIGeometryService
    {
        public Point Cell2UI(int x, int y)
        {
            return new Point(x * ModelConstants.CellWidth, y * ModelConstants.CellHeight);
        }
        public Point Cell2UI(GridLocation p, bool offsetToMiddleOfCell = false)
        {
            if (!offsetToMiddleOfCell)
                return new Point(p.Column * ModelConstants.CellWidth, p.Row * ModelConstants.CellHeight);

            else
                return new Point((p.Column + 0.5D) * ModelConstants.CellWidth, (p.Row + 0.5D) * ModelConstants.CellHeight);
        }
        public Rect Cell2UIRect(GridLocation p, bool addCellOffset)
        {
            if (!addCellOffset)
                return new Rect(Cell2UI(p), new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));
            else
            {
                Point pt = Cell2UI(p);
                pt.X += ModelConstants.CellWidth / 2;
                pt.Y += ModelConstants.CellHeight / 2;
                return new Rect(pt, new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));
            }
        }
        public Rect Cell2UIRect(int column, int row)
        {
            return new Rect(Cell2UI(column, row), new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));
        }
        public Rect Cell2UIRect(RegionBoundary r)
        {
            Point p = Cell2UI((int)r.Column, (int)r.Row);
            int w = (int)r.Width * ModelConstants.CellWidth;
            int h = (int)r.Height * ModelConstants.CellHeight;
            return new Rect(p, new Size(w, h));
        }

        public GridLocation UI2Cell(double roughX, double roughY)
        {
            return new GridLocation((int)(roughX / ModelConstants.CellWidth),
                                    (int)(roughY / ModelConstants.CellHeight));
        }

        public GridLocation UI2Cell(Point roughLocation)
        {
            return new GridLocation((int)(roughLocation.X / ModelConstants.CellWidth),
                                    (int)(roughLocation.Y / ModelConstants.CellHeight));
        }
    }
}
