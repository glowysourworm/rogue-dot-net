using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface ICorridorCreator
    {
        /// <summary>
        /// Creates a corridor between the two points
        /// </summary>
        /// <param name="grid">The layout grid</param>
        /// <param name="cell1">Starting cell for the corridor</param>
        /// <param name="cell2">Ending cell for the corridor</param>
        /// <param name="createDoors">Option to create doors at the ends of the cells</param>
        /// <param name="overwrite">Option to overwrite existing cells</param>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void CreateCorridor(GridCellInfo[,] grid, GridCellInfo cell1, GridCellInfo cell2, bool createDoors, bool overwrite);

        /// <summary>
        /// Creates a rectilinear corridor section in the specified direction. This will iterate in the specified direction until the
        /// opposing cell's coordinates are met (in the direction of iteration). This is used to create halves of a corridor that is
        /// rectangular.
        /// </summary>
        /// <param name="grid">The layout grid</param>
        /// <param name="location1">Starting location for the corridor section</param>
        /// <param name="location2">Ending location for the corridor section</param>
        /// <param name="yDirection">Specifies whether to iterate in the y-direction</param>
        /// <param name="overwrite">Option to overwrite existing cells</param>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void CreateLinearCorridorSection(GridCellInfo[,] grid, GridLocation location1, GridLocation location2, bool yDirection, bool overwrite);
    }
}
