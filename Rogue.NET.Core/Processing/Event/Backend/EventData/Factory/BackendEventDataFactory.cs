﻿using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.Factory
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IBackendEventDataFactory))]
    public class BackendEventDataFactory : IBackendEventDataFactory
    {
        public AnimationEventData Animation(AnimationSequence animation, GridLocation source, IEnumerable<GridLocation> targets)
        {
            return new AnimationEventData()
            {
                Animation = animation,
                SourceLocation = source,
                TargetLocations = targets
            };
        }
        public ProjectileAnimationEventData ThrowAnimation(ScenarioImage scenarioImage, GridLocation source, GridLocation target)
        {
            return new ProjectileAnimationEventData()
            {
                ProjectileImage = scenarioImage,
                TargetLocation = target,
                SourceLocation = source,
                OrientedImage = false
            };
        }
        public ProjectileAnimationEventData AmmoAnimation(ScenarioImage scenarioImage, GridLocation source, GridLocation target)
        {
            return new ProjectileAnimationEventData()
            {
                ProjectileImage = scenarioImage,
                TargetLocation = target,
                SourceLocation = source,
                OrientedImage = true
            };
        }
        public LevelEventData Event(LevelEventType type, string contentId)
        {
            return new LevelEventData()
            {
                LevelUpdateType = type,
                ContentIds = new string[] { contentId }
            };
        }
        public LevelEventData Event(LevelEventType type, string[] contentIds)
        {
            return new LevelEventData()
            {
                LevelUpdateType = type,
                ContentIds = contentIds
            };
        }
        public LevelEventData ConsumableAddUpdate(string consumableId)
        {
            return new LevelEventData()
            {
                LevelUpdateType = LevelEventType.PlayerConsumableAddOrUpdate,
                ContentIds = new string[] { consumableId }
            };
        }
        public LevelEventData ConsumableRemove(string consumableId)
        {
            return new LevelEventData()
            {
                LevelUpdateType = LevelEventType.PlayerConsumableRemove,
                ContentIds = new string[] { consumableId }
            };
        }
        public LevelEventData EquipmentAddUpdate(string equipmentId)
        {
            return new LevelEventData()
            {
                LevelUpdateType = LevelEventType.PlayerEquipmentAddOrUpdate,
                ContentIds = new string[] { equipmentId }
            };
        }
        public LevelEventData EquipmentRemove(string equipmentId)
        {
            return new LevelEventData()
            {
                LevelUpdateType = LevelEventType.PlayerEquipmentRemove,
                ContentIds = new string[] { equipmentId }
            };
        }
        public TargetRequestEventData TargetRequest(TargetRequestType type, string associatedId)
        {
            return new TargetRequestEventData(type, associatedId);
        }
        public ScenarioEventData LevelChange(int levelNumber, PlayerStartLocation playerStartLocation)
        {
            return new ScenarioEventData()
            {
                ScenarioUpdateType = ScenarioUpdateType.LevelChange,
                LevelNumber = levelNumber,
                StartLocation = playerStartLocation
            };
        }
        public ScenarioEventData PlayerDeath(string deathMessage)
        {
            return new ScenarioEventData()
            {
                ScenarioUpdateType = ScenarioUpdateType.PlayerDeath,
                PlayerDeathMessage = deathMessage
            };
        }
        public ScenarioEventData Save()
        {
            return new ScenarioEventData()
            {
                ScenarioUpdateType = ScenarioUpdateType.Save
            };
        }
        public ScenarioEventData Tick()
        {
            return new ScenarioEventData()
            {
                ScenarioUpdateType = ScenarioUpdateType.StatisticsTick
            };
        }
        public ScenarioEventData StatisticsUpdate(ScenarioUpdateType type, string contentRogueName)
        {
            return new ScenarioEventData()
            {
                ScenarioUpdateType = ScenarioUpdateType.StatisticsDoodadUsed,
                ContentRogueName = contentRogueName
            };
        }
        public DialogEventData Dialog(DialogEventType type)
        {
            return new DialogEventData()
            {
                Type = type
            };
        }
        public DialogEventData DialogAlterationEffect(IAlterationEffect effect)
        {
            return new DialogAlterationEffectEventData()
            {
                Type = DialogEventType.AlterationEffect,
                Effect = effect
            };
        }
        public DialogEventData DialogNote(string noteMessage, string noteTitle)
        {
            return new DialogNoteEventData()
            {
                NoteMessage = noteMessage,
                NoteTitle = noteTitle,
                Type = DialogEventType.Note
            };
        }
        public DialogEventData DialogPlayerAdvancement(Player player, int playerPoints)
        {
            return new DialogPlayerAdvancementEventData()
            {
                Type = DialogEventType.PlayerAdvancement,
                Agility = player.AgilityBase,
                Intelligence = player.IntelligenceBase,
                Strength = player.StrengthBase,
                PlayerPoints = playerPoints,
                SkillPoints = player.SkillPoints
            };
        }
    }
}
