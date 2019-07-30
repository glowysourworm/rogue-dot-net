using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /*
    Scenario Asset Reference Manager: Component responsible for handling references to common objects

        - Use Case:  Attack attribute added to scenario
            - Action:  Add attack attribute to all enemy, and alteration collections

        - Use Case:  Attack attribute removed from scenario
            - Action:  Remove attack attribute from all enemies and alteration collections
            - Action:  Validate effected objects to notify user in case they're no longer effective (TBD)

        - Use Case:  Item removed from scenario
            - Action:  Remove item from player / enemy inventory
            - Action:  Validate Scenario Objective (be sure to notify user) (TBD)

        - Use Case:  Skill Set Removed from scenario
            - Action:  Remove skill set from player / item / doodad (learned skill)
            - Action:  Validate that items / doodads have a use (learned skill) (TBD)

        - Use Case:  Alteration Removed from scenario 
            - Action:  Remove alteration reference from items / doodads / skill sets
            - Action:  Validate that items / doodads / skill sets have a use (TBD)
            - Action:  Validate related player skill sets / items (TBD)

        - Use Case:  Animation Removed from scenario
            - Action:  (Similar to above) Validate all affected objects up the tree

        - Use Case:  Brush removed from scenario
            - Action:  (Same)
     */
    public interface IScenarioAssetReferenceService
    {
        /// <summary>
        /// Updates Attack Attribute references on Enemies, Equipment, and Alterations. This will
        /// ADD them to the collections if they don't exist. (ALL Attack Attributes exist on ALL
        /// collections of them in the Scenario. This makes it much easier to manage downstream)
        /// </summary>
        void UpdateAttackAttributes(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Character Class references on Enemies, Equipment, Consumables, and Doodads. This will
        /// ensure that the reference is broken if it has been removed.
        /// </summary>
        void UpdateCharacterClasses(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Altered Character State references on Alterations. This will set dangling references
        /// to default (new AlteredCharacterStateTemplate) - which will be a "Normal" Base Type.
        /// </summary>
        void UpdateAlteredCharacterStates(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Item references on Enemies, and Player
        /// </summary>
        void UpdateItems(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates SkillSet references on Player, and Alterations
        /// </summary>
        void UpdateSkillSets(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Alteration references on Equipment, Consumable, Doodad, and SkillSet
        /// </summary>
        void UpdateAlterations(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Animation references on Alterations
        /// </summary>
        void UpdateAnimations(ScenarioConfigurationContainerViewModel configuration);

        /// <summary>
        /// Updates Brush references on Animations
        /// </summary>
        void UpdateBrushes(ScenarioConfigurationContainerViewModel configuration);
    }
}
