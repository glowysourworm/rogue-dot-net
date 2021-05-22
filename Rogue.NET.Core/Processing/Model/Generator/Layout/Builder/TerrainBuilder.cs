using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;
using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    /// <summary>
    /// NOTE*** State of terrain stored in this component for use by the LayoutContainer.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ITerrainBuilder))]
    public class TerrainBuilder : ITerrainBuilder
    {
        readonly INoiseGenerator _noiseGenerator;

        const int TERRAIN_PADDING = 2;

        // *** TERRAIN STATE -> Stores ORIGINAL terrain grids. These are processed to
        //                      create the final regions.
        IDictionary<TerrainLayerTemplate, GridLocation[,]> _terrainDict;

        int _width;
        int _height;

        [ImportingConstructor]
        public TerrainBuilder(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
            _terrainDict = new Dictionary<TerrainLayerTemplate, GridLocation[,]>();
            _width = 0;
            _height = 0;
        }

        /// <summary>
        /// Creates terrain layers as 2D cell info array from the input primary grid and the template.
        /// </summary>
        public void Initialize(int width, int height, IEnumerable<TerrainLayerGenerationTemplate> layers)
        {
            _width = width;
            _height = height;

            foreach (var template in layers)
            {
                // Use the layer parameter to order the layers
                var terrainGrid = new GridLocation[width, height];

                switch (template.GenerationType)
                {
                    case TerrainGenerationType.PerlinNoise:
                        {
                            _noiseGenerator.Run(NoiseType.PerlinNoise,
                                                width,
                                                height,
                                                template.Frequency,
                                                new PostProcessingCallback(
                            (column, row, value) =>
                            {
                                // Leave padding around the edge
                                if ((column < TERRAIN_PADDING) ||
                                    (row < TERRAIN_PADDING) ||
                                    ((column + TERRAIN_PADDING) >= width) ||
                                    ((row + TERRAIN_PADDING) >= height))
                                    return 0;

                                // Translate from [0,1] fill ratio to the [-1, 1] Perlin noise range
                                //
                                if (value < ((2 * template.FillRatio) - 1))
                                {
                                    // Check the cell's terrain layers for other entries
                                    if (!_terrainDict.Any(layer =>
                                    {

                                        // Terrain layer already present
                                        return layer.Value[column, row] != null &&

                                           // Other terrain layers at this location exclude this layer
                                           (layer.Key.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                           // Other terrain layers at this location overlay this layer
                                           (layer.Key.LayoutType == TerrainLayoutType.Overlay &&
                                            layer.Key.Layer > template.TerrainLayer.Layer));
                                    }))
                                    {
                                        // Initialize the terrain grids
                                        terrainGrid[column, row] = new GridLocation(column, row);
                                    }
                                }

                                return value;
                            }));
                        }
                        break;
                    default:
                        throw new Exception("Unhandled terrain layer generation type");
                }

                _terrainDict.Add(template.TerrainLayer, terrainGrid);
            }
        }

        public void AddTerrainSupport(LayoutContainer container)
        {
            for (int column = 0; column < _width; column++)
            {
                for (int row = 0; row < _height; row++)
                {
                    foreach (var element in _terrainDict)
                    {
                        if (element.Key.MaskingType.Has(TerrainMaskingType.EmptySpace))
                            continue;

                        var terrainGrid = element.Value;

                        // Add terrain support cells where none exist for any terrain layers - to the layout grid
                        //
                        if (terrainGrid[column, row] != null &&
                            container.Get(column, row) == null)
                        {
                            container.AddLayout(column, row, new GridCellInfo(column, row)
                            {
                                IsTerrainSupport = true
                            });
                        }
                    }
                }
            }
        }

        // Constructs terrain regions based on the original terrain grid - removing islands.
        public Dictionary<TerrainLayerTemplate, IEnumerable<RegionInfo<GridLocation>>> FinalizeTerrainRegions(LayoutContainer container)
        {
            var result = new Dictionary<TerrainLayerTemplate, IEnumerable<RegionInfo<GridLocation>>>();

            foreach (var element in _terrainDict)
            {
                var regionGrid = _terrainDict[element.Key];

                // Create from original terrain grid
                var terrainRegions = regionGrid.ConstructRegions(element.Key.Name, location =>
                {
                    var cell = container.Get(location.Column, location.Row);

                    // Reference the layout grid - TERRAIN SUPPORT MUST BE AVAILABLE
                    if (cell == null)
                        return false;

                    return PassesTerrainMask(cell, element.Key);

                }, location => location).ToList();

                var removedTerrainRegions = new List<RegionInfo<GridLocation>>();

                foreach (var terrainRegion in terrainRegions)
                {
                    var foundLayoutSupport = false;

                    // Search edge locations of the terrain for ONE that is in a FULL region
                    //
                    foreach (var location in terrainRegion.EdgeLocations)
                    {
                        // QUERIES CURRENT STATE OF REGIONS - VALID!
                        if (container.GetRegions(LayoutLayer.FullNoTerrainSupport)
                                     .Any(region => region[location] != null))
                        {
                            foundLayoutSupport = true;
                            break;
                        }
                    }

                    // NO LAYER SUPPORT!
                    if (!foundLayoutSupport)
                    {
                        removedTerrainRegions.Add(terrainRegion);
                    }
                }

                // Remove terrain islands
                foreach (var terrainRegion in removedTerrainRegions)
                {
                    // Remove terrain locations from original grid AND the layout grid
                    foreach (var location in terrainRegion.Locations)
                    {
                        // Nullify terrain grid
                        regionGrid[location.Column, location.Row] = null;

                        // DOUBLE CHECK THAT CELL IS ONLY TERRAIN SUPPORT
                        var cell = container.Get(location.Column, location.Row);

                        if (!cell.IsTerrainSupport)
                            throw new Exception("Trying to remove terrain island from non-support cell:  TerrainBuilder.FinalizeTerrainRegions");

                        // REMOVE LAYOUT CELL
                        container.RemoveLayout(cell);
                    }
                }

                // Add result to the list
                result.Add(element.Key, terrainRegions.Except(removedTerrainRegions));

                // RE-CREATE WALKABLE AND TERRAIN SUPPORT REGIONS
                container.ProcessRegions();
            }

            return result;
        }

        public bool AnyTerrain(int column, int row)
        {
            return _terrainDict.Any(element => element.Value[column, row] != null);
        }

        public bool AnyImpassableTerrain(int column, int row)
        {
            return _terrainDict.Any(element => !element.Key.IsPassable && element.Value[column, row] != null);
        }

        /// <summary>
        /// Returns true if it is safe to apply this terrain type to the layout container
        /// </summary>
        private bool PassesTerrainMask(GridCellInfo cell, TerrainLayerTemplate template)
        {
            if (template.MaskingType.Has(TerrainMaskingType.Corridors))
            {
                if (cell.IsLayer(LayoutLayer.Corridor))
                    return false;
            }

            if (template.MaskingType.Has(TerrainMaskingType.Regions))
            {
                if (cell.IsLayer(LayoutLayer.ConnectionRoom) || cell.IsLayer(LayoutLayer.Room))
                    return false;
            }

            if (template.MaskingType.Has(TerrainMaskingType.EmptySpace))
            {
                // EMPTY SPACE IS EXCLUSIVE TO TERRAIN SUPPORT LAYER
                if (cell.IsLayer(LayoutLayer.TerrainSupport))
                    return false;
            }

            // INCORRECT MASK - BUT THESE CONSTRAINTS ARE ALREADY APPLIED!!!
            //if (!template.IsPassable)
            //{
            //    if (cell.IsLayer(LayoutLayer.Corridor) || cell.IsLayer(LayoutLayer.ConnectionRoom))
            //        return false;
            //}
            
            return true;
        }
    }
}
