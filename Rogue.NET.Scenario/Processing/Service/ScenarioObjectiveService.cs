using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Enums;
using System;
using System.ComponentModel.Composition;
using Rogue.NET.Scenario.Processing.Service.Interface;

namespace Rogue.NET.Scenario.Processing.Service
{
    [Export(typeof(IScenarioObjectiveService))]
    public class ScenarioObjectiveService : IScenarioObjectiveService
    {
        public IDictionary<string, bool> GetScenarioObjectiveUpdates(ScenarioContainer scenarioContainer)
        {
            return scenarioContainer.ScenarioEncyclopedia
                                    .Values
                                    .Where(x => x.IsObjective)
                                    .ToDictionary(x => x.RogueName, x =>
                                    {
                                        return IsObjectiveAcheived(scenarioContainer, x);
                                    });
        }


        public bool IsObjectiveAcheived(ScenarioContainer scenarioContainer)
        {
            return scenarioContainer
                       .ScenarioEncyclopedia
                       .Values
                       .Where(x => x.IsObjective)
                       .All(metaData =>
                       {
                           return IsObjectiveAcheived(scenarioContainer, metaData);
                       });
        }

        private bool IsObjectiveAcheived(ScenarioContainer scenarioContainer, ScenarioMetaData metaData)
        {
            switch (metaData.ObjectType)
            {
                // Must have used one doodad
                case DungeonMetaDataObjectTypes.Doodad:
                    return scenarioContainer.Statistics.DoodadStatistics.Any(doodad => doodad.RogueName == metaData.RogueName);

                // Must have slain one enemy
                case DungeonMetaDataObjectTypes.Character:
                    return scenarioContainer.Statistics.EnemyStatistics.Any(enemy => enemy.RogueName == metaData.RogueName);

                // Must have one in Player inventory
                case DungeonMetaDataObjectTypes.Item:
                    return scenarioContainer.Player.Inventory.Values.Any(item => item.RogueName == metaData.RogueName);

                case DungeonMetaDataObjectTypes.Skill:
                    throw new Exception("Skill sets should not support ScenarioMetaData.IsObjective");

                default:
                    return false;
            }
        }
    }
}
