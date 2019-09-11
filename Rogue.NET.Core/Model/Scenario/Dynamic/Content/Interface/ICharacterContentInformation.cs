using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using System.Collections.Generic;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.Character;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface
{
    /// <summary>
    /// Component that is created once per level and works with ICharacterLayoutInformation to 
    /// dynamically store content that is visible to a particular character
    /// </summary>
    public interface ICharacterContentInformation
    {
        /// <summary>
        /// Updates content information from the vantage point of all characters using
        /// the Level and Player information. UPDATES EXPLORED / REVEALED FLAGS FOR THE CONTENT
        /// </summary>
        void ApplyUpdate(Level level, Player player);

        /// <summary>
        /// Returns visible content from the vantage point of the character (line-of-sight within
        /// light radius)
        /// </summary>
        IEnumerable<ScenarioObject> GetVisibleContents(CharacterBase character);

        /// <summary>
        /// Returns visible characters from the vantage point of the character (line-of-sight within
        /// light radius)
        /// </summary>
        IEnumerable<CharacterBase> GetVisibleCharacters(CharacterBase character);
    }
}
