using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRectangularRegionCreator))]
    public class RectangularRegionCreator : IRectangularRegionCreator
    {
        public RectangularRegionCreator()
        {

        }

        public void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, bool overwrite)
        {
            for (int i = boundary.Left; i <= boundary.Right; i++)
            {
                for (int j = boundary.Top; j <= boundary.Bottom; j++)
                {
                    if (grid[i, j] != null && !overwrite)
                        throw new Exception("Trying to overwrite existing grid cell RectangularRegionCreator");

                    grid[i, j] = new GridCellInfo(i, j);
                }
            } 
        }
    }
}
