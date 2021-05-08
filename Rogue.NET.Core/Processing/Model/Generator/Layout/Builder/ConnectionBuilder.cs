using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Component;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
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

        [ImportingConstructor]
        public ConnectionBuilder(IMazeRegionCreator mazeRegionCreator)
        {
            _mazeRegionCreator = mazeRegionCreator;
        }

        public void BuildConnections(LayoutContainer container, LayoutTemplate template, GraphInfo<GridCellInfo> connectionGraph)
        {
            BuildConnectionsWithAvoidRegions(container, template, connectionGraph);
        }

        public void BuildConnectionsWithAvoidRegions(LayoutContainer container,
                                                     LayoutTemplate template,
                                                     GraphInfo<GridCellInfo> connectionGraph)
        {
            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                    ConnectUsingShortestPath(container, connectionGraph);
                    break;
                case LayoutConnectionType.ConnectionPoints:
                    throw new Exception("Connection points aren't calculated in the connection builder. These are triangulated into the connection layer");
                case LayoutConnectionType.Maze:
                    throw new Exception("Maze layout types need to be handled using specific calls to CreateMazeCorridors AND ");
                default:
                    throw new Exception("Unhandled Connection Type");
            }
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        public void CreateMazeCorridors(LayoutContainer container,
                                        LayoutTemplate template,
                                        MazeType mazeType)
        {
            // Procedure
            //
            // - Make a set of empty space regions for the maze generator
            // - Calculate room, and impassable terrain region sets
            // - Create mazes for each empty space region - AVOIDING ROOMS AND IMPASSABLE TERRAIN
            // - Apply the resulting 2D boolean array to the layout - adding new cells where TRUE
            //

            // Add walls in the negative space - leaving room for region cells
            var emptySpaceRegions = container.ConstructRegions((column, row) => container.Get(column, row) == null);

            // Fetch all other regions in the layout that are walkable
            var roomRegions = container.GetRegions(LayoutGrid.LayoutLayer.Room);
            var corridorRegions = container.GetRegions(LayoutGrid.LayoutLayer.Corridor);
            var impassableRegions = container.GetRegions(LayoutGrid.LayoutLayer.ImpassableTerrain);

            // CORRIDORS HAVE ALREADY BEEN CALCULATED!
            if (corridorRegions.Any())
                throw new Exception("Trying to re-calculate corridors:  ConnectionBuilder.CreateMazeCorridors");

            // Fill empty regions with recrusive-backtracked corridors
            //
            foreach (var region in emptySpaceRegions)
            {
                // Create the maze!
                //
                var mazeGrid = _mazeRegionCreator.CreateMaze(container.Width, container.Height, region,
                                                             roomRegions.Union(corridorRegions),
                                                             mazeType, template.MazeWallRemovalRatio,
                                                             template.MazeHorizontalVerticalBias);

                // Copy the new corridor data back to the layout
                mazeGrid.Iterate((column, row) =>
                {
                    // Outside the empty space region
                    if (region[column, row] == null)
                        return;

                    var cell = container.Get(column, row);

                    // TRUE => CORRIDOR
                    if (mazeGrid[column, row])
                    {
                        // New cell
                        if (cell == null)
                            container.SetLayout(column, row, new GridCellInfo(column, row)
                            {
                                IsCorridor = true
                            });

                        // Existing cell -> Remove the wall setting
                        else
                            throw new Exception("Trying to overwrite an existing layout cell:  ConnectionBuilder.CreateMazeCorridors");
                        //{
                        //    cell.IsWall = false;
                        //    cell.IsCorridor = true;
                        //}
                    }
                });
            }
        }

        /// <summary>
        /// Connects layout based on the pre-calculated triangulation and regions
        /// </summary>
        public void ConnectUsingShortestPath(LayoutContainer container, GraphInfo<GridCellInfo> connectionGraph)
        {
            // Pre-fetch impassable regions. These are invalidated during the connection process
            var impassableRegions = container.GetRegions(LayoutGrid.LayoutLayer.ImpassableTerrain);

            // For each edge in the triangulation - create a corridor
            //
            foreach (var connection in connectionGraph.Connections)
            {
                // Create a Dijkstra path generator to find paths for the edge
                var dijkstraMap = new DijkstraPathGenerator(container.Width, container.Height, impassableRegions,
                                                            connection.Location, new GridCellInfo[] { connection.AdjacentLocation }, true,

                                                            // Main Callback for Dijkstra's Algorithm to fetch cells
                                                            (column, row) =>
                                                            {
                                                                // NOTE*** THIS CALLBACK IS BEING USED TO CREATE NEW CELLS FOR
                                                                //         EMBEDDING A PATH FOR THE LAYOUT THAT CAN GO OFF INTO
                                                                //         UN-USED GRID LOCATIONS.
                                                                return container.Get(column, row) ?? new GridCellInfo(column, row);
                                                            });

                var pathCells = new List<GridCellInfo>();

                // NOTE*** EMBED PATH CELLS USING CALLBACK. THESE MAY-OR-MAY-NOT YET BE A PART
                //         OF THE LAYOUT
                dijkstraMap.CalculatePaths(new DijkstraPathGenerator.DijkstraPathCallback(cell =>
                {
                    // THESE CELLS MAY NOT YET BE PART OF THE LAYOUT GRID
                    cell.IsCorridor = true;

                    // WALL SETTING IS APPLIED DURING LAYOUT FINALIZATION. THIS NEEDS TO BE
                    // RE-DESIGNED!!
                    cell.IsWall = false;

                    // Invalidates the layout
                    container.SetLayout(cell.Location.Column, cell.Location.Row, cell);

                    pathCells.Add(cell);
                }));

                // Path tracing is done from TARGET -> SOURCE
                if (pathCells.Count > 1)
                {
                    // Doors are the final cell before entering a room
                    connection.DoorLocation = pathCells[pathCells.Count - 2];
                    connection.AdjacentDoorLocation = pathCells[1];
                }
                else if (pathCells.Count == 1)
                {
                    connection.DoorLocation = pathCells[0];
                    connection.AdjacentDoorLocation = pathCells[0];
                }
                else
                    throw new Exception("Invalid path found between two regions:  ConnectionBuilder.ConnectUsingShortestPath");

                // Store corridor for reference
                connection.CorridorLocations = pathCells;
            }
        }
    }
}
