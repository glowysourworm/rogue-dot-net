using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Extension
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Returns null if out of bounds
        /// </summary>
        public static T Get<T>(this T[,] grid, int column, int row) where T : class
        {
            if (column < 0 ||
                column >= grid.GetLength(0) ||
                row < 0 ||
                row >= grid.GetLength(1))
                return null;

            return grid[column, row];
        }       
    }
}
