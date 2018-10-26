using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Logic.Algorithm.Interface
{
    public interface IRayTracer
    {
        IEnumerable<CellPoint> GetVisibleLocations(LevelGrid grid, CellPoint cell, int lightRadius);
        Point Transform(CellPoint p);
    }
}
