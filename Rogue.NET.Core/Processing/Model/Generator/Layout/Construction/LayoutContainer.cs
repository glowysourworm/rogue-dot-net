using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    /// <summary>
    /// Container used to hold pieces of the layout during generation
    /// </summary>
    public class LayoutContainer
    {
        /// <summary>
        /// Delegate that returns true if the location is to be used for constructing a region
        /// </summary>
        public delegate bool RegionConstructionCallback(int column, int row);

        readonly ITerrainBuilder _terrainBuilder;

        GridCellInfo[,] _grid;

        // Finalized regions - created by calling FinalizeLayout()
        IDictionary<LayoutLayer, IEnumerable<RegionInfo<GridLocation>>> _regionDict;
        IDictionary<TerrainLayerTemplate, IEnumerable<RegionInfo<GridLocation>>> _finalizedTerrainDict;

        // Store data for both graphs for the layout
        RegionGraphInfo<GridLocation> _graph;

        // State of the finalized layout and terrain layers. Resets when cells are modified
        IDictionary<LayoutLayer, bool> _invalidRegionDict;
        bool _graphInvalid = true;

        /// <summary>
        /// Width of the primary layout grid (and all terrain grids)
        /// </summary>
        public int Width { get { return _grid.GetLength(0); } }

        /// <summary>
        /// Height of the primary layout grid (and all terrain grids)
        /// </summary>
        public int Height { get { return _grid.GetLength(1); } }

        /// <summary>
        /// Boundary of the entire layout
        /// </summary>
        public RegionBoundary Bounds { get; private set; }

        /// <summary>
        /// Returns regions for the specified layer
        /// </summary>
        public IEnumerable<RegionInfo<GridLocation>> GetRegions(LayoutLayer layer)
        {
            if (_invalidRegionDict[layer])
                throw new Exception("Must finalize layout or process regions before calling for the regions");

            return _regionDict[layer];
        }

        /// <summary>
        /// Returns cell from primary layout grid
        /// </summary>
        public GridCellInfo Get(int column, int row)
        {
            return _grid[column, row];
        }

        public RegionGraphInfo<GridLocation> GetConnectionGraph()
        {
            if (_graphInvalid)
                throw new Exception("Must triangulate connection graph of rooms because of changes to layout");

            return _graph;
        }

        public bool HasImpassableTerrain(int column, int row)
        {
            return _terrainBuilder.AnyImpassableTerrain(column, row);
        }

        /// <summary>
        /// Sets new grid cell in the primary layout grid. INVALIDATES OTHER LAYERS OF THE FINAL LAYOUT
        /// </summary>
        public void AddLayout(int column, int row, GridCellInfo cell)
        {
            if (_grid[column, row] != null)
                throw new Exception("Trying to overwrite existing layout cell LayoutContainer.AddLayout");

            cell.IsLayoutModifiedEvent -= LayoutModified;
            cell.IsLayoutModifiedEvent += LayoutModified;

            _grid[column, row] = cell;

            // Invalidate appropriate layers
            LayoutAddedValidation(cell);
        }

        /// <summary>
        /// Removes grid cell in the primary layout grid. INVALIDATES OTHER LAYERS OF THE FINAL LAYOUT
        /// </summary>
        public void RemoveLayout(GridCellInfo cell)
        {
            if (_grid[cell.Column, cell.Row] != cell)
                throw new Exception("Trying to overwrite invalid layout cell LayoutContainer.AddLayout");

            cell.IsLayoutModifiedEvent -= LayoutModified;

            // Nullify grid
            _grid[cell.Column, cell.Row] = null;

            // Invalidate appropriate layers
            LayoutAddedValidation(cell);
        }

        /// <summary>
        /// Sets graph for the current room regions and flags it as VALID.
        /// </summary>
        public void SetConnectionGraph(RegionGraphInfo<GridLocation> graph)
        {
            if (AnyInvalidLayers())
                throw new Exception("Trying to set connection graph before finalizing layout:  LayoutContainer.SetConnectionGraph");

            _graph = graph;
            _graphInvalid = false;
        }

        /// <summary>
        /// Constructs regions of empty space in the primary layout grid in its current state. THESE ARE NOT
        /// STORED IN THE CONTAINER.
        /// </summary>
        public IEnumerable<RegionInfo<GridLocation>> CreateEmptySpaceRegions()
        {
            var locations = new GridLocation[this.Width, this.Height];

            _grid.Iterate((column, row) =>
            {
                if (_grid[column, row] == null)
                    locations[column, row] = new GridLocation(column, row);
            });

            return locations.ConstructRegions("Empty Space", location => true, location => location);
        }

        /// <summary>
        /// Processes updates to the layout regions using the current state of the layout / terrain grids.
        /// </summary>
        public void ProcessRegions()
        {
            RecreateRegions();
        }


        /// <summary>
        /// Final validation routine - verifies layout is valid, regions, terrain, and runs a final connection layer validation
        /// to check that region id's and connections match exactly.
        /// </summary>
        public bool Validate()
        {
            return !AnyInvalidLayers() &&
                   !_graphInvalid &&
                   ValidateConnections();
        }

        /// <summary>
        /// Finalizes layout by making sure it is valid and there is support for terrain cells
        /// </summary>
        public FinalizedLayoutContainer Finalize()
        {
            if (_graphInvalid)
                throw new Exception("Trying to finalize an invalid graph layout:  LayoutContainer.Finalize");

            if (_finalizedTerrainDict == null)
                throw new Exception("Must finalize terrain before finalizing the layout");

            RecreateRegions();

            return new FinalizedLayoutContainer()
            {
                Bounds = this.Bounds,
                Graph = _graph,
                Grid = _grid,
                RegionDict = _regionDict,
                TerrainDict = _finalizedTerrainDict
            };
        }

        /// <summary>
        /// CALLED BEFORE ADDING WALLS TO CREATE TERRAIN SUPPORT! FINALIZE TERRAIN BEFORE LAYOUT!  Creates terrain regions and 
        /// support in the layout grid for valid terrain regions (removes islands). FORCES RE-CREATION OF LAYOUT LAYERS.
        /// </summary>
        /// <returns></returns>
        public void FinalizeTerrain()
        {

            if (_graphInvalid)
                throw new Exception("Trying to finalize an invalid graph layout:  LayoutContainer.FinalizeTerrain");

            // Create terrain support in the primary grid, then re-create regions to finalize the terrain layers.
            // There may be some new overlap between non-connected regions; but the graph is still valid and playable.
            // Remove 
            //

            // Procedure
            //
            // 0) Re-create regions to ensure valid layers
            // 1) Create terrain regions - eliminating islands (REMOVE FROM ORIGINAL)
            // 2) Set terrain as valid since its cells align with the layout
            // 3) Re-create FINAL set of LAYOUT regions checking for the terrain support flag
            //

            // Validate the layers
            RecreateRegions();

            // Construct new regions that obey the layout changes. This
            // will prepare the final terrain regions that have support from the
            // layout grid.

            // Creates cells in the layout where terrain exists - can invalidate the layers. SHOULD
            // NOT MODIFY THE TOPOLOGY!!
            //
            _terrainBuilder.AddTerrainSupport(this);

            // Re-validate the layers - ADDING THE TERRAIN SUPPORT LAYER
            RecreateRegions();

            // Eliminates islands based on the WALKABLE AND TERRAIN SUPPORT LAYERS - obeying the terrain mask
            _finalizedTerrainDict = _terrainBuilder.FinalizeTerrainRegions(this);

            RecreateRegions();

            // NOTE***  This should ONLY be called HERE! Since the terrain has been finalized - there is a change
            //          the the regions. The new ones will have a different hash code identity than the old ones. This
            //          is OK to do HERE - but ONLY because the TOPOLOGY HASN'T BEEN BROKEN.
            RebuildGraph();
        }


        /// <summary>
        /// STATEFUL SERVICE INJECTION
        /// </summary>
        public LayoutContainer(ITerrainBuilder terrainBuilder,
                               LayoutTemplate template,
                               GridCellInfo[,] grid)
        {
            _terrainBuilder = terrainBuilder;
            _grid = grid;
            _regionDict = new Dictionary<LayoutLayer, IEnumerable<RegionInfo<GridLocation>>>();
            _invalidRegionDict = new Dictionary<LayoutLayer, bool>();
            _graph = null;

            // Initialize
            this.Bounds = new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1));

            // Initialize terrain layers
            _terrainBuilder.Initialize(grid.GetLength(0), grid.GetLength(1), template.TerrainLayers);

            // Create layer flags
            foreach (var layer in Enum.GetValues(typeof(LayoutLayer))
                                      .Cast<LayoutLayer>())
            {
                // Initialize to invalid
                _invalidRegionDict.Add(layer, true);
                _regionDict.Add(layer, new List<RegionInfo<GridLocation>>());
            }

            RecreateRegions();
        }

        #region (private) Methods

        private void RecreateRegions(bool force = false)
        {
            // Enumerate layout layers - skipping ones that haven't been modified
            foreach (var layer in Enum.GetValues(typeof(LayoutLayer))
                                      .Cast<LayoutLayer>())
            {
                // SKIP VALIDATED LAYERS - PRIMARY INVALID FLAG SET IF NEW CELLS ADDED
                if (_invalidRegionDict[layer] || force)
                {
                    // Combines layout grid with query to the terrain grids to construct layout regions. Avoids impassable terrain.
                    //
                    // NOTE*** IsTerrainSupport flag is NOT used as a query for terrain. It is supposed to be available to add cells
                    //         where terrain overlaps the layout AND empty space. These MAY OR MAY NOT be walkable.
                    //         
                    switch (layer)
                    {
                        // AVOID IMPASSABLE TERRAIN
                        case LayoutLayer.Walkable:
                        case LayoutLayer.Placement:
                        case LayoutLayer.ConnectionRoom:
                        case LayoutLayer.Corridor:
                        case LayoutLayer.Wall:
                            _regionDict[layer] = _grid.ConstructRegions(layer.ToString(), cell => cell.IsLayer(layer) &&
                                                                                                  !_terrainBuilder.AnyImpassableTerrain(cell.Column, cell.Row),
                                                                                                  cell => cell.Location);
                            break;

                        // DON'T AVOID IMPASSABLE TERRAIN
                        case LayoutLayer.Room:
                        case LayoutLayer.FullNoTerrainSupport:
                        case LayoutLayer.TerrainSupport:
                            _regionDict[layer] = _grid.ConstructRegions(layer.ToString(), cell => cell.IsLayer(layer), cell => cell.Location);
                            break;
                        default:
                            throw new Exception("Unhandled LayoutLayer type:  LayoutContainer.CreateRegions");
                    }

                    _invalidRegionDict[layer] = false;
                }
            }
        }

        // Re-creates graph with new regions based on connection points that match with the current region dictionary
        private void RebuildGraph()
        {
            // NOTE*** This only works when CONNECTIONROOM regions haven't been altered by CORRIDORS in a way 
            //         that BREAKS THE TOPOLOGY.
            //

            // First, Check for a SINGLE ROOM topology
            if (!_graph.HasEdges())
            {
                // MUST BE SINGLE REGION
                var singleRegion = _regionDict[LayoutLayer.ConnectionRoom].Single();

                _graph = new RegionGraphInfo<GridLocation>(singleRegion);

                return;
            }

            var edges = _graph.GetConnections();
            var newEdges = new List<RegionConnectionInfo<GridLocation>>();

            // Match edges using connection points
            foreach (var edge in edges)
            {
                // Locate first region
                var node = _regionDict[LayoutLayer.ConnectionRoom].First(region => region[edge.Location] != null);

                // Locate adjacent region
                var adjacentNode = _regionDict[LayoutLayer.ConnectionRoom].First(region => region[edge.AdjacentLocation] != null);

                // Create a new edge for the graph
                newEdges.Add(new RegionConnectionInfo<GridLocation>(node,
                                                                    adjacentNode,
                                                                    node[edge.Location],
                                                                    adjacentNode[edge.AdjacentLocation],
                                                                    edge.Vertex,
                                                                    edge.AdjacentVertex,
                                                                    edge.EuclideanRenderedDistance));
            }

            _graph = new RegionGraphInfo<GridLocation>(newEdges);
            _graphInvalid = false;
        }

        private LayoutLayer GetAffectedLayers(string cellPropertyName)
        {
            switch (cellPropertyName)
            {
                case "IsWall": return LayoutLayer.Wall | LayoutLayer.Placement | LayoutLayer.ConnectionRoom | LayoutLayer.Walkable | LayoutLayer.Room;
                case "IsCorridor": return LayoutLayer.ConnectionRoom | LayoutLayer.Room | LayoutLayer.Corridor;
                default:
                    throw new Exception("Unhandled GridCellInfo property:  LayoutContainer.IsLayerModified");
            }
        }

        private void LayoutAddedValidation(GridCellInfo cell)
        {
            // Other Layers
            foreach (var layer in Enum.GetValues(typeof(LayoutLayer))
                                      .Cast<LayoutLayer>())
            {
                // Layer is affected by the addition of the new cell
                if (cell.IsLayer(layer))
                {
                    _invalidRegionDict[layer] = true;

                    if (layer == LayoutLayer.ConnectionRoom)
                        _graphInvalid = true;
                }
            }
        }

        private void LayoutModified(string propertyName, GridCellInfo modifiedCell)
        {
            // Other Layers
            foreach (var layer in Enum.GetValues(typeof(LayoutLayer))
                                      .Cast<LayoutLayer>())
            {
                // 1) The layer directly contains the cell
                if (_regionDict[layer].Any(region => region[modifiedCell.Column, modifiedCell.Row] != null))
                    _invalidRegionDict[layer] = true;

                // 2) The layer is affected by the property that changed
                if (GetAffectedLayers(propertyName).Has(layer))
                    _invalidRegionDict[layer] = true;

                // CHECK TO SEE WHETHER INVALID CELL IS PART OF THE CONNECTION LAYER
                if (_invalidRegionDict[layer] && layer == LayoutLayer.ConnectionRoom)
                    _graphInvalid = true;
            }
        }

        private bool ValidateConnections()
        {
            // Be sure that regions have been first validated
            if (AnyInvalidLayers())
                return false;

            // Save some iterating by making this dictionary first
            var regionLookup = _regionDict[LayoutLayer.ConnectionRoom].ToDictionary(region => region.Id, region => region);

            var valid = true;

            if (_graph.HasEdges())
            {
                // CHECK THAT GRAPH CONTAINS CONNECTIONS FOR THIS LAYER -> EXACTLY!
                foreach (var connection in _graph.GetConnections())
                {
                    valid &= regionLookup.ContainsKey(connection.Vertex.ReferenceId);
                    valid &= regionLookup.ContainsKey(connection.AdjacentVertex.ReferenceId);
                }

                // CHECK THAT THE REVERSE LOOKUP IS TRUE (FOR REGION ID's)
                foreach (var region in _regionDict[LayoutLayer.ConnectionRoom])
                {
                    valid &= _graph.GetAdjacentEdges(region).Any();
                }
            }

            return valid;
        }

        private bool AnyInvalidLayers()
        {
            return _invalidRegionDict.Values.Any(invalid => invalid);
        }

        #endregion
    }
}
