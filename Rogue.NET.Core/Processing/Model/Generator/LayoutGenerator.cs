using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        readonly IRegionBuilder _regionBuilder;
        readonly IConnectionBuilder _corridorBuilder;
        readonly ITerrainBuilder _terrainBuilder;
        readonly IWallFinisher _wallFinisher;
        readonly ICellularAutomataRegionCreator _cellularAutomataRegionCreator;
        readonly ILightingFinisher _lightingFinisher;

        // Size is great enough for an upstairs, downstairs, and 2 teleport pods (in and out of the room)
        private const int ROOM_SIZE_MIN = 4;

        // Default name used to initialize empty layer
        private const string DEFAULT_LAYER_NAME = "Default Layer";

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               IConnectionBuilder corridorBuilder,
                               ITerrainBuilder terrainBuilder,
                               IWallFinisher wallFinisher,
                               ICellularAutomataRegionCreator cellularAutomataRegionCreator,
                               ILightingFinisher lightingFinisher)
        {
            _regionBuilder = regionBuilder;
            _corridorBuilder = corridorBuilder;
            _terrainBuilder = terrainBuilder;
            _wallFinisher = wallFinisher;
            _cellularAutomataRegionCreator = cellularAutomataRegionCreator;
            _lightingFinisher = lightingFinisher;
        }

        public LevelGrid CreateLayout(LayoutTemplate template)
        {
            // Build regions of cells to initialize the layout grid
            var grid = _regionBuilder.BuildRegions(template);

            // Identify Regions and create connectors
            var regions = CreateCorridors(grid, template);

            if (regions.Count() <= 0)
                return CreateDefaultLayout();

            // Make Symmetric!
            if (template.MakeSymmetric)
            {
                // First, copy cells over
                MakeSymmetric(grid, template.SymmetryType);

                // Second, re-create corridors
                regions = CreateCorridors(grid, template);
            }

            var roomLayer = new LayerInfo("Room Layer", regions);

            // Final room layer is calculated by the terrain builder
            // LayerInfo roomLayer;

            // Build Terrain -> Identify new regions -> Re-connect regions
            // var terrainLayers = _terrainBuilder.BuildTerrain(grid, template, out roomLayer);

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Create Lighting
            _lightingFinisher.CreateLighting(grid, roomLayer, template);

            // return new LevelGrid(grid, roomLayer, terrainLayers);
            return new LevelGrid(grid, roomLayer, new LayerInfo[] { });
        }

        private IEnumerable<Region> CreateCorridors(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Use rectilinear corridors for rectangular region layouts only
            var rectilinearCorridors = template.Type == LayoutType.RandomRectangularRegion ||
                                       template.Type == LayoutType.RectangularRegion;

            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                    return rectilinearCorridors ? _corridorBuilder.BuildRectilinearCorridors(grid) :
                                                     _corridorBuilder.BuildCorridors(grid);
                case LayoutConnectionType.Teleporter:
                    return _corridorBuilder.BuildConnectionPoints(grid);
                case LayoutConnectionType.Maze:
                    return _corridorBuilder.BuildMazeCorridors(grid);
                default:
                    throw new Exception("Unhandled Connection Type");
            }
        }

        private LevelGrid CreateDefaultLayout()
        {
            var grid = _regionBuilder.BuildDefaultRegion();

            var regions = grid.IdentifyRegions();

            return new LevelGrid(grid, new LayerInfo("Room Layer", regions), new LayerInfo[] { });
        }

        private void MakeSymmetric(GridCellInfo[,] grid, LayoutSymmetryType symmetryType)
        {
            switch (symmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    {
                        // E Half
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1); j++)
                            {
                                // E -> W
                                if (grid[i, j] != null)
                                    grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j) { IsWall = grid[i, j].IsWall };

                                // W -> E
                                else if (grid[mirrorColumn, j] != null)
                                    grid[i, j] = new GridCellInfo(i, j) { IsWall = grid[mirrorColumn, j].IsWall };
                            }
                        }
                    }
                    break;
                case LayoutSymmetryType.Quadrant:
                    {
                        // NE, SE, SW Quadrants
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1) / 2; j++)
                            {
                                var mirrorRow = grid.GetLength(1) - j - 1;

                                GridCellInfo[] cells = new GridCellInfo[4];

                                // Find cell to mirror - start with NW

                                // NW
                                if (grid[i, j] != null)
                                    cells[0] = grid[i, j];

                                // NE
                                else if (grid[mirrorColumn, j] != null)
                                    cells[1] = grid[mirrorColumn, j];

                                // SE
                                else if (grid[mirrorColumn, mirrorRow] != null)
                                    cells[2] = grid[mirrorColumn, mirrorRow];

                                // SW
                                else if (grid[i, mirrorRow] != null)
                                    cells[3] = grid[i, mirrorRow];

                                var cell = cells.FirstOrDefault(x => x != null);

                                // Mirror cell over to other quadrants
                                if (cell != null)
                                {
                                    grid[i, j] = new GridCellInfo(i, j) { IsWall = cell.IsWall };
                                    grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j) { IsWall = cell.IsWall };
                                    grid[i, mirrorRow] = new GridCellInfo(i, mirrorRow) { IsWall = cell.IsWall };
                                    grid[mirrorColumn, mirrorRow] = new GridCellInfo(mirrorColumn, mirrorRow) { IsWall = cell.IsWall };
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Validates the level - returns false if there is an issue so that a default layout can be generated. NOTE*** THROWS EXCEPTIONS
        /// DURING DEBUG INSTEAD.
        /// </summary>
        private bool Validate(GridCellInfo[,] grid, LayerInfo roomLayer, IEnumerable<LayerInfo> terrainLayers)
        {
            var savePoint = false;
            var stairsUp = false;
            var stairsDown = false;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null &&
                        grid[i, j].IsMandatory)
                    {
                        savePoint |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.SavePoint;
                        stairsUp |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.StairsUp;
                        stairsDown |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.StairsDown;
                    }
                }
            }

#if DEBUG
            if (!savePoint)
                throw new Exception("Layout must have a mandatory cell for the save point");

            if (!stairsUp)
                throw new Exception("Layout must have a mandatory cell for the stairs up");

            if (!stairsDown)
                throw new Exception("Layout must have a mandatory cell for the stairs down");
#else
            if (!savePoint)
                return false;

            if (!stairsUp)
                return false;

            if (!stairsDown)
                return false;
#endif

            foreach (var region in roomLayer.Regions)
            {
                var roomConnector1 = false;
                var roomConnector2 = false;

                foreach (var cell in region.Cells)
                {
                    if (grid[cell.Column, cell.Row].IsMandatory &&
                        grid[cell.Column, cell.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector1)
                        roomConnector1 = true;

                    if (grid[cell.Column, cell.Row].IsMandatory &&
                        grid[cell.Column, cell.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector2)
                        roomConnector2 = true;
                }

#if DEBUG
                if (region.Cells.Length < ROOM_SIZE_MIN)
                    throw new Exception("Room Regions must have a minimum size of " + ROOM_SIZE_MIN.ToString());

                if (!roomConnector1)
                    throw new Exception("Room doesn't have a mandatory cell for connector 1");

                if (!roomConnector2)
                    throw new Exception("Room doesn't have a mandatory cell for connector 2");
#else
                if (region.Cells.Length < ROOM_SIZE_MIN)
                    return false;

                if (!roomConnector1)
                    return false;

                if (!roomConnector2)
                    return false;
#endif
            }

            return true;
        }
    }
}
