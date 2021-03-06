﻿using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Scenario.Processing.Service.Interface;

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Processing.Service
{
    [Export(typeof(IScenarioObjectiveService))]
    public class ScenarioObjectiveService : IScenarioObjectiveService
    {
        public SimpleDictionary<string, bool> GetScenarioObjectiveUpdates(ScenarioContainer scenarioContainer)
        {
            return scenarioContainer.Encyclopedia.SubSet(metaData => metaData.IsObjective)
                                    .ToSimpleDictionary(x => x.RogueName, x =>
                                    {
                                        return IsObjectiveAcheived(scenarioContainer, x);
                                    });
        }


        public bool IsObjectiveAcheived(ScenarioContainer scenarioContainer)
        {
            return scenarioContainer
                       .Encyclopedia
                       .SubSet(x => x.IsObjective)
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
