using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.ComponentModel.Composition;
using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionBuilder))]
    public class RegionBuilder : IRegionBuilder
    {
        readonly IRegionGeometryCreator _regionGeometryCreator;
        readonly IRectangularRegionCreator _rectangularRegionCreator;
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly ICellularAutomataRegionCreator _cellularAutomataRegionCreator;
        readonly INoiseGenerator _noiseGenerator;

        [ImportingConstructor]
        public RegionBuilder(IRegionGeometryCreator regionGeometryCreator,
                             IRectangularRegionCreator rectangularRegionCreator,
                             IMazeRegionCreator mazeRegionCreator,
                             ICellularAutomataRegionCreator cellularAutomataRegionCreator,
                             INoiseGenerator noiseGenerator)
        {
            _regionGeometryCreator = regionGeometryCreator;
            _rectangularRegionCreator = rectangularRegionCreator;
            _mazeRegionCreator = mazeRegionCreator;
            _cellularAutomataRegionCreator = cellularAutomataRegionCreator;
            _noiseGenerator = noiseGenerator;
        }

        public GridCellInfo[,] BuildRegions(LayoutTemplate template)
        {
            switch (template.Type)
            {
                case LayoutType.RectangularRegion:
                    return CreateRectangularGridRegions(template);
                case LayoutType.RandomRectangularRegion:
                    return CreateRandomRectangularRegions(template);
                case LayoutType.MazeMap:
                    return CreateMazeMap(template);
                case LayoutType.ElevationMap:
                    return CreateElevationMap(template);
                case LayoutType.CellularAutomataMap:
                    return CreateCellularAutomataMap(template);
                default:
                    throw new Exception("Unhandled Layout Type RegionBuilder");
            }
        }
        public GridCellInfo[,] BuildDefaultRegion()
        {
            var grid = new GridCellInfo[20, 15];

            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                    grid[i, j] = new GridCellInfo(i, j);

            return grid;
        }

        private GridCellInfo[,] CreateRectangularGridRegions(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = _regionGeometryCreator.CreateGridRectangularRegions(template.NumberRoomCols, template.NumberRoomRows,
                                                                                     template.RectangularGridPadding, template.RegionWidthRange,
                                                                                     template.RegionHeightRange);

            // Create cells in the regions
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCells(grid, boundary, false);

            return grid;
        }

        private GridCellInfo[,] CreateRandomRectangularRegions(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the room rectangles - IF THERE'S TOO MUCH CLUTTER WITH SYMMETRY THEN WE CAN LIMIT THE BOUNDARY TO THE FIRST QUADRANT
            var roomBoundaries = _regionGeometryCreator.CreateRandomRectangularRegions(new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height),
                                                                                       template.RandomRoomCount, template.RegionWidthRange, template.RegionHeightRange,
                                                                                       template.RandomRoomSpread);

            // Create contiguous regions - OVERWRITE EXISTING CELLS BECAUSE OF RANDOM LAYOUT
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCells(grid, boundary, true);

            return grid;
        }

        private GridCellInfo[,] CreateCellularAutomataMap(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);

            // Create cellular automata in each region
            _cellularAutomataRegionCreator.GenerateCells(grid, boundary, template.CellularAutomataType, template.CellularAutomataFillRatio, false);

            return grid;
        }

        private GridCellInfo[,] CreateMazeMap(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);

            // Create cellular automata in each region
            _mazeRegionCreator.CreateCells(grid, boundary, template.MazeWallRemovalRatio, false);

            return grid;
        }

        private GridCellInfo[,] CreateElevationMap(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);

            // Create the regions using noise generation
            _noiseGenerator.Run(NoiseType.PerlinNoise, template.Width, template.Height, template.OpenWorldElevationFrequency, new PostProcessingCallback((column, row, value) =>
            {
                // Create cells within the elevation band
                if (template.OpenWorldElevationRegionRange.Contains(value))
                {
                    grid[column, row] = new GridCellInfo(column, row);
                }

                return value;
            }));

            return grid;
        }
    }
}
