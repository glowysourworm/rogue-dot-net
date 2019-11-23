using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface IRectangularRegionCreator
    {
        /// <summary>
        /// Creates cells within the given boundary - with option to overwrite existing cells. 
        /// </summary>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, bool overwrite);
    }
}
