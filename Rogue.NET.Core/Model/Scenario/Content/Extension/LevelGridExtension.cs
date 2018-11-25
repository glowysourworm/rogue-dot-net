using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Extension
{
    public static class LevelGridExtension
    {
        public static IEnumerable<CellPoint> GetAdjacentLocations(this LevelGrid grid, CellPoint location)
        {
            var n = grid[location.Column, location.Row - 1]?.Location ?? null;
            var s = grid[location.Column, location.Row + 1]?.Location ?? null;
            var e = grid[location.Column + 1, location.Row]?.Location ?? null;
            var w = grid[location.Column - 1, location.Row - 1]?.Location ?? null;
            var ne = grid[location.Column + 1, location.Row - 1]?.Location ?? null;
            var nw = grid[location.Column - 1, location.Row - 1]?.Location ?? null;
            var se = grid[location.Column + 1, location.Row + 1]?.Location ?? null;
            var sw = grid[location.Column - 1, location.Row + 1]?.Location ?? null;

            var result = new List<CellPoint>() { n, s, e, w, ne, nw, se, sw };

            return result.Where(x => x != null);
        }
    }
}
