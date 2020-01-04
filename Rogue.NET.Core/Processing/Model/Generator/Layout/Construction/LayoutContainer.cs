using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Construction
{
    public class LayoutContainer
    {
        // Base regions are calculated by the Region Builder. Modifications happen because of several reasons:
        //
        // 1) Symmetry algorithm
        // 2) Maze corridor builder
        // 3) Impassable terrain
        //
        // The base regions are used by the finishers to try and position doors more according to the original 
        // layout. 
        //
        // Either the base or modified regions MUST make up the connection layer. This is required for path 
        // finding in the game.
        //
        // So, the builders need to know whether the base layer has been modified or not; and choose regions
        // to build with accordingly.
        //

        /// <summary>
        /// Primary grid for the layout 
        /// </summary>
        public GridCellInfo[,] Grid { get; private set; }

        /// <summary>
        /// Original base layer regions for the grid before ANY modifications:  Symmetry, Maze corridors, Terrain.
        /// </summary>
        public IEnumerable<Region<GridCellInfo>> BaseRegions { get; private set; }

        /// <summary>
        /// Final connection layer re-built after other layout modifications
        /// </summary>
        public ConnectedLayerInfo<GridCellInfo> ConnectionLayer { get; private set; }

        /// <summary>
        /// Terrain layers built by the ITerrainBuilder
        /// </summary>
        public IEnumerable<LayerInfo<GridCellInfo>> TerrainLayers { get; private set; }

        /// <summary>
        /// Set to true if the default layout was used because of a failed run
        /// </summary>
        public bool IsDefault { get; private set; }

        public bool IsBaseLayerModifed { get; private set; }

        public LayoutContainer(GridCellInfo[,] grid, bool isDefault = false)
        {
            this.Grid = grid;
        }

        /// <summary>
        /// Sets up "Base" regions - resets any modifications (connection layer)
        /// </summary>
        public void SetBaseLayer(IEnumerable<ConnectedRegion<GridCellInfo>> baseRegions)
        {
            this.BaseRegions = baseRegions;

            // Reset the connection layer
            this.ConnectionLayer = null;
            this.IsBaseLayerModifed = false;
        }

        /// <summary>
        /// Sets up the connection layer - this could be the BASE regions if there were no modifications.
        /// </summary>
        /// <param name="connectionRegions">BASE (or) MODIFIED regions with supporting triangulation</param>
        /// <param name="connectionGraph">Supporting triangulation graph for the connection layer. MUST MATCH THE REGIONS BY ID.</param>
        public void SetConnectionLayer(IEnumerable<ConnectedRegion<GridCellInfo>> connectionRegions, Graph connectionGraph)
        {
            this.ConnectionLayer = new ConnectedLayerInfo<GridCellInfo>("Connection Layer", connectionGraph, connectionRegions, true);
            
            // Set to true if the base and connection regions don't match (by reference.. probably good enough)
            this.IsBaseLayerModifed = (this.BaseRegions != connectionRegions);
        }

        public void SetTerrainLayers(IEnumerable<LayerInfo<GridCellInfo>> terrainLayers)
        {
            this.TerrainLayers = terrainLayers;
        }
    }
}
