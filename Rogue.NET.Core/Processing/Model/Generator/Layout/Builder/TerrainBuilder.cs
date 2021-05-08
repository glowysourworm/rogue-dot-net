using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

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

        const int TERRAIN_PADDING = 2;

        [ImportingConstructor]
        public TerrainBuilder(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }

        public bool BuildTerrain(LayoutContainer container, LayoutTemplate template)
        {
            // Procedure
            //
            // 1) Generate all layers in order for this layout as separate 2D arrays
            // 2) Create masked grid from the primary grid removing impassible cells
            // 3) Re-identify regions to create new base layer
            // 4) Finally, finalize the terrain layers and the room layer
            //

            // Create all terrain layers in order and separate them by logical layers (see LayoutTerrainLayer enum)
            CreateTerrain(container, template);

            // (Terrain Initial Clean-up) Remove non-overlapping terrain - (Created when there's no "Empty Space" mask)
            RemoveTerrainIslands(container);

            // Create combined terrain-blocked grid. This will have null cells where impassible terrain exists.
            // var terrainMaskedGrid = CreateTerrainMaskedGrid(container.Grid, terrainDict);

            // Check for invalid regions and remove them - put null cells in to be filled with walls or paths
            // RemoveInvalidRegions(terrainMaskedGrid, terrainDict);

            // Create the terrain layers 
            //var terrainLayers = terrainDict.Select(element =>
            //{
            //    // First, identify terrain regions for this layer
            //    var regions = element.Value.ConstructRegions(cell => true);

            //    // Return new layer info
            //    return new LayerInfo<GridCellInfo>(element.Key.Name, regions, element.Key.IsPassable);

            //}).Actualize();

            // Modify the masked grid to include cells for the new terrain (if not already there)
            //terrainMaskedGrid.Iterate((column, row) =>
            //{
            //    var cell = terrainMaskedGrid[column, row];

            //    // Nothing to do
            //    if (cell != null)
            //        return;

            //    var isDefined = terrainLayers.Any(layer => layer.IsDefined(column, row));
            //    var isPassable = terrainLayers.Any(layer => layer.IsDefined(column, row) && layer.IsPassable);

            //    // If layer defined - create supporting cell for the primary grid
            //    if (isDefined)
            //    {
            //        terrainMaskedGrid[column, row] = new GridCellInfo(column, row)
            //        {
            //            // TODO: CREATING WALLS FOR IMPASSIBLE TERRAIN
            //            IsWall = !isPassable
            //        };
            //    }
            //});

            //// Finally, create masked regions - THESE CONTAIN ORIGINAL REGION AND / OR CORRIDOR CELLS
            //var modifiedRegions = terrainMaskedGrid.ConstructRegions(cell => !cell.IsWall);

            //// Check that there are valid regions -> DEFAULT RETURNS FAILED
            //if (!modifiedRegions.Any(region => RegionValidator.ValidateBaseRegion(region)))
            //{
            //    return false;
            //}

            //// SETUP NEW TERRAIN MASKED GRID, NEW BASE REGIONS, AND TERRAIN LAYERS IN THE CONTAINER
            //container.SetTerrain(terrainMaskedGrid, modifiedRegions, terrainLayers);

            return true;
        }

        /// <summary>
        /// Creates terrain layers as 2D cell info array from the input primary grid and the template.
        /// </summary>
        private void CreateTerrain(LayoutContainer container, LayoutTemplate template)
        {
            // Use the layer parameter to order the layers
            foreach (var terrain in template.TerrainLayers.OrderBy(layer => layer.TerrainLayer.Layer))
            {
                // Create terrain layer
                container.CreateTerrainLayer(terrain.TerrainLayer);

                switch (terrain.GenerationType)
                {
                    case TerrainGenerationType.PerlinNoise:
                        {
                            _noiseGenerator.Run(NoiseType.PerlinNoise,
                                                container.Width,
                                                container.Height,
                                                terrain.Frequency,
                                                new PostProcessingCallback(
                            (column, row, value) =>
                            {
                                // Leave padding around the edge
                                if (column < TERRAIN_PADDING ||
                                    row < TERRAIN_PADDING ||
                                    column + TERRAIN_PADDING >= container.Width ||
                                    row + TERRAIN_PADDING >= container.Height)
                                    return 0;

                                // Translate from [0,1] fill ratio to the [-1, 1] Perlin noise range
                                //
                                if (value < ((2 * terrain.FillRatio) - 1))
                                {
                                    // Check the cell's terrain layers for other entries
                                    if (!container.TerrainDefinitions.Any(definition =>
                                    {
                                        // Terrain layer already present
                                        return container.HasTerrain(definition, column, row) &&

                                               // Other terrain layers at this location exclude this layer
                                               (definition.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                               // Other terrain layers at this location overlay this layer
                                               (definition.LayoutType == TerrainLayoutType.Overlay &&
                                                definition.Layer > terrain.TerrainLayer.Layer));
                                    }))
                                    {
                                        // APPLY TERRAIN MASK - Also, remove walls / corridors appropriately
                                        //

                                        // Regions
                                        if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Regions) &&
                                                  container.Get(column, row) != null &&
                                                  !container.Get(column, row).IsWall &&
                                                  !container.Get(column, row).IsCorridor)
                                        {
                                            // No Region mask applied - so go ahead and create the terrain
                                            container.SetTerrain(terrain.TerrainLayer, column, row, true);
                                        }

                                        // Corridors - PASSABLE TERRAIN ONLY
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Corridors) &&
                                                  container.Get(column, row) != null &&
                                                  container.Get(column, row).IsCorridor &&
                                                  terrain.TerrainLayer.IsPassable)
                                        {
                                            // No Corridor mask applied - so go ahead and create the terrain
                                            container.SetTerrain(terrain.TerrainLayer, column, row, true);
                                        }

                                        // Walls
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.Walls) &&
                                                  container.Get(column, row) != null &&
                                                  container.Get(column, row).IsWall)
                                        {
                                            // No Wall mask applied - so go ahead and create the terrain
                                            container.SetTerrain(terrain.TerrainLayer, column, row, true);
                                        }

                                        // Empty Space
                                        else if (!terrain.TerrainLayer.MaskingType.Has(TerrainMaskingType.EmptySpace) &&
                                                  container.Get(column, row) == null)
                                        {
                                            // No Empty Space mask applied - so go ahead and create the terrain in the new cell
                                            container.SetTerrain(terrain.TerrainLayer, column, row, true);
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
            }
        }

        /// <summary>
        /// Removes terrain islands from the terrain grids using 4-way adjacnecy check with the base regions - looks for a non-null base region cell. Also,
        /// modifies the base grid in case the terrain added cells to the grid
        /// </summary>
        private void RemoveTerrainIslands(LayoutContainer container)
        {
            foreach (var definition in container.TerrainDefinitions)
            {
                var regions = container.ConstructTerrainRegions(definition, (column, row) => container.HasTerrain(definition, column, row));

                foreach (var region in regions)
                {
                    var foundRegion = false;

                    // Look for 4-way adjacent cells to edge locations. One must be non-null in a base region.
                    //
                    foreach (var location in region.EdgeLocations)
                    {
                        // Condition for keeping the terrain region
                        if (container.GetCardinalAdjacentElements(location.Column, location.Row)
                                     .Any(cell => container.Get(location.Column, location.Row) != null))
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
                            container.SetTerrain(definition, islandLocation.Column, islandLocation.Row, false);
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
        /// Removes regions of the terrain blocked grid and the base grid that are no longer big enough.
        /// </summary>
        /// <param name="grid">Cell grid before terrain cells were removed</param>
        /// <param name="terrainMaskedGrid">Cell grid with cells removed where there is impassible terrain</param>
        /// <param name="baseRegions">Regions calculated before laying the terrain</param>
        private void RemoveInvalidRegions(GridCellInfo[,] terrainMaskedGrid,
                                          Dictionary<TerrainLayerTemplate, GridCellInfo[,]> terrainDict)
        {
            var regions = terrainMaskedGrid.ConstructRegions(cell => !cell.IsWall);

            // Look for invalid regions and remove them
            foreach (var invalidRegion in regions.Where(region => !RegionValidator.ValidateBaseRegion(region)))
            {
                // REMOVE ALL CELLS FROM TERRAIN MASKED GRID, AND ALL TERRAIN GRIDS
                foreach (var location in invalidRegion.Locations)
                {
                    terrainMaskedGrid[location.Column, location.Row] = null;

                    foreach (var terrainGrid in terrainDict.Values)
                        terrainGrid[location.Column, location.Row] = null;
                }
            }
        }
    }
}
