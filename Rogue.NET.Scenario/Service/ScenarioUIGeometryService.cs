using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Scenario.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET.Scenario.Service
{
    [Export(typeof(IScenarioUIGeometryService))]
    public class ScenarioUIGeometryService : IScenarioUIGeometryService
    {
        public Point Cell2UI(int x, int y)
        {
            return new Point(x * ModelConstants.CellWidth, y * ModelConstants.CellHeight);
        }
        public Point Cell2UI(CellPoint p, bool offsetToMiddleOfCell = false)
        {
            if (!offsetToMiddleOfCell)
                return new Point(p.Column * ModelConstants.CellWidth, p.Row * ModelConstants.CellHeight);

            else
                return new Point((p.Column + 0.5D) * ModelConstants.CellWidth, (p.Row + 0.5D) * ModelConstants.CellHeight);
        }
        public Rect Cell2UIRect(CellPoint p, bool addCellOffset)
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
        public Rect Cell2UIRect(CellRectangle r)
        {
            Point p = Cell2UI((int)r.Location.Column, (int)r.Location.Row);
            int w = (int)r.CellWidth * ModelConstants.CellWidth;
            int h = (int)r.CellHeight * ModelConstants.CellHeight;
            return new Rect(p, new Size(w, h));
        }
    }
}
