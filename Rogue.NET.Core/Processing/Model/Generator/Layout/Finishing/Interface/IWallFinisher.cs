using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface
{
    public interface IWallFinisher
    {
        /// <summary>
        /// Creates wall outline for each region in the grid (using null cells) by checking 8-way adjacency.
        /// </summary>
        /// <param name="grid">The layout grid</param>
        void CreateWalls(GridCellInfo[,] grid);
    }
}
