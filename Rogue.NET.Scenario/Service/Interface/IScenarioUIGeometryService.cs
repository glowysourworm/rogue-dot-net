using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Windows;

namespace Rogue.NET.Scenario.Service.Interface
{
    public interface IScenarioUIGeometryService
    {
        Point Cell2UI(int x, int y);
        Point Cell2UI(CellPoint p, bool offsetToMiddleOfCell = false);
        Rect Cell2UIRect(CellPoint p, bool addCellOffset);
        Rect Cell2UIRect(CellRectangle r);
    }
}
