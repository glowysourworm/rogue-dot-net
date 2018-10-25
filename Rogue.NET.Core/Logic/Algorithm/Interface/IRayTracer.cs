using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Algorithm.Interface
{
    public interface IRayTracer
    {
        IEnumerable<CellPoint> GetVisibleLocations(LevelGrid grid, CellPoint cell, int lightRadius);
    }
}
