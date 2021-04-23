using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionBuilder))]
    public class RegionBuilder : IRegionBuilder
    {
        readonly IRegionGeometryCreator _regionGeometryCreator;
        readonly IRectangularRegionCreator _rectangularRegionCreator;
        readonly ICellularAutomataRegionCreator _cellularAutomataRegionCreator;
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly INoiseGenerator _noiseGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRegionTriangulationCreator _triangulationCreator;

        // TODO: Move to primary model constants
        const int LAYOUT_WIDTH_MAX = 100;
        const int LAYOUT_WIDTH_MIN = 20;
        const int LAYOUT_HEIGHT_MAX = 60;
        const int LAYOUT_HEIGHT_MIN = 16;

        [ImportingConstructor]
        public RegionBuilder(IRegionGeometryCreator regionGeometryCreator,
                             IRectangularRegionCreator rectangularRegionCreator,
                             ICellularAutomataRegionCreator cellularAutomataRegionCreator,
                             IMazeRegionCreator mazeRegionCreator,
                             INoiseGenerator noiseGenerator,
                             IRandomSequenceGenerator randomSequenceGenerator,
                             IRegionTriangulationCreator triangulationCreator)
        {
            _regionGeometryCreator = regionGeometryCreator;
            _rectangularRegionCreator = rectangularRegionCreator;
            _cellularAutomataRegionCreator = cellularAutomataRegionCreator;
            _mazeRegionCreator = mazeRegionCreator;
            _noiseGenerator = noiseGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _triangulationCreator = triangulationCreator;
        }

        public LayoutContainer BuildRegions(LayoutTemplate template)
        {
            GridCellInfo[,] grid;

            switch (template.Type)
            {
                case LayoutType.RectangularRegion:
                    grid = CreateRectangularGridRegions(template);
                    break;
                case LayoutType.RandomRectangularRegion:
                    grid = CreateRandomRectangularRegions(template, false);
                    break;
                case LayoutType.RandomSmoothedRegion:
                    grid = CreateRandomRectangularRegions(template, true);
                    break;
                case LayoutType.MazeMap:
                    grid = CreateMazeMap(template);
                    break;
                case LayoutType.ElevationMap:
                    grid = CreateElevationMap(template);
                    break;
                case LayoutType.CellularAutomataMap:
                    grid = CreateCellularAutomataMap(template);
                    break;
                case LayoutType.CellularAutomataMazeMap:
                    grid = CreateCellularAutomataMazeMap(template);
                    break;
                case LayoutType.ElevationMazeMap:
                    grid = CreateElevationMazeMap(template);
                    break;
                default:
                    throw new Exception("Unhandled Layout Type RegionBuilder");
            }

            return CompleteBaseLayout(grid, template);
        }

        public LayoutContainer BuildDefaultLayout()
        {
            var grid = new GridCellInfo[20, 15];

            for (int i = 1; i < grid.GetLength(0) - 1; i++)
                for (int j = 1; j < grid.GetLength(1) - 1; j++)
                    grid[i, j] = new GridCellInfo(i, j);

            return CompleteBaseLayout(grid, null);
        }

        private LayoutContainer CompleteBaseLayout(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Remove invalid regions
            grid.RemoveInvalidRegions(cell => !cell.IsWall, region => !RegionValidator.ValidateBaseRegion(region));

            // Create container for the layout
            var container = new LayoutContainer(grid, template == null);

            // Identify the region
            var baseRegions = grid.ConstructConnectedRegions(cell => !cell.IsWall);

            // Create triangulation to complete the connection layer
            var graph = template == null ? _triangulationCreator.CreateDefaultTriangulation(baseRegions)
                                         : _triangulationCreator.CreateTriangulation(baseRegions, template);

            container.SetBaseLayer(baseRegions);
            container.SetConnectionLayer(baseRegions, graph);

            return container;
        }

        private GridCellInfo[,] CreateGrid(LayoutTemplate template)
        {
            // NOTE*** LAYOUT SIZE IS PRE-CALCULATED BASED ON ALL TEMPLATE PARAMETERS (INCLUDING SYMMETRY)
            var width = (int)(template.WidthRatio * (LAYOUT_WIDTH_MAX - LAYOUT_WIDTH_MIN)) + LAYOUT_WIDTH_MIN;
            var height = (int)(template.HeightRatio * (LAYOUT_HEIGHT_MAX - LAYOUT_HEIGHT_MIN)) + LAYOUT_HEIGHT_MIN;

            return new GridCellInfo[width, height];
        }

        private GridCellInfo[,] CreateRectangularGridRegions(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Create the room rectangles
            var roomBoundaries = _regionGeometryCreator.CreateGridRectangularRegions(grid.GetLength(0), grid.GetLength(1), template.RoomColumnRatio,
                                                                                     template.RoomRowRatio, template.RoomSize, template.FillRatioRooms,
                                                                                     template.RoomSizeErradicity);
            // Create cells in the regions
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCells(grid, boundary, false);

            return grid;
        }

        private GridCellInfo[,] CreateRandomRectangularRegions(LayoutTemplate template, bool runSmoothingIteration)
        {
            var grid = CreateGrid(template);

            // Create the room rectangles - IF THERE'S TOO MUCH CLUTTER WITH SYMMETRY THEN WE CAN LIMIT THE BOUNDARY TO THE FIRST QUADRANT
            var roomBoundaries = _regionGeometryCreator.CreateRandomRectangularRegions(grid.GetLength(0), grid.GetLength(1), template.FillRatioRooms, template.RoomSize, template.RoomSizeErradicity);

            // Calculate padding limits
            var roomMinHeight = roomBoundaries.Min(region => region.Height);
            var roomMinWidth = roomBoundaries.Min(region => region.Width);

            var paddingLimit = System.Math.Min(roomMinHeight / 2, roomMinWidth / 2);
            var padding = (int)(paddingLimit * template.RandomRoomSpacing).Clip(0, paddingLimit);

            // Create contiguous regions - OVERWRITE EXISTING CELLS BECAUSE OF RANDOM LAYOUT
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCellsXOR(grid, boundary, padding, template.RandomRoomSeparationRatio);

            // Run one smoothing / roughness iteration to make rough edges
            if (runSmoothingIteration)
            {
                //foreach (var boundary in roomBoundaries)
                _cellularAutomataRegionCreator.RunSmoothingIteration(grid, new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1)), template.CellularAutomataType);
            }

            return grid;
        }

        private GridCellInfo[,] CreateCellularAutomataMap(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Create cellular automata in the
            _cellularAutomataRegionCreator.GenerateCells(grid, boundary, template.CellularAutomataType, template.CellularAutomataFillRatio, false);

            return grid;
        }

        private GridCellInfo[,] CreateCellularAutomataMazeMap(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Create cellular automata the region
            _cellularAutomataRegionCreator.GenerateCells(grid, boundary, template.CellularAutomataType, template.CellularAutomataFillRatio, false);

            // Fills cell regions with mazes
            FillRegionsWithMazes(grid, template.MazeWallRemovalRatio, template.MazeHorizontalVerticalBias);

            return grid;
        }

        private GridCellInfo[,] CreateMazeMap(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Create the boundary
            var boundary = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Create maze in the region
            _mazeRegionCreator.CreateCells(grid, boundary, MazeType.Filled, template.MazeWallRemovalRatio, template.MazeHorizontalVerticalBias, false);

            return grid;
        }

        private GridCellInfo[,] CreateElevationMap(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Edge padding
            var padding = 1;

            // Map [-1, 1] to the proper elevation band of 0.4 using [0, 1] elevation selector
            var elevationLow = (1.6 * template.ElevationSelector) - 1;
            var elevationHigh = (1.6 * template.ElevationSelector) - 0.6;

            // Create the regions using noise generation
            _noiseGenerator.Run(NoiseType.PerlinNoise, grid.GetLength(0), grid.GetLength(1), template.ElevationFrequency, (column, row, value) =>
            {
                // Leave padding around the edge
                if (column < padding ||
                    row < padding ||
                    column + padding >= grid.GetLength(0) ||
                    row + padding >= grid.GetLength(1))
                    return 0;

                // Create cells within the elevation band
                if (value.Between(elevationLow, elevationHigh, true))
                {
                    grid[column, row] = new GridCellInfo(column, row);
                }

                return value;
            });

            // Run smoothing iteration
            _cellularAutomataRegionCreator.RunSmoothingIteration(grid, new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1)), template.CellularAutomataType);

            return grid;
        }

        private GridCellInfo[,] CreateElevationMazeMap(LayoutTemplate template)
        {
            var grid = CreateGrid(template);

            // Edge padding
            var padding = 1;

            // Map [-1, 1] to the proper elevation band of 0.4 using [0, 1] elevation selector
            var elevationLow = (1.6 * template.ElevationSelector) - 1;
            var elevationHigh = (1.6 * template.ElevationSelector) - 0.6;

            // Create the regions using noise generation
            _noiseGenerator.Run(NoiseType.PerlinNoise, grid.GetLength(0), grid.GetLength(1), template.ElevationFrequency, (column, row, value) =>
            {
                // Leave padding around the edge
                if (column < padding ||
                    row < padding ||
                    column + padding >= grid.GetLength(0) ||
                    row + padding >= grid.GetLength(1))
                    return 0;

                // Create cells within the elevation band
                if (value.Between(elevationLow, elevationHigh, true))
                {
                    grid[column, row] = new GridCellInfo(column, row);
                }

                return value;
            });

            // Run smoothing iteration
            _cellularAutomataRegionCreator.RunSmoothingIteration(grid, new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1)), template.CellularAutomataType);

            // Fills cell regions with mazes
            FillRegionsWithMazes(grid, template.MazeWallRemovalRatio, template.MazeHorizontalVerticalBias);

            return grid;
        }

        private void FillRegionsWithMazes(GridCellInfo[,] grid, double wallRemovalRatio, double horizontalVerticalBias)
        {
            // Identify regions
            var regions = grid.ConstructRegions(cell => !cell.IsWall);

            if (regions.Count() == 0)
                throw new Exception("Trying to fill regions with mazes; but no regions were generated");

            // Create walls inside each region and run maze generator
            foreach (var region in regions)
            {
                foreach (var location in region.Locations)
                    grid[location.Column, location.Row].IsWall = true;

                for (int i = 0; i < region.Locations.Length; i++)
                {
                    // Look for other places to start a maze
                    if (grid.GetAdjacentElements(region.Locations[i].Column, region.Locations[i].Row)
                            .All(cell => cell.IsWall))
                        _mazeRegionCreator.CreateCellsStartingAt(grid, new Region<GridCellInfo>[] { }, _randomSequenceGenerator.GetRandomElement(region.Locations).Location, MazeType.Open, wallRemovalRatio, horizontalVerticalBias);
                }
            }
        }
    }
}
