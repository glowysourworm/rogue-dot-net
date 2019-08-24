using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Processing.Factory
{
    [Export(typeof(IRogueUpdateFactory))]
    public class RogueUpdateFactory : IRogueUpdateFactory
    {
        public RogueUpdateEventArgs Animation(IEnumerable<AnimationData> animations, GridLocation source, IEnumerable<GridLocation> targets, RogueUpdatePriority priority = RogueUpdatePriority.High)
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
        public RogueUpdateEventArgs DialogEnhanceEquipment(EquipmentEnhanceAlterationEffect effect)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogEquipmentEnhanceUpdate()
                {
                    Type = DialogEventType.ModifyEquipment,
                    Effect = effect
                },
                Priority = RogueUpdatePriority.High
            };
        }
        public RogueUpdateEventArgs DialogNote(string noteMessage, string noteTitle)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogNoteUpdate()
                {
                    NoteMessage = noteMessage,
                    NoteTitle = noteTitle,
                    Type = DialogEventType.Note
                },
                Priority = RogueUpdatePriority.High
            };
        }
        public RogueUpdateEventArgs DialogPlayerAdvancement(Player player, int playerPoints)
        {
            return new RogueUpdateEventArgs()
            {
                Update = new DialogPlayerAdvancementUpdate()
                {
                    Type = DialogEventType.PlayerAdvancement,
                    Agility = player.AgilityBase,
                    Intelligence = player.IntelligenceBase,
                    Strength = player.StrengthBase,
                    PlayerPoints = playerPoints,
                    SkillPoints = player.SkillPoints
                },
                Priority = RogueUpdatePriority.High
            };
        }
    }
}
