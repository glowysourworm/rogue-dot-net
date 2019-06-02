﻿using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Processing.Factory
{
    [Export(typeof(IRogueUpdateFactory))]
    public class RogueUpdateFactory : IRogueUpdateFactory
    {
        public RogueUpdateEventArgs Animation(IEnumerable<AnimationTemplate> animations, CellPoint source, IEnumerable<CellPoint> targets, RogueUpdatePriority priority = RogueUpdatePriority.High)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new AnimationUpdate()
                {
                    Animations = animations,
                    SourceLocation = source,
                    TargetLocations = targets
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs Update(LevelUpdateType type, string contentId, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = type,
                    ContentIds = new string[] { contentId }
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs Update(LevelUpdateType type, string[] contentIds, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = type,
                    ContentIds = contentIds
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs ConsumableAddUpdate(string consumableId, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.PlayerConsumableAddOrUpdate,
                    ContentIds = new string[] { consumableId }
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs ConsumableRemove(string consumableId, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.PlayerConsumableRemove,
                    ContentIds = new string[] { consumableId }
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs EquipmentAddUpdate(string equipmentId, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.PlayerEquipmentAddOrUpdate,
                    ContentIds = new string[] { equipmentId }
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs EquipmentRemove(string equipmentId, RogueUpdatePriority priority = RogueUpdatePriority.Low)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.PlayerEquipmentRemove,
                    ContentIds = new string[] { equipmentId }
                },
                Priority = priority
            };
        }
        public RogueUpdateEventArgs LevelChange(int levelNumber, PlayerStartLocation playerStartLocation)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.LevelChange,
                    LevelNumber = levelNumber,
                    StartLocation = playerStartLocation
                },
                Priority = RogueUpdatePriority.Critical
            };
        }
        public RogueUpdateEventArgs PlayerDeath(string deathMessage)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.PlayerDeath,
                    PlayerDeathMessage = deathMessage
                },
                Priority = RogueUpdatePriority.Critical
            };
        }
        public RogueUpdateEventArgs Save()
        {
            return new RogueUpdateEventArgs()
            {
                Update = new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.Save
                },
                Priority = RogueUpdatePriority.Low
            };
        }
        public RogueUpdateEventArgs Tick()
        {
            return new RogueUpdateEventArgs()
            {
                Update = new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.StatisticsTick
                },
                Priority = RogueUpdatePriority.Low
            };
        }
        public RogueUpdateEventArgs StatisticsUpdate(ScenarioUpdateType type, string contentRogueName)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new ScenarioUpdate()
                {
                    ScenarioUpdateType = ScenarioUpdateType.StatisticsDoodadUsed,
                    ContentRogueName = contentRogueName
                },
                Priority = RogueUpdatePriority.Low
            };
        }
        public RogueUpdateEventArgs Dialog(DialogEventType type)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogUpdate()
                {
                    Type = type
                },
                Priority = RogueUpdatePriority.High
            };
        }
        public RogueUpdateEventArgs Dialog(DialogEventType type, IEnumerable<AttackAttribute> attackAttributes)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogUpdate()
                {
                    Type = type,
                    ImbueAttackAttributes = attackAttributes
                },
                Priority = RogueUpdatePriority.High
            };
        }
        public RogueUpdateEventArgs DialogNote(string noteMessage, string noteTitle)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogUpdate()
                {
                    NoteMessage = noteMessage,
                    NoteTitle = noteTitle,
                    Type = DialogEventType.Note
                },
                Priority = RogueUpdatePriority.High
            };
        }
    }
}