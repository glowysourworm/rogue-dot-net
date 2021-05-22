using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
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

        #region (protected) Nested Classes

        /// <summary>
        /// Stores data from an attempted connection run for the connection using shortest path layout
        /// </summary>
        protected class ConnectionInfo
        {
            // Using dictionary for faster hashing
            public Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>> FinishedConnections { get; private set; }

            public Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo> PartialConnectionDict { get; private set; }

            public ConnectionInfo(Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>> finishedConnections,
                                  Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo> partialConnectionDict)
            {
                this.FinishedConnections = finishedConnections;
                this.PartialConnectionDict = partialConnectionDict;
            }

            public static ConnectionInfo CreateEmpty()
            {
                return new ConnectionInfo(new Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>>(),
                                          new Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo>());
            }
        }

        /// <summary>
        /// Stores all data involved with a partial connection
        /// </summary>
        protected class PartialConnectionInfo
        {
            public RegionInfo<GridLocation> Source { get; private set; }
            public RegionConnectionInfo<GridLocation> Connection { get; private set; }

            public RegionInfo<GridLocation> PartialRegion { get; private set; }
            public List<GridLocation> PartialPath { get; private set; }

            /// <summary>
            /// Graph of the original connections with the failed connection removed
            /// </summary>
            public RegionGraphInfo<GridLocation> PartialGraph { get; private set; }
            
            /// <summary>
            /// The route to the destination region MUST BE HAVE CONNECTIONS IN THE ORIGINAL GRAPH.
            /// </summary>
            public GraphTraversal<RegionInfo<GridLocation>, RegionConnectionInfo<GridLocation>> Route { get; private set; }

            /// <summary>
            /// Signals that the connection has been completed
            /// </summary>
            public bool IsRouted
            {
                get { return this.Route != null; }
            }

            public void SetPartial(RegionInfo<GridLocation> partialRegion, List<GridLocation> partialPath)
            {
                this.PartialRegion = partialRegion;
                this.PartialPath = partialPath;
            }

            public void SetRoute(GraphTraversal<RegionInfo<GridLocation>, RegionConnectionInfo<GridLocation>> traversal)
            {
                this.Route = traversal;
            }

            public PartialConnectionInfo(RegionGraphInfo<GridLocation> originalGraph,
                                         RegionInfo<GridLocation> source, 
                                         RegionConnectionInfo<GridLocation> connection)
            {
                this.Source = source;
                this.Connection = connection;

                // Create copies of the graph connection references
                var connections = originalGraph.GetConnections().ToList();

                // REMOVE THE FAILED CONNECTION
                connections.Remove(connection);

                // Create the PARTIAL GRAPH
                this.PartialGraph = new RegionGraphInfo<GridLocation>(connections);
            }
        }


        #endregion

        [ImportingConstructor]
        public ConnectionBuilder(IMazeRegionCreator mazeRegionCreator)
        {
            _mazeRegionCreator = mazeRegionCreator;
        }

        public void BuildConnections(LayoutContainer container, LayoutTemplate template)
        {
            ConnectUsingShortestPath(container);
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        public void CreateMazeRegionsInEmptySpace(LayoutContainer container,
                                                  LayoutTemplate template,
                                                  MazeType mazeType)
        {
            // Procedure
            //
            // - Make a set of empty space regions for the maze generator
            // - Calculate room, and impassable terrain region sets
            // - Create mazes for each empty space region - AVOIDING CONNECTION ROOMS AND IMPASSABLE TERRAIN
            // - Apply the resulting 2D boolean array to the layout - adding new cells where TRUE
            //

            // Create empty space regions from the layout
            var emptySpaceRegions = container.CreateEmptySpaceRegions();

            // Avoid WALKABLE regions - this already includes IMPASSABLE TERRAIN
            var avoidRegions = container.GetRegions(LayoutGrid.LayoutLayer.Walkable);

            // Fill empty regions with recrusive-backtracked corridors
            //
            foreach (var region in emptySpaceRegions)
            {
                // Create the maze!
                //
                var mazeGrid = _mazeRegionCreator.CreateMaze(container.Width, container.Height, region,
                                                            (location) => avoidRegions.Any(region => region[location] != null),
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
                            container.AddLayout(column, row, new GridCellInfo(column, row));

                        // Existing cell -> Remove the wall setting
                        else
                            throw new Exception("Trying to overwrite an existing layout cell:  ConnectionBuilder.CreateMazeCorridors");
                    }
                });
            }
        }

        /// <summary>
        /// Connects layout based on the pre-calculated triangulation and regions
        /// </summary>
        public void ConnectUsingShortestPath(LayoutContainer container)
        {
            // Procedure
            //
            // 1) Try connecting region to its adjacent partner using the connection
            // 2) If any intermediate region
            //       - wait until next pass to see if it becomes connected
            //       - store region sub-graph in a list
            // 3) Re-try using latest sub-graph connection until one is found
            // 4) After connection is found - create entry in region connection "sub-graph" 
            //    to keep track of connection from the primary region.
            // 5) Optionally* - store the list of regions as a path to get to the end result.
            //    

            // NOTE*** THIS HAS TO BE RE-PROCESSED AFTER THE ROUTINE BECAUSE WE'RE FORCING IT 
            //         TO BECOME INVALID WITHOUT CHECKING WITH THE CONTAINER.
            //
            var connectionInfo = ConnectionInfo.CreateEmpty();
            var topologyChanged = false;

            do
            {
                var finishedConnectionCount = connectionInfo.FinishedConnections.Count;

                // Iterate next batch of connections excluding those that are finished
                //
                // CREATES NEW PARTIAL CONNECTIONS!
                //
                var nextConnectionInfo = RouteConnections(container, connectionInfo);

                // Attempt to route partial connections
                nextConnectionInfo = RoutePartialConnections(container, nextConnectionInfo);

                // Detect topology change
                topologyChanged = (finishedConnectionCount != nextConnectionInfo.FinishedConnections.Count);

                // NO SUCCESSFUL PARTIALS -> Finish routing by forcing partials that did not route. 
                if (!topologyChanged)
                    nextConnectionInfo = ForceRoutes(container, nextConnectionInfo);

                // Update the follower
                connectionInfo = nextConnectionInfo;

            } while (topologyChanged);

            // RE-PROCESS TERRAIN SINCE WE INVALIDATED IT HERE.
            container.ProcessRegions();
        }

        // Attempts connections for the entire graph excluding finished ones - using the current 
        // state of the layout graph (CREATES NEW CONNECTION INFO)
        private ConnectionInfo RouteConnections(LayoutContainer container, ConnectionInfo connectionInfo)
        {
            var connectionsToRoute = container.GetConnectionGraph()
                                              .GetConnections()
                                              .Except(connectionInfo.FinishedConnections.Keys)
                                              .ToList();

            var finishedConnections = new Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>>(connectionInfo.FinishedConnections);
            var partialConnections = new Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo>();

            foreach (var connection in connectionsToRoute)
            {
                RegionInfo<GridLocation> partialRegion = RegionInfo<GridLocation>.Empty;
                List<GridLocation> pathCells = null;

                // Successful Connection!
                if (ConnectRegions(container,
                                   connection,
                                   false,
                                   out partialRegion, out pathCells))
                {
                    EmbedPathCells(container, connection, pathCells, false);

                    // Mark this connection as completed
                    finishedConnections.Add(connection, connection);
                }
                // Partial Connection -> Store the result and attempt to connect later
                else
                {
                    var partialConnection = new PartialConnectionInfo(container.GetConnectionGraph(), connection.Node, connection);

                    partialConnection.SetPartial(partialRegion, pathCells);
                    partialConnections.Add(connection, partialConnection);
                }
            }

            return new ConnectionInfo(finishedConnections, partialConnections);
        }

        // Attempts routing PARTIAL connections for the ones that have a successful traversal - using the current 
        // state of the layout graph (CREATES NEW CONNECTION INFO)
        private ConnectionInfo RoutePartialConnections(LayoutContainer container, ConnectionInfo connectionInfo)
        {
            // Create new ConnectionInfo for result
            var finishedConnections = new Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>>(connectionInfo.FinishedConnections);
            var partialConnections = new Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo>(connectionInfo.PartialConnectionDict);

            // Iterate PARTIAL connections where there are new ones to attempt
            foreach (var connection in partialConnections.Where(element => !element.Value.IsRouted))
            {
                // Attempt a connection run from the LAST PARTIAL connected region to the FINAL region
                var traversal = DijkstrasAlgorithm.Run(connection.Value.PartialGraph, connection.Value.PartialRegion, connection.Key.AdjacentNode);

                // Un-successful... Proceed to next phase
                if (traversal == null)
                    continue;

                // SUCCESS!
                else
                    connection.Value.SetRoute(traversal);
            }

            // Iterate SUCCESSFUL traversals -> update topology and embed the partial path IF THE FULL PATH IS READY!
            // foreach (var partial in partialConnections.Where(element => element.Value.IsCompleted))
            for (int index = partialConnections.Count - 1; index >= 0; index--)
            {
                var element = partialConnections.ElementAt(index);
                var partial = element.Value;
                var connection = element.Key;

                // Skip un-routed connections
                if (!partial.IsRouted)
                    continue;

                // PARTIAL CONNECTION ROUTE FOUND! (Intermediate connections completed successfully)
                if (partial.Route.Route.All(route => finishedConnections.ContainsKey(route)))
                {
                    // CREATE NEW REGION CONNECTION TO APPLY TO THE GRAPH
                    var newVertex = new GraphVertex(partial.PartialRegion.Id,
                                                    partial.PartialPath.Last().Column * ModelConstants.CellWidth,
                                                    partial.PartialPath.Last().Row * ModelConstants.CellHeight,
                                                    Metric.MetricType.Euclidean);

                    var newConnection = new RegionConnectionInfo<GridLocation>(partial.Source,
                                                                               partial.PartialRegion,
                                                                               partial.PartialPath.First(),
                                                                               partial.PartialPath.Last(),
                                                                               partial.Connection.Vertex,
                                                                               newVertex,
                                                                               Metric.Distance(partial.Connection.Vertex, newVertex));

                    // Route the partial path into the grid - SHOULD NOT INVALIDATE THE GRAPH
                    EmbedPathCells(container, newConnection, partial.PartialPath, false);

                    // NOTE*** Since the path embedding doesn't modify the connection room region corridor setting, 
                    // the graph SHOULD still be valid. We need to do our due dilligence and update
                    // the connection graph. Otherwise, the whole layout container will
                    // fail validation because it's connections won't all be accounted for.
                    //

                    var connectionGraph = container.GetConnectionGraph();

                    connectionGraph.Modify(connection, newConnection);

                    // SETS GRAPH AS VALID
                    container.SetConnectionGraph(connectionGraph);

                    // Remove partial connection -> Set as finished connection since graph is modified
                    partialConnections.Remove(connection);
                    finishedConnections.Add(newConnection, newConnection);
                }
            }

            return new ConnectionInfo(finishedConnections, partialConnections);
        }

        // Forces routes from PARTIAL connections that didn't finish to be connected. USES THE EXISTING ROUTE
        // TOPOLOGY. MAY INVALIDATE THE GRAPH AND CONNECTION LAYER!
        private ConnectionInfo ForceRoutes(LayoutContainer container, ConnectionInfo connectionInfo)
        {
            // Create new ConnectionInfo for result
            var finishedConnections = new Dictionary<RegionConnectionInfo<GridLocation>, RegionConnectionInfo<GridLocation>>(connectionInfo.FinishedConnections);
            var partialConnections = new Dictionary<RegionConnectionInfo<GridLocation>, PartialConnectionInfo>(connectionInfo.PartialConnectionDict);

            for (int index = partialConnections.Count - 1; index >= 0; index--)
            {
                var element = partialConnections.ElementAt(index);
                var connection = element.Key;

                RegionInfo<GridLocation> partialRegion = RegionInfo<GridLocation>.Empty;
                List<GridLocation> pathCells = null;

                // Partial Connection -> FORCE THE CONNECTION
                if (ConnectRegions(container,
                                   connection,
                                   true,
                                   out partialRegion, out pathCells))
                {
                    EmbedPathCells(container, connection, pathCells, true);

                    finishedConnections.Add(connection, connection);
                    partialConnections.Remove(connection);
                }
                // UN-SUCCESSFUL CONNECTIONS!
                else
                {
                    throw new Exception("Route connection failure - ConnectionBuilder.ForceRoutes");
                }
            }

            return new ConnectionInfo(finishedConnections, partialConnections);
        }

        // Runs a Dijkstra path generator for the supplied connection. DOES NOT EMBED PATH! 
        // Returns false if path found is a partial path.
        private bool ConnectRegions(LayoutContainer container, 
                                    RegionConnectionInfo<GridLocation> connection, 
                                    bool forceConnection,
                                    out RegionInfo<GridLocation> intermediateRegion, 
                                    out List<GridLocation> pathLocations)
        {
            // Create a Dijkstra path generator to find paths for the edge
            var dijkstraMap = new DijkstraPathGenerator(container.Width, container.Height, 
                                                        connection.Location, new GridLocation[] { connection.AdjacentLocation }, true,

                                                        // Cost Callback for Dijkstra's Algorithm -> asks for movement cost from location1 -> location2
                                                        (column1, row1, column2, row2) =>
                                                        {
                                                            // MOVEMENT COST BASED ON THE DESTINATION CELL
                                                            //
                                                            // if (container.Get(column2, row2) == null)
                                                            //    return DijkstraMapBase.MapCostAvoid;

                                                            if (container.HasImpassableTerrain(column2, row2))
                                                                return DijkstraMapBase.MapCostInfinity;

                                                            else
                                                                return 0;
                                                        },

                                                        // Main Callback for Dijkstra's Algorithm to fetch cells
                                                        (column, row) =>
                                                        {
                                                            // NOTE*** THIS CALLBACK IS BEING USED TO CREATE NEW CELLS FOR
                                                            //         EMBEDDING A PATH FOR THE LAYOUT THAT CAN GO OFF INTO
                                                            //         UN-USED GRID LOCATIONS.
                                                            return container.Get(column, row)?.Location ?? new GridLocation(column, row);
                                                        });



            // NOTE*** THESE CELLS MAY-OR-MAY-NOT YET BE A PART OF THE LAYOUT
            var dijkstraPath = dijkstraMap.CalculatePath(connection.AdjacentLocation);

            // Out variables
            pathLocations = new List<GridLocation>();
            intermediateRegion = RegionInfo<GridLocation>.Empty;

            // Iterate the path from source -> target
            foreach (var location in dijkstraPath)
            {
                // RETRIEVE CELL FROM THE CONTAINER OR TREAT THE NEW LOCATION AS PART OF THE PATH
                var gridLocation = container.Get(location.Column, location.Row)?.Location ?? (location as GridLocation);

                // Store cells for path (ALLOW ENTRY TO REGION FOR CONNECTION POINT)
                pathLocations.Add(gridLocation);

                // DON'T WORRY ABOUT EXISTING REGIONS
                if (forceConnection)
                    continue;

                // CHECK FOR EXISTING REGION
                var existingRegion = container.GetRegions(LayoutGrid.LayoutLayer.ConnectionRoom)
                                              .Where(region => region[location] != null)
                                              .Where(region => region.Id != connection.Vertex.ReferenceId &&
                                                               region.Id != connection.AdjacentVertex.ReferenceId)
                                              .FirstOrDefault();

                // Found a region already underneath the location
                if (!existingRegion.IsEmpty())
                {
                    intermediateRegion = existingRegion;
                    return false;
                }
            }

            return true;
        }
        
        // Sets path cells appropriately for corridors; and stores the information on the connection
        // info for the path.
        private void EmbedPathCells(LayoutContainer container, RegionConnectionInfo<GridLocation> connection, List<GridLocation> pathCells, bool forceConnection)
        {
            // TODO: REDESIGN DOORS
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

            var corridors = new List<GridCellInfo>();

            // EMBED THE PATH -> SET AS CORRIDOR 
            for (int index = 0; index < pathCells.Count; index++)
            {
                var location = pathCells[index];
                var cell = container.Get(location.Column, location.Row);

                // CELL MAY HAVE BEEN A NEW INSTANCE NOT YET IN THE GRID
                if (cell == null)
                    cell = new GridCellInfo(location);

                // NOTE*** Path may meander into the region. This is ok as long as it is
                //         one of the two regions involved in the connection. Otherwise,
                //         it's considered a "partial path" or a FORCED CONNECTION.
                //
                //         For FORCED CONNECTION - don't set the IsCorridor setting for
                //         cells inside the room (connection layer) regions
                //
                //         TODO: CONSIDER ADDING CONNECTION IN THE GRAPH FOR THESE FORCED
                //               CONNECTIONS
                //

                // INSIDE THE FIRST REGION
                if (connection.Node[cell] != null)
                    continue;

                // INSIDE THE ADJACENT REGION
                else if (connection.AdjacentNode[cell] != null)
                    continue;

                else
                {
                    // NO REASON TO FLAG CORRIDORS IF INSIDE A CONNECTION ROOM REGION - UNLESS WE NEED
                    // TO INVALIDATE THE LAYOUT
                    var insideConnectionRoomRegion = container.GetRegions(LayoutGrid.LayoutLayer.ConnectionRoom)
                                                              .Any(region => region[cell] != null);

                    // ONLY INVALIDATES LAYOUT IF IT FALLS INSIDE A REGION IN THE CONNECTION LAYER
                    // (Could invalidate the terrain masking)
                    if (insideConnectionRoomRegion)
                    {
                        if (!forceConnection)
                            throw new Exception("Trying to enter a Connection Room region while NOT forcing connection path:  ConnectionBuilder.EmbedPathCells");
                    }
                        
                    // Otherwise, mark the cell as a corridor
                    else
                        cell.IsCorridor = true;

                    corridors.Add(cell);
                }

                // Check to see whether layout cell already exists
                if (container.Get(cell.Column, cell.Row) == null)
                    container.AddLayout(cell.Column, cell.Row, cell);
            }

            // Store corridor for reference
            connection.CorridorLocations = pathCells;
        }
    }
}
