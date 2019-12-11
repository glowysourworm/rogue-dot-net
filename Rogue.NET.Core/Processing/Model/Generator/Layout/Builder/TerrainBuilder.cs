using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
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
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly INoiseGenerator _noiseGenerator;
        readonly IRegionValidator _regionValidator;
        readonly IConnectionBuilder _connectionBuilder;

        [ImportingConstructor]
        public TerrainBuilder(IRandomSequenceGenerator randomSequenceGenerator, INoiseGenerator noiseGenerator, IRegionValidator regionValidator, IConnectionBuilder connectionBuilder)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _noiseGenerator = noiseGenerator;
            _regionValidator = regionValidator;
            _connectionBuilder = connectionBuilder;
        }

        public bool BuildTerrain(GridCellInfo[,] grid, IEnumerable<Region> regions, LayoutTemplate template, out LayerInfo roomLayer, out IEnumerable<LayerInfo> terrainLayers)
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
            var terrainDict = CreateTerrain(grid, template);

            // Create combined terrain-blocked grid. This will have null cells where impassible terrain exists.
            var terrainMaskedGrid = CreateTerrainMaskedGrid(grid, terrainDict);

            // Check for invalid regions and expand them as needed
            ExpandInvalidRegions(grid, terrainMaskedGrid, terrainDict);

            // Create corridors and new regions
            var finalRegions = _connectionBuilder.BuildConnections(terrainMaskedGrid, template);

            // Transfer the corridor cells back to the primary and terrain grids
            TransferCorridors(terrainMaskedGrid, grid, terrainDict);

            // Create the resulting room layer
            roomLayer = new LayerInfo("Room Layer", finalRegions);

            // Create the terrain layers
            terrainLayers = terrainDict.Select(element => new LayerInfo(element.Key.Name, element.Value.IdentifyRegions()))
                                       .Actualize();

            return true;
        }

        /// <summary>
        /// Creates terrain layers as 2D cell info array from the input primary grid and the template.
        /// </summary>
        private Dictionary<TerrainLayerTemplate, GridCellInfo[,]> CreateTerrain(GridCellInfo[,] grid, LayoutTemplate template)
        {
            var terrainDict = new Dictionary<TerrainLayerTemplate, GridCellInfo[,]>();

            // Use the ZOrder parameter to order the layers
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
                                // Translate from [0,1] fill ration to the [-1, 1] Perlin noise range
                                //
                                if (value < ((2 * terrain.FillRatio) - 1) &&
                                    grid[column, row] != null)
                                {
                                    // Check the cell's terrain layers for other entries
                                    if (!terrainDict.Any(element => 
                                    {
                                        // Terrain layer already present
                                        return element.Value[column, row] != null &&

                                               // Other terrain layers at this location don't exclude this layer at the same location
                                               (element.Key.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                               // Other terrain layers at this location DO exclude other terrain; but not at this layer
                                               (element.Key.LayoutType == TerrainLayoutType.LayerExclusive &&
                                                element.Key.Layer == terrain.TerrainLayer.Layer));

                                    }))
                                    {
                                        // Keep track of the terrain cells using the grid
                                        terrainGrid[column, row] = grid[column, row];
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
                        if (!terrainDict.Any(element => !element.Key.IsPassable && element.Value[i,j] != null))
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
            var regions = terrainMaskedGrid.IdentifyRegions();

            // Look for invalid regions and modify them using the base regions
            foreach (var invalidRegion in regions.Where(region => !_regionValidator.ValidateRoomRegion(region)))
            {
                var regionCells = new List<GridLocation>(invalidRegion.Cells);

                // Expand region randomly until minimum size is reached
                while (regionCells.Count < _regionValidator.MinimumRoomSize)
                {
                    // Get cells in the specified direction for all the region cells - checking the base region to see where it was so that
                    // it doesn't grow into a corridor.
                    var expandedCells = regionCells.SelectMany(location => grid.GetAdjacentElementsWithCardinalConnection(location.Column, location.Row))
                                                   .Where(cell => !cell.IsWall)
                                                   .Actualize();

                    if (expandedCells.Count() == 0)
                        throw new Exception("Trying to expand region of terrian masked grid without any free adjacent cells");

                    // Select those that aren't part of the region cells already. Add these to the region cells
                    foreach (var cell in expandedCells)
                    {
                        if (!regionCells.Contains(cell.Location))
                        {
                            // Add to the list of expanded cells for the invalid region
                            regionCells.Add(cell.Location);

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
