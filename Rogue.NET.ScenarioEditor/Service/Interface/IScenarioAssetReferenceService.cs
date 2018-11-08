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
        void UpdateAttackAttributes(ScenarioConfigurationContainerViewModel configuration);
        void UpdateItems(ScenarioConfigurationContainerViewModel configuration);
        void UpdateDoodads(ScenarioConfigurationContainerViewModel configuration);
        void UpdateSkillSets(ScenarioConfigurationContainerViewModel configuration);
        void UpdateAlterations(ScenarioConfigurationContainerViewModel configuration);
        void UpdateAnimations(ScenarioConfigurationContainerViewModel configuration);
        void UpdateBrushes(ScenarioConfigurationContainerViewModel configuration);
    }
}
