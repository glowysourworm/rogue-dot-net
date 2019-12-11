using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        readonly IRegionBuilder _regionBuilder;
        readonly IConnectionBuilder _connectionBuilder;
        readonly ITerrainBuilder _terrainBuilder;
        readonly IWallFinisher _wallFinisher;
        readonly ILightingFinisher _lightingFinisher;
        readonly IRegionValidator _regionValidator;

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               IConnectionBuilder connectionBuilder,
                               ITerrainBuilder terrainBuilder,
                               IWallFinisher wallFinisher,
                               ILightingFinisher lightingFinisher,
                               IRegionValidator regionValidator)
        {
            _regionBuilder = regionBuilder;
            _connectionBuilder = connectionBuilder;
            _terrainBuilder = terrainBuilder;
            _wallFinisher = wallFinisher;
            _lightingFinisher = lightingFinisher;
            _regionValidator = regionValidator;
        }

        public LevelGrid CreateLayout(LayoutTemplate template)
        {
            // Build regions of cells to initialize the layout grid
            var grid = _regionBuilder.BuildRegions(template);

            // Identify Regions and create connectors
            IEnumerable<Region> regions;

            if (IdentifyValidRegions(grid, out regions))
                _connectionBuilder.BuildConnections(grid, regions, template);

            // No Valid Regions -> return default layout
            else
                return CreateDefaultLayout();

            // Make Symmetric!
            if (template.MakeSymmetric)
            {
                // First, copy cells over
                MakeSymmetric(grid, template.SymmetryType);

                // Second, re-create corridors
                if (IdentifyValidRegions(grid, out regions))
                    _connectionBuilder.BuildConnections(grid, regions, template);

                // No Valid Regions -> return default layout
                else
                    return CreateDefaultLayout();
            }

            // Final room layer is calculated by the terrain builder
            LayerInfo roomLayer;
            IEnumerable<LayerInfo> terrainLayers;

            // If there are any terrain layers - proceed building them and re-creating any blocked corridors
            if (template.TerrainLayers.Any())
            {
                // Build Terrain -> Identify new regions -> Re-connect regions
                if (!_terrainBuilder.BuildTerrain(grid, regions, template, out roomLayer, out terrainLayers))
                    return CreateDefaultLayout();
            }

            // Create room layer with regions as-is
            else
            {
                roomLayer = new LayerInfo("Room Layer", regions);
                terrainLayers = new LayerInfo[] { };
            }

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Create Lighting
            _lightingFinisher.CreateLighting(grid, roomLayer, template);

            return new LevelGrid(grid, roomLayer, terrainLayers);
        }

        /// <summary>
        /// Identifies regions - removing invalid ones. Returns false if there are no valid regions.
        /// </summary>
        private bool IdentifyValidRegions(GridCellInfo[,] grid, out IEnumerable<Region> validRegions)
        {
            var regions = grid.IdentifyRegions();

            // Check for default room size constraints
            var invalidRegions = regions.Where(region => !_regionValidator.ValidateRoomRegion(region));

            // Set valid regions
            validRegions = regions.Except(invalidRegions);

            // Must have at least one valid region
            if (invalidRegions.Count() == regions.Count())
                return false;

            // Remove invalid regions
            else
            {
                foreach (var region in invalidRegions)
                {
                    // Remove region cells
                    foreach (var cell in region.Cells)
                        grid[cell.Column, cell.Row] = null;
                }
            }

            return true;
        }

        private LevelGrid CreateDefaultLayout()
        {
            var grid = _regionBuilder.BuildDefaultRegion();

            var regions = grid.IdentifyRegions();

            var roomLayer = new LayerInfo("Room Layer", regions);

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Create Lighting
            _lightingFinisher.CreateDefaultLighting(grid);

            return new LevelGrid(grid, roomLayer, new LayerInfo[] { });
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
                if (!_regionValidator.ValidateRoomRegion(region))
                    throw new Exception("Room Region invalid");

                if (!roomConnector1)
                    throw new Exception("Room doesn't have a mandatory cell for connector 1");

                if (!roomConnector2)
                    throw new Exception("Room doesn't have a mandatory cell for connector 2");
#else
                if (!_regionValidator.ValidateRoomRegion(region))
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
