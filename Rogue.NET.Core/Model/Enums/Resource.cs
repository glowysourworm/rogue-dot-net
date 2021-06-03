namespace Rogue.NET.Core.Model.Enums
{
    public enum ConfigResources
    {
        Adventurer,
        Fighter,
        Paladin,
        Witch,
        Sorcerer,
        Necromancer
    }

    public enum SymbolType
    {
        /// <summary>
        /// Image resources are drawn from the Smiley control
        /// </summary>
        Smiley,

        /// <summary>
        /// Image resources are drawn from the Character sub-folder (see svg resources)
        /// </summary>
        Character,

        /// <summary>
        /// Image resources are drawn from the Symbol sub-folder (see svg resources)
        /// </summary>
        Symbol,

        /// <summary>
        /// Image resources are drawn from the Game sub-folder (see svg resources)
        /// </summary>
        Game,

        /// <summary>
        /// Image resources are drawn from the OrientedSymbol sub-folder (see svg resources)
        /// </summary>
        OrientedSymbol,

        /// <summary>
        /// Image resources are drawn from the Terrain sub-folder (see svg resources)
        /// </summary>
        Terrain
    }
}
