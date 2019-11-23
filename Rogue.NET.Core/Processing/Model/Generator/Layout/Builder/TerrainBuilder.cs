using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
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

        [ImportingConstructor]
        public TerrainBuilder(IRandomSequenceGenerator randomSequenceGenerator, INoiseGenerator noiseGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _noiseGenerator = noiseGenerator;
        }

        public IEnumerable<LayerInfo> BuildTerrain(GridCellInfo[,] grid, LayoutTemplate template, out LayerInfo roomLayer)
        {
            // Procedure
            //
            // 1) Generate all layers in order for this layout as separate 2D arrays
            // 2) Identify terrain regions and set up cell infos
            //

            var terrainDict = new Dictionary<GridCellInfo[,], TerrainLayerTemplate>();

            // Use the ZOrder parameter to order the layers
            foreach (var terrain in template.TerrainLayers.OrderBy(layer => layer.TerrainLayer.Layer))
            {
                // Generate terrain layer randomly based on the weighting
                if (_randomSequenceGenerator.Get() > terrain.GenerationWeight)
                    continue;

                // Create a new grid for each terrain layer
                var terrainLayerGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];

                // Store the grid with the associated terrain layer
                terrainDict.Add(terrainLayerGrid, terrain.TerrainLayer);

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
                                // Translate from [-1, 1] -> [0, 1] to check fill ratio
                                if ((System.Math.Abs(value) / 2.0) < terrain.FillRatio &&
                                    grid[column, row] != null)
                                {
                                    // Check the terrain dictionary for other entries
                                    if (!terrainDict.Any(element =>
                                    {
                                        // None of the grids have terrain at this location
                                        return element.Key[column, row] != null &&

                                               // Other terrain layers at this location don't exclude this layer at the same location
                                               (element.Value.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                               // Other terrain layers at this location DO exclude other terrain; but not at this layer
                                               (element.Value.LayoutType == TerrainLayoutType.LayerExclusive &&
                                                element.Value.Layer == terrain.TerrainLayer.Layer));

                                    }))
                                    {
                                        // Add to the region
                                        terrainLayerGrid[column, row] = grid[column, row];
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

            // Detect new room regions
            var terrainBlockedGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];
            var terrainBlockedInputMap = new double[grid.GetLength(0), grid.GetLength(1)];
            var foundBlockedCell = false;

            // Create new grid with removed cells for blocked terrain
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Found a grid cell - check for blocking terrain
                    if (grid[i, j] != null)
                    {
                        // Check for any impassable terrain that has been generated
                        if (!terrainDict.Any(element => !element.Value.IsPassable && element.Key[i, j] != null))
                        {
                            // Copy cell reference
                            terrainBlockedGrid[i, j] = grid[i, j];
                        }

                        // DON'T copy cell reference -> flag the blocked cell -> set Dijkstra weight to large number
                        else
                        {
                            // FLAG BLOCKED CELL TO PREVENT EXTRA WORK IF NOT NEEDED
                            foundBlockedCell = true;

                            // Block off the tile on the Dijkstra input map
                            terrainBlockedInputMap[i, j] = 10000;
                        }
                    }
                }
            }

            // Generate the new room regions
            var roomRegions = terrainBlockedGrid.IdentifyRegions();

            // Set up the new room layer
            roomLayer = new LayerInfo("Room Layer", roomRegions);

            if (foundBlockedCell)
            {
                // Create MST for the rooms
                var roomGraph = GeometryUtility.PrimsMinimumSpanningTree(roomRegions, Metric.MetricType.Roguian);

                // For each edge in the triangulation - create a corridor
                foreach (var edge in roomGraph.Edges)
                {
                    var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);
                    var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);

                    // Creates Dijkstra map from the input map to find paths along the edges
                    var dijkstraMap = new DijkstraMap(terrainBlockedInputMap, location1, location2);

                    dijkstraMap.Run();

                    // Generate Path locations
                    var path = dijkstraMap.GeneratePath(true);

                    // Add path to the grid
                    foreach (var location in path)
                        grid[location.Column, location.Row] = new GridCellInfo(location) { IsWall = false };
                }
            }

            var terrainLayers = new List<LayerInfo>();

            // Identify Terrain Regions
            foreach (var element in terrainDict)
            {
                // Sets up a set of terrain regions for the specified layer
                terrainLayers.Add(new LayerInfo(element.Value.Name, element.Key.IdentifyRegions()));
            }

            return terrainLayers;
        }
    }
}
