using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Windows;

namespace Rogue.NET.Scenario.Processing.Service.Interface
{
    public interface IScenarioUIGeometryService
    {
        Point Cell2UI(int x, int y);
        Point Cell2UI(GridLocation p, bool offsetToMiddleOfCell = false);
        Rect Cell2UIRect(GridLocation p, bool addCellOffset);
        Rect Cell2UIRect(RegionBoundary r);

        GridLocation UI2Cell(double roughX, double roughY);
        GridLocation UI2Cell(Point roughLocation);
    }
}
