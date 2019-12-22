using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ITerrainBuilder))]
    public class TerrainBuilder : ITerrainBuilder
    {
        readonly INoiseGenerator _noiseGenerator;
        readonly IConnectionBuilder _connectionBuilder;

        const int TERRAIN_PADDING = 2;

        [ImportingConstructor]
        public TerrainBuilder(INoiseGenerator noiseGenerator, IConnectionBuilder connectionBuilder)
        {
            _noiseGenerator = noiseGenerator;
            _connectionBuilder = connectionBuilder;
        }

        public bool BuildTerrain(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, LayoutTemplate template, out IEnumerable<LayerInfo> terrainLayers)
        {
            // Procedure
            //
            // 1) Generate all layers in order for this layout as separate 2D arrays
            // 2) Create masked grid from the primary grid removing impassible cells
            // 3) Re-identify regions to create new connections
            // 4) Expand any invalid regions to at LEAST the minimum size (using the region validator)
            //      - NOTE***  This works because the input regions were already valid
            // 5) Build corridors to connect the resulting regions (this will run flood fill to re-identify regions)
            // 6) Iterate the whole grid once to add any new corridor cells to the primary grid and to remove terrain cells
            //    where corridors were placed.
            // 7) Finally, finalize the terrain layers and the room layer
            //

            // Create all terrain layers in order and separate them by logical layers (see LayoutTerrainLayer enum)
            var terrainDict = CreateTerrain(grid, regions, template);

            // (Terrain Initial Clean-up) Remove non-overlapping terrain
            RemoveTerrainIslands(grid, terrainDict, regions);

            // Create combined terrain-blocked grid. This will have null cells where impassible terrain exists.
            var terrainMaskedGrid = CreateTerrainMaskedGrid(grid, terrainDict);

            // Check for invalid regions and expand them as needed - MODIFIES TERRAIN MASKED GRID AND TERRAIN GRID(S)
            // ExpandInvalidRegions(grid, terrainMaskedGrid, terrainDict);

            // Check for invalid regions and remove them - put null cells in to be filled with walls or paths
            RemoveInvalidRegions(grid, terrainMaskedGrid, terrainDict);

            // Create masked regions - THESE CONTAIN ORIGINAL REGION AND / OR CORRIDOR CELLS
            var maskedRegions = terrainMaskedGrid.IdentifyRegions(cell => !cell.IsWall);

            // Check that there are valid regions
            if (!maskedRegions.Any(region => RegionValidator.ValidateRoomRegion(region)))
            {
                terrainLayers = new LayerInfo[] { };
                return false;
            }

            // Calculate avoid regions for the connection builder
            var avoidRegions = terrainDict.Where(element => !element.Key.IsPassable &&
                                                             element.Key.ConnectionType == TerrainConnectionType.Avoid)
                                          .SelectMany(element => element.Value.IdentifyRegions(cell => true))
                                          .Actualize();

            // Create corridors and new regions
            _connectionBuilder.BuildConnectionsWithAvoidRegions(terrainMaskedGrid, maskedRegions, avoidRegions, template);

            // Transfer the corridor cells back to the primary and terrain grids
            TransferCorridors(terrainMaskedGrid, grid, terrainDict);

            // Create the terrain layers
            terrainLayers = terrainDict.Select(element =>
            {
                // First, identify terrain regions for this layer
                var regionsCellInfo = element.Value.IdentifyRegions(cell => true);

                // Next, convert to regions of grid locations
                var terrainRegions = regionsCellInfo.Select(cellInfoRegion =>
                {
                    var locations = cellInfoRegion.Locations.Select(cell => cell.Location);
                    var edgeLocations = cellInfoRegion.EdgeLocations.Select(cell => cell.Location);

                    return new Region<GridLocation>(locations.ToArray(),
                                                    edgeLocations.ToArray(),
                                                    cellInfoRegion.Boundary,
                                                    new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1)));
                });

                return new LayerInfo(element.Key.Name, terrainRegions, element.Key.IsPassable);

            }).Actualize();

            return true;
        }

        /// <summary>
        /// Creates terrain layers as 2D cell info array from the input primary grid and the template.
        /// </summary>
        private Dictionary<TerrainLayerTemplate, GridCellInfo[,]> CreateTerrain(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> baseRegions, LayoutTemplate template)
        {
            var terrainDict = new Dictionary<TerrainLayerTemplate, GridCellInfo[,]>();

            // Use the layer parameter to order the layers
            foreach (var terrain in template.TerrainLayers.OrderBy(layer => layer.TerrainLayer.Layer))
            {
                var terrainGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];

                switch (terrain.GenerationType)
                {
                    case TerrainGenerationType.PerlinNoise:
                        {
                            _noiseGenerator.Run(NoiseType.PerlinNoise,
                                                grid.GetLength(0),
                                                grid.GetLength(1),
                                                terrain.Frequency,
                                                new PostProcessingCallback(
                            (column, row, value) =>
                            {
                                // Leave padding around the edge
                                if (column < TERRAIN_PADDING ||
                                    row < TERRAIN_PADDING ||
                                    column + TERRAIN_PADDING >= grid.GetLength(0) ||
                                    row + TERRAIN_PADDING >= grid.GetLength(1))
                                    return 0;

                                // Translate from [0,1] fill ration to the [-1, 1] Perlin noise range
                                //
                                if (value < ((2 * terrain.FillRatio) - 1))
                                {
                                    // Check the cell's terrain layers for other entries
                                    if (!terrainDict.Any(element =>
                                    {
                                        // Terrain layer already present
                                        return element.Value[column, row] != null &&

                                               // Other terrain layers at this location exclude this layer
                                               (element.Key.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                               // Other terrain layers at this location overlay this layer
                                               (element.Key.LayoutType == TerrainLayoutType.Overlay &&
                                                element.Key.Layer > terrain.TerrainLayer.Layer));
                                    }))
                                    {
                                        // APPLY TERRAIN MASK - Also, remove walls / corridors appropriately
                                        //

                                        // Regions
                                        if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Regions) &&
                                             baseRegions.Any(region => region[column, row] != null))
                                        {
                                            // No Region mask applied - so go ahead and create the terrain
                                            terrainGrid[column, row] = grid[column, row];
                                        }

                                        // Corridors
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Corridors) &&
                                                  grid[column, row] != null &&
                                                  grid[column, row].IsCorridor)
                                        {
                                            // For impassible terrain - remove any wall or corridor settings
                                            if (!terrain.TerrainLayer.IsPassable)
                                                grid[column, row].IsCorridor = false;

                                            // No Corridor mask applied - so go ahead and create the terrain
                                            terrainGrid[column, row] = grid[column, row];
                                        }

                                        // Walls
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Walls) &&
                                                  grid[column, row] != null &&
                                                  grid[column, row].IsWall)
                                        {
                                            // For impassible terrain - remove any wall or corridor settings
                                            if (!terrain.TerrainLayer.IsPassable)
                                                grid[column, row].IsWall = false;

                                            // No Wall mask applied - so go ahead and create the terrain
                                            terrainGrid[column, row] = grid[column, row];
                                        }

                                        // Empty Space
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.EmptySpace) &&
                                                  grid[column, row] == null)
                                        {
                                            // First, create the grid cell
                                            grid[column, row] = new GridCellInfo(column, row);

                                            // No Empty Space mask applied - so go ahead and create the terrain in the new cell
                                            terrainGrid[column, row] = grid[column, row];
                                        }
                                    }
                                }

                                return value;
                            }));
                        }
                        break;
                    default:
                        throw new Exception("Unhandled terrain layer generation type");
                }

                // NOTE*** Not removing "small" (constrained size) terrain regions
                terrainDict.Add(terrain.TerrainLayer, terrainGrid);
            }

            return terrainDict;
        }

        /// <summary>
        /// Removes terrain islands from the terrain grids using 4-way adjacnecy check with the base regions - looks for a non-null base region cell. Also,
        /// modifies the base grid in case the terrain added cells to the grid
        /// </summary>
        private void RemoveTerrainIslands(GridCellInfo[,] grid, Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict, IEnumerable<Region<GridCellInfo>> baseRegions)
        {
            foreach (var element in terrainDict)
            {
                var terrainGrid = element.Value;
                var regions = terrainGrid.IdentifyRegions(cell => true);

                foreach (var region in regions)
                {
                    var foundRegion = false;

                    // Look for 4-way adjacent cells to edge locations. One must be non-null in a base region.
                    //
                    foreach (var location in region.EdgeLocations)
                    {
                        // Condition for keeping the terrain region
                        if (terrainGrid.GetCardinalAdjacentElements(location.Column, location.Row)
                                       .Any(cell => baseRegions.Any(baseRegion => baseRegion[cell.Column, cell.Row] != null)))
                        {
                            foundRegion = true;
                            break;
                        }
                    }

                    // Found terrain island
                    if (!foundRegion)
                    {
                        // Remove terrain from the terrain grid
                        foreach (var islandLocation in region.Locations)
                        {
                            terrainGrid[islandLocation.Column, islandLocation.Row] = null;

                            // Check to make sure that cell was part of the original grid. If not, then remove it.
                            if (!baseRegions.Any(region => region[islandLocation.Column, islandLocation.Row] != null) &&
                                !grid[islandLocation.Column, islandLocation.Row].IsWall &&
                                !grid[islandLocation.Column, islandLocation.Row].IsCorridor)
                            {
                                grid[islandLocation.Column, islandLocation.Row] = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a 2D cell array with impassible terrain removed from the primary grid
        /// </summary>
        private GridCellInfo[,] CreateTerrainMaskedGrid(GridCellInfo[,] grid, Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict)
        {
            // Detect new room regions
            var terrainMaskedGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];

            // Create new grid with removed cells for blocked terrain
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Found a grid cell - check for blocking terrain
                    if (grid[i, j] != null)
                    {
                        // Check for any impassable terrain that has been generated
                        if (!terrainDict.Any(element => !element.Key.IsPassable && element.Value[i, j] != null))
                        {
                            // Copy cell reference to denote a NON-blocked cell
                            terrainMaskedGrid[i, j] = grid[i, j];
                        }
                    }
                }
            }

            return terrainMaskedGrid;
        }

        /// <summary>
        /// Expands regions of the terrain blocked grid using the base grid. Re-creates terrain layers accordingly.
        /// </summary>
        /// <param name="grid">Cell grid before terrain cells were removed</param>
        /// <param name="terrainMaskedGrid">Cell grid with cells removed where there is impassible terrain</param>
        /// <param name="baseRegions">Regions calculated before laying the terrain</param>
        private void ExpandInvalidRegions(GridCellInfo[,] grid,
                                          GridCellInfo[,] terrainMaskedGrid,
                                          Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict)
        {
            var regions = terrainMaskedGrid.IdentifyRegions(cell => !cell.IsWall);

            // Look for invalid regions and modify them using the base regions
            foreach (var invalidRegion in regions.Where(region => !RegionValidator.ValidateRoomRegion(region)))
            {
                var regionCells = new List<GridCellInfo>(invalidRegion.Locations);

                // Expand region randomly until minimum size is reached
                while (regionCells.Count < RegionValidator.MinimumRoomSize)
                {
                    // Get cells in the specified direction for all the region cells - checking the base region to see where it was so that
                    // it doesn't grow into a corridor.
                    var expandedLocations = regionCells.SelectMany(location => grid.GetAdjacentElementsWithCardinalConnection(location.Column, location.Row))
                                                       .Where(cell => !cell.IsWall)
                                                       .Actualize();

                    if (expandedLocations.Count() == 0)
                        throw new Exception("Trying to expand region of terrian masked grid without any free adjacent cells");

                    // Select those that aren't part of the region cells already. Add these to the region cells
                    foreach (var cell in expandedLocations)
                    {
                        if (!regionCells.Contains(cell))
                        {
                            // Add to the list of expanded cells for the invalid region
                            regionCells.Add(cell);

                            // This cell is no longer going to be impassible - go ahead and mark terrain accordingly
                            terrainMaskedGrid[cell.Location.Column, cell.Location.Row] = grid[cell.Location.Column, cell.Location.Row];

                            // Check each terrain layer grid
                            foreach (var element in terrainDict)
                            {
                                // Remove cells for any impassible layers
                                if (!element.Key.IsPassable)
                                    element.Value[cell.Location.Column, cell.Location.Row] = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes regions of the terrain blocked grid and the base grid that are no longer big enough.
        /// </summary>
        /// <param name="grid">Cell grid before terrain cells were removed</param>
        /// <param name="terrainMaskedGrid">Cell grid with cells removed where there is impassible terrain</param>
        /// <param name="baseRegions">Regions calculated before laying the terrain</param>
        private void RemoveInvalidRegions(GridCellInfo[,] grid,
                                          GridCellInfo[,] terrainMaskedGrid,
                                          Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict)
        {
            var regions = terrainMaskedGrid.IdentifyRegions(cell => !cell.IsWall);

            // Look for invalid regions and remove them
            foreach (var invalidRegion in regions.Where(region => !RegionValidator.ValidateRoomRegion(region)))
            {
                // REMOVE ALL CELLS FROM THE BASE GRID, TERRAIN MASKED GRID, AND ALL TERRAIN GRIDS
                foreach (var location in invalidRegion.Locations)
                {
                    grid[location.Column, location.Row] = null;
                    terrainMaskedGrid[location.Column, location.Row] = null;

                    foreach (var terrainGrid in terrainDict.Values)
                        terrainGrid[location.Column, location.Row] = null;
                }
            }
        }

        /// <summary>
        /// Transfers any new cell references from the terrain masked grid to the primary grid. Also, eliminates cell references from the 
        /// terrain grids where they were marked impassible.
        /// </summary>
        /// <param name="grid">The primary grid</param>
        /// <param name="terrainMaskedGrid">The terrain masked grid that was used to create corridors</param>
        /// <param name="terrainDict">The grids that are used to create the terrain layers</param>
        private void TransferCorridors(GridCellInfo[,] terrainMaskedGrid, GridCellInfo[,] grid, Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (terrainMaskedGrid[i, j] != null)
                    {
                        // These references should already be set unless there are new corridor cells in the terrain masked grid
                        grid[i, j] = terrainMaskedGrid[i, j];

                        foreach (var element in terrainDict)
                        {
                            // Check to see that the layer is impassible - then remove this cell from the terrain layer
                            if (!element.Key.IsPassable)
                                element.Value[i, j] = null;
                        }
                    }
                }
            }
        }
    }
}
