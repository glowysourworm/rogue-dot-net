using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    public class CreateTemporaryCharacterAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                                       IDoodadAlterationEffect,
                                                                       IEnemyAlterationEffect,
                                                                       ISkillAlterationEffect
    {
        public AlterationRandomPlacementType RandomPlacementType { get; set; }

        public TemporaryCharacterTemplate TemporaryCharacter { get; set; }

        /// <summary>
        /// Range (in cell pseudo-euclidean distance) from acting character (based on the placement type)
        /// </summary>
        public int Range { get; set; }

        public CreateTemporaryCharacterAlterationEffect() { }
    }
}