using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using System;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface ICellularAutomataRegionCreator
    {
        /// <summary>
        /// Generates cellular automata within the specified region. 
        /// </summary>
        /// <param name="grid">Input 2D array of cells</param>
        /// <param name="boundary">Region to build the maze inside of</param>
        /// <param name="type">The type of CA rule to use</param>
        /// <param name="fillRatio">The [0,1] fill ratio (THIS IS FURTHER SCALED BY THE COMPONENT)</param>
        /// <param name="overwrite">Option to overwrite existing cells</param>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void GenerateCells(GridCellInfo[,] grid, RegionBoundary boundary, LayoutCellularAutomataType type, double fillRatio, bool overwrite);


        /// <summary>
        /// Runs cellular automata rule in the specified region for a single iteration.
        /// </summary>
        void RunSmoothingIteration(GridCellInfo[,] grid, RegionBoundary boundary, LayoutCellularAutomataType type);
    }
}
