using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface IMazeRegionCreator
    {
        /// <summary>
        /// This specifies the recursive back-tracker rule to use. (NOT MEANT TO BE LAYOUT TEMPLATE PARAMETER!)
        /// </summary>
        public enum MazeType
        {
            Open,
            Filled
        }

        /// <summary>
        /// Generates a maze within the given region BY FIRST GENERATING WALLS.
        /// </summary>
        /// <param name="grid">Input 2D array of cells</param>
        /// <param name="boundary">Region to build the maze inside of</param>
        /// <param name="mazeType">Type of maze to build</param>
        /// <param name="wallRemovalRatio">Number in [0,1] to specify wall removal (THIS IS FURTHER SCALED BY THE COMPONENT)</param>
        /// <param name="horizontalVerticalBias">Number in [0,1] to specify horizontal v.s. vertical direction bias</param>
        /// <param name="overwrite">Option to overwrite existing cells</param>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias, bool overwrite);

        /// <summary>
        /// Generates a maze starting at the provided location - MUST BE A WALL. This will explore all connected wall cells to generate
        /// a maze. (Can be used to fill in negative space with a maze). 
        /// </summary>
        /// <param name="grid">Input 2D array of cells</param>
        /// <param name="mazeType">Type of maze to build</param>
        /// <param name="wallRemovalRatio">Number in [0,1] to specify wall removal (THIS IS FURTHER SCALED BY THE COMPONENT)</param>
        /// <param name="horizontalVerticalBias">Number in [0,1] to specify horizontal v.s. vertical direction bias</param>
        /// <param name="startingLocation">Starting WALL location</param>
        /// <param name="avoidRegions">Regions to avoid while running the algorithm</param>
        /// <exception cref="ArgumentException">Starting location is null or NOT A WALL CELL</exception>
        void CreateCellsStartingAt(GridCellInfo[,] grid, IEnumerable<Region> avoidRegions, GridLocation startingLocation, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias);
    }
}
