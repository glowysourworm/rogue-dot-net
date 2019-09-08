using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Windows;

namespace Rogue.NET.Scenario.Processing.Service.Interface
{
    public interface IScenarioUIGeometryService
    {
        Point Cell2UI(int x, int y);
        Point Cell2UI(GridLocation p, bool offsetToMiddleOfCell = false);
        Rect Cell2UIRect(GridLocation p, bool addCellOffset);
        Rect Cell2UIRect(CellRectangle r);
    }
}
