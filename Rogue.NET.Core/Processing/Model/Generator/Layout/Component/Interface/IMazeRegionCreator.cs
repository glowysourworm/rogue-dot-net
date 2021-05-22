using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;

using static Rogue.NET.Core.Processing.Model.Extension.ArrayExtension;

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
        /// Generates a maze within the given region.
        /// </summary>
        /// <param name="region">Region to build the maze inside of</param>
        /// <param name="mazeType">Type of maze to build</param>
        /// <param name="wallRemovalRatio">Number in [0,1] to specify wall removal (THIS IS FURTHER SCALED BY THE COMPONENT)</param>
        /// <param name="horizontalVerticalBias">Number in [0,1] to specify horizontal v.s. vertical direction bias</param>
        bool[,] CreateMaze(int width, int height,
                           RegionInfo<GridLocation> region,
                           MazeType mazeType,
                           double wallRemovalRatio,
                           double horizontalVerticalBias);

        /// <summary>
        /// Generates a maze within the provided region - AVOIDING other regions. This will explore all connected NON-NULL cells to generate
        /// a maze. (Can be used to fill in negative space with a maze). 
        /// </summary>
        /// <param name="mazeType">Type of maze to build</param>
        /// <param name="wallRemovalRatio">Number in [0,1] to specify wall removal (THIS IS FURTHER SCALED BY THE COMPONENT)</param>
        /// <param name="horizontalVerticalBias">Number in [0,1] to specify horizontal v.s. vertical direction bias</param>
        /// <param name="avoidCallback">Callback for cells to avoid -> used with the algorithm to maze around it!</param>
        /// <exception cref="ArgumentException">Starting location is null or NOT A WALL CELL</exception>
        bool[,] CreateMaze(int width, int height,
                           RegionInfo<GridLocation> region,
                           GridPredicate<GridLocation> avoidCallback,
                           MazeType mazeType,
                           double wallRemovalRatio,
                           double horizontalVerticalBias);
    }
}
