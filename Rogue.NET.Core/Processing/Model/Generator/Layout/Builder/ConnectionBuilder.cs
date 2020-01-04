using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Algorithm.Component;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IConnectionBuilder))]
    public class ConnectionBuilder : IConnectionBuilder
    {
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRegionTriangulationCreator _regionTriangulationCreator;

        [ImportingConstructor]
        public ConnectionBuilder(IMazeRegionCreator mazeRegionCreator,
                                 IRandomSequenceGenerator randomSequenceGenerator,
                                 IRegionTriangulationCreator regionTriangulationCreator)
        {
            _mazeRegionCreator = mazeRegionCreator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _regionTriangulationCreator = regionTriangulationCreator;
        }

        public void BuildConnections(LayoutContainer container, LayoutTemplate template)
        {
            BuildConnectionsWithAvoidRegions(container, template, new Region<GridCellInfo>[] { });
        }

        public void BuildConnectionsWithAvoidRegions(LayoutContainer container,
                                                      LayoutTemplate template,
                                                      IEnumerable<Region<GridCellInfo>> avoidRegions)
        {
            if (!PreValidateRegions(container.BaseRegions))
                throw new Exception("Invalid region layout in the grid - ConnectionBuilder.BuildCorridorsWithAvoidRegions");

            if (!PreValidateRegions(container.ConnectionLayer.ConnectionRegions))
                throw new Exception("Invalid region layout in the grid - ConnectionBuilder.BuildCorridorsWithAvoidRegions");

            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                    ConnectUsingShortestPath(container, avoidRegions);
                    break;
                case LayoutConnectionType.ConnectionPoints:
                    throw new Exception("Connection points aren't calculated in the connection builder. These are triangulated into the connection layer");
                case LayoutConnectionType.Maze:
                    {
                        // Use "Filled" rule for rectangular regions only. "Open" maze rule works better with non-rectangular regions.
                        switch (template.Type)
                        {
                            case LayoutType.RectangularRegion:
                            case LayoutType.RandomRectangularRegion:
                            case LayoutType.CellularAutomataMap:
                            case LayoutType.ElevationMap:
                            case LayoutType.RandomSmoothedRegion:
                                CreateMazeCorridors(container, template, avoidRegions, MazeType.Filled);
                                break;
                            case LayoutType.MazeMap:
                            case LayoutType.CellularAutomataMazeMap:
                            case LayoutType.ElevationMazeMap:
                            default:
                                throw new Exception("Unhandled or Unsupported Layout Type for maze connections");
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled Connection Type");
            }
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        private void CreateMazeCorridors(LayoutContainer container,
                                          LayoutTemplate template,
                                          IEnumerable<Region<GridCellInfo>> avoidRegions,
                                          MazeType mazeType)
        {
            // Procedure
            //
            // - (Pre-Validated) Identify regions to pass to the maze generator (avoids these when removing walls)
            // - Fill in empty cells with walls
            // - Create mazes where there are 8-way walls surrounding a cell
            // - Since maze generator removes walls (up to the edge of the avoid regions)
            //   Must add back just those walls adjacent to the avoid regions
            //
            // - Then, BuildCorridors(...) will connect any mazes together that were interrupted
            //   (Also, by the adding back of walls)
            //

            container.Grid.Iterate((column, row) =>
            {
                // Add walls in the negative space - leaving room for region cells
                if (container.Grid[column, row] == null)
                    container.Grid[column, row] = new GridCellInfo(column, row) { IsWall = true, IsCorridor = false };
            });

            // Create a recursive-backtrack corridor in cell that contains 8-way walls. Continue until entire map is
            // considered.

            // Find empty regions and fill them with recrusive-backtracked corridors
            //
            // NOTE*** Avoiding edges that WERE created as walls - because the "Filled" rule won't touch those.
            //
            for (int i = 1; i < container.Grid.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < container.Grid.GetLength(1) - 1; j++)
                {
                    // Create a corridor where all adjacent cells are walls
                    if (container.Grid.GetAdjacentElements(i, j).All(cell => cell.IsWall))
                    {
                        // Avoid both base regions and any additional avoid regions (impassable terrain)
                        var allAvoidedRegions = container.BaseRegions.Union(avoidRegions);

                        // Create the maze!
                        _mazeRegionCreator.CreateCellsStartingAt(container.Grid,
                                                                 allAvoidedRegions,
                                                                 container.Grid[i, j].Location,
                                                                 mazeType, template.MazeWallRemovalRatio,
                                                                 template.MazeHorizontalVerticalBias);
                    }
                }
            }

            // Add back walls surrounding the regions
            foreach (var region in container.BaseRegions)
            {
                foreach (var edgeLocation in region.EdgeLocations)
                {
                    var adjacentCells = container.Grid.GetAdjacentElements(edgeLocation.Column, edgeLocation.Row);

                    foreach (var cell in adjacentCells)
                    {
                        // Adjacent cell is not in region; but is on the edge. This should
                        // be a wall cell
                        if (region[cell.Location.Column, cell.Location.Row] == null)
                        {
                            cell.IsWall = true;
                            cell.IsCorridor = false;
                        }
                    }
                }
            }

            // *** GRAPH BASE REGIONS HAVE NOW BEEN MODIFIED - MUST CALCULATE THE CONNECTION LAYER AND
            //     STORE AS THE "MODIFIED" REGIONS.

            // Re-identify regions to connect
            var modifiedRegions = container.Grid.ConstructConnectedRegions(cell => !cell.IsWall);

            // Finally, connect the MODIFIED regions using shortest path
            var modifiedGraph = _regionTriangulationCreator.CreateTriangulation(modifiedRegions, template);

            // RESET CONNECTION LAYER
            container.SetConnectionLayer(modifiedRegions, modifiedGraph);

            // Create corridors between any disconnected maze regions
            ConnectUsingShortestPath(container, avoidRegions);
        }

        /// <summary>
        /// Connects the CONNECTION layer based on the pre-calculated triangulation and regions
        /// </summary>
        private void ConnectUsingShortestPath(LayoutContainer container,
                                              IEnumerable<Region<GridCellInfo>> avoidRegions)
        {
            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in container.ConnectionLayer.RegionGraph.Edges)
            {
                var region1 = container.ConnectionLayer.ConnectionRegions.First(region => region.Id == edge.Point1.ReferenceId);
                var region2 = container.ConnectionLayer.ConnectionRegions.First(region => region.Id == edge.Point2.ReferenceId);

                region1.CalculateConnection(region2, _randomSequenceGenerator);

                var connection = region1.GetConnection(region2);

                var location1 = connection.Location;
                var location2 = connection.AdjacentLocation;

                // Create a Dijkstra path generator to find paths for the edge
                var dijkstraMap = new DijkstraPathGenerator(container.Grid, avoidRegions, location1, new GridCellInfo[] { location2 }, true);

                // Embed path cells using callback to set properties
                dijkstraMap.EmbedPaths(new DijkstraPathGenerator.DijkstraEmbedPathCallback(cell =>
                {
                    cell.IsWall = false;
                    cell.IsCorridor = true;
                }));
            }
        }

        private bool PreValidateRegions(IEnumerable<Region<GridCellInfo>> regions)
        {
            // Validate room regions
            var invalidRoomRegions = regions.Where(region => !RegionValidator.ValidateBaseRegion(region));

            // Check for a valid room region
            if (invalidRoomRegions.Count() > 0)
                return false;

            return true;
        }
    }
}
