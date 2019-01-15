using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IScenarioMetaDataGenerator
    {
        ScenarioMetaData CreateScenarioMetaData(ConsumableTemplate template);
        ScenarioMetaData CreateScenarioMetaData(EquipmentTemplate template);
        ScenarioMetaData CreateScenarioMetaData(EnemyTemplate template);
        ScenarioMetaData CreateScenarioMetaData(DoodadTemplate template);
        ScenarioMetaData CreateScenarioMetaData(SkillSetTemplate template);
        ScenarioMetaData CreateScenarioMetaData(SpellTemplate template);
        ScenarioMetaData CreateScenarioMetaData(DoodadNormalType doodadNormalType);
        ScenarioMetaData CreateScenarioMetaData(ReligionTemplate religionTemplate);
    }
}
