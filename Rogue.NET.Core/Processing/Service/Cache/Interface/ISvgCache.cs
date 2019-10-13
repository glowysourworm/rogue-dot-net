using Rogue.NET.Core.Model.Enums;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface ISvgCache
    {
        /// <summary>
        /// Returns drawing group for related SVG for the input cache image
        /// </summary>
        DrawingGroup GetDrawing(ScenarioCacheImage scenarioCacheImage);

        /// <summary>
        /// Returns resource names for the provided symbol type. ONLY SUPPORTS GAME / SYMBOL!
        /// </summary>
        /// <returns>Resource names (with prefix (AND) .svg removed)</returns>
        IEnumerable<string> GetResourceNames(SymbolType type);

        /// <summary>
        /// Returns the set of character resource categories 
        /// </summary>
        IEnumerable<string> GetCharacterCategories();

        /// <summary>
        /// Returns the set of resource names for the specified character category
        /// </summary>
        IEnumerable<string> GetCharacterResourceNames(string category);
    }
}
