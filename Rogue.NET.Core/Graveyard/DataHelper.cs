using System;
using System.Windows;
using System.Windows.Media;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

namespace Rogue.NET.Core.Graveyard
{
    public static class DataHelper
    {
        //Configuration Helper
        public static Point Cell2UI(int x, int y)
        {
            return new Point(x * ModelConstants.CELLWIDTH, y * ModelConstants.CELLHEIGHT);
        }
        public static Point Cell2UI(CellPoint p, bool offsetToMiddleOfCell = false)
        {
            if (!offsetToMiddleOfCell)
                return new Point(p.Column * ModelConstants.CELLWIDTH, p.Row * ModelConstants.CELLHEIGHT);

            else
                return new Point((p.Column + 0.5D) * ModelConstants.CELLWIDTH, (p.Row + 0.5D) * ModelConstants.CELLHEIGHT);
        }
        public static Rect Cell2UIRect(CellPoint p, bool addCellOffset)
        {
            if (!addCellOffset)
                return new Rect(Cell2UI(p), new Size(ModelConstants.CELLWIDTH, ModelConstants.CELLHEIGHT));
            else
            {
                Point pt = Cell2UI(p);
                pt.X += ModelConstants.CELLWIDTH / 2;
                pt.Y += ModelConstants.CELLHEIGHT / 2;
                return new Rect(pt, new Size(ModelConstants.CELLWIDTH, ModelConstants.CELLHEIGHT));
            }
        }
        public static Rect Cell2UIRect(CellRectangle r)
        {
            Point p = Cell2UI((int)r.Location.Column, (int)r.Location.Row);
            int w = (int)r.CellWidth * ModelConstants.CELLWIDTH;
            int h = (int)r.CellHeight * ModelConstants.CELLHEIGHT;
            return new Rect(p, new Size(w, h));
        }
    }
}
