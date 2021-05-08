using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;

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

        GridCellInfo[,] _grid;
        IDictionary<TerrainLayerTemplate, GridLocation[,]> _terrainDict;

        // Finalized regions - created by calling FinalizeLayout()
        IDictionary<LayoutLayer, IEnumerable<Region<GridCellInfo>>> _regionDict;

        // Terrain regions are applied to the underlying layout
        IDictionary<TerrainLayerTemplate, IEnumerable<Region<GridLocation>>> _terrainRegionDict;

        // State of the finalized layout and terrain layers. Resets when cells are modified
        bool _invalid = false;
        bool _terrainInvalid = false;

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
        /// Returns a set of terrain layer templates for each layer
        /// </summary>
        public IEnumerable<TerrainLayerTemplate> TerrainDefinitions { get { return _terrainDict.Keys; } }

        /// <summary>
        /// Returns regions for the specified layer
        /// </summary>
        public IEnumerable<Region<GridCellInfo>> GetRegions(LayoutLayer layer)
        {
            if (_invalid || _terrainInvalid)
                throw new Exception("Must finalize layout before calling for the regions");

            return _regionDict[layer];
        }

        /// <summary>
        /// Returns set of terrain regions for the layer
        /// </summary>
        public IEnumerable<Region<GridLocation>> GetTerrainRegions(TerrainLayerTemplate definition)
        {
            if (_invalid || _terrainInvalid)
                throw new Exception("Must finalize layout before calling for the regions");

            return _terrainRegionDict[definition];
        }

        /// <summary>
        /// Returns cell from primary layout grid
        /// </summary>
        public GridCellInfo Get(int column, int row)
        {
            return _grid[column, row];
        }

        // TODO: Consider moving these to an interface design

        /// <summary>
        /// Returns cardinal adjacent elements on the primary layout grid
        /// </summary>
        public IEnumerable<GridCellInfo> GetCardinalAdjacentElements(int column, int row)
        {
            return _grid.GetCardinalAdjacentElements(column, row);
        }

        public IEnumerable<GridLocation> GetCardinalAdjacentElements(TerrainLayerTemplate definition, int column, int row)
        {
            return _terrainDict[definition].GetCardinalAdjacentElements(column, row);
        }

        public bool HasTerrain(TerrainLayerTemplate definition, int column, int row)
        {
            return _terrainDict.ContainsKey(definition) &&
                   _terrainDict[definition][column, row] != null;
        }

        public bool HasImpassableTerrain(int column, int row)
        {
            return _terrainDict.Where(element => !element.Key.IsPassable)
                               .Any(element => element.Value[column, row] != null);
        }

        /// <summary>
        /// Sets new grid cell in the primary layout grid. INVALIDATES OTHER LAYERS OF THE FINAL LAYOUT
        /// </summary>
        public void SetLayout(int column, int row, GridCellInfo cell)
        {
            cell.IsLayoutModifiedEvent -= LayoutModified;
            cell.IsLayoutModifiedEvent += LayoutModified;

            // Invalidate for new cells
            _invalid |= _grid[column, row] != cell;

            _grid[column, row] = cell;
        }

        public void SetTerrain(TerrainLayerTemplate definition, int column, int row, bool hasTerrain)
        {
            if (hasTerrain)
            {
                // Check for invalidation of terrain
                _terrainInvalid |= (_terrainDict[definition][column, row] == null);

                _terrainDict[definition][column, row] = _terrainDict[definition][column, row] ?? new GridLocation(column, row);
            }
            else
            {
                // Check for invalidation of terrain
                _terrainInvalid |= (_terrainDict[definition][column, row] != null);

                _terrainDict[definition][column, row] = null;
            }
        }

        /// <summary>
        /// Creates a new terrain layer and sets up necessary layout support
        /// </summary>
        public void CreateTerrainLayer(TerrainLayerTemplate definition)
        {
            _terrainDict.Add(definition, new GridLocation[this.Width, this.Height]);
        }

        /// <summary>
        /// Constructs regions of the primary layout grid in its current state
        /// </summary>
        public IEnumerable<Region<GridCellInfo>> ConstructRegions(RegionConstructionCallback predicate)
        {
            return _grid.ConstructRegions(cell => predicate(cell.Column, cell.Row));
        }

        public IEnumerable<Region<GridLocation>> ConstructTerrainRegions(TerrainLayerTemplate definition, RegionConstructionCallback predicate)
        {
            return _terrainDict[definition].ConstructRegions(cell => predicate(cell.Column, cell.Row));
        }

        /// <summary>
        /// Processes the primary grid with the terrain grids to create the final set of regions and terrain regions
        /// </summary>
        public void RecreateRegions()
        {
            if (!_invalid && !_terrainInvalid)
                return;

            ApplyTerrain();
            CreateRegions();

            _invalid = false;
            _terrainInvalid = false;
        }

        public LayoutContainer(GridCellInfo[,] grid)
        {
            _grid = grid;
            _terrainDict = new Dictionary<TerrainLayerTemplate, GridLocation[,]>();
            _regionDict = new Dictionary<LayoutLayer, IEnumerable<Region<GridCellInfo>>>();
            _terrainRegionDict = new Dictionary<TerrainLayerTemplate, IEnumerable<Region<GridLocation>>>();

            this.Bounds = new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1));
        }

        #region (private) Methods

        private void ApplyTerrain()
        {
            // Procedure
            //
            // 1) Create cells for all terrain locations in the primary grid
            // 2) Create walls where terrain is impassible
            // 3) SKIP cells marked as corridors

            // Create new grid with removed cells for blocked terrain
            for (int column = 0; column < _grid.GetLength(0); column++)
            {
                for (int row = 0; row < _grid.GetLength(1); row++)
                {
                    // Skipping corridors will allow paths through impassable terrain
                    if (_grid[column, row] != null &&
                        _grid[column, row].IsCorridor)
                        continue;

                    // Check all terrain layers
                    foreach (var element in _terrainDict)
                    {
                        // Terrain exists
                        if (element.Value[column, row] != null)
                        {
                            _grid[column, row] = _grid[column, row] ?? new GridCellInfo(column, row);

                            // NOTE*** Fires Layout Modified event (SHOULD CONSIDER ADDING IMPASSABLE TERRAIN FLAG)
                            _grid[column, row].IsWall = !element.Key.IsPassable;
                        }
                    }
                }
            }
        }
        private void CreateRegions()
        {
            _regionDict.Clear();
            _terrainRegionDict.Clear();

            // Regions that don't depend on terrain
            _regionDict.Add(LayoutLayer.Corridor, _grid.ConstructRegions(cell => cell.IsCorridor));
            _regionDict.Add(LayoutLayer.Full, _grid.ConstructRegions(cell => true));
            _regionDict.Add(LayoutLayer.Room, _grid.ConstructRegions(cell => !cell.IsCorridor && !cell.IsWall && !cell.IsDoor));
            _regionDict.Add(LayoutLayer.Wall, _grid.ConstructRegions(cell => cell.IsWall));

            // Regions that depend on IMASSABLE terrain
            _regionDict.Add(LayoutLayer.Placement, _grid.ConstructRegions(cell => !cell.IsDoor && !cell.IsWall && !HasImpassableTerrain(cell.Column, cell.Row)));
            _regionDict.Add(LayoutLayer.Walkable, _grid.ConstructRegions(cell => !cell.IsWall && !HasImpassableTerrain(cell.Column, cell.Row)));
            _regionDict.Add(LayoutLayer.ImpassableTerrain, _grid.ConstructRegions(cell => HasImpassableTerrain(cell.Column, cell.Row)));

            // Create terrain regions
            _terrainRegionDict = _terrainDict.ToDictionary(element => element.Key,
                                                           element => element.Value
                                                                             .ConstructRegions(cell =>
                                                                             {
                                                                                 // AVOID CORRIDORS FOR IMPASSABLE TERRAIN ONLY
                                                                                 //
                                                                                 // These locations have been marked by the connection builder
                                                                                 //
                                                                                 return !_grid[cell.Column, cell.Row].IsCorridor;
                                                                             }));
        }



        private void LayoutModified(GridCellInfo modifiedCell)
        {
            _invalid = true;
        }

        #endregion
    }
}
