using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData.Factory.Interface
{
    public interface IBackendEventDataFactory
    {
        AnimationEventData Animation(AnimationSequence animation, GridLocation source, IEnumerable<GridLocation> targets);
        ProjectileAnimationEventData Animation(ScenarioImage scenarioImage, GridLocation source, GridLocation target);
        LevelEventData Event(LevelEventType type, string contentId);
        LevelEventData Event(LevelEventType type, string[] contentIds);
        LevelEventData ConsumableAddUpdate(string consumableId);
        LevelEventData ConsumableRemove(string consumableId);
        LevelEventData EquipmentAddUpdate(string equipmentId);
        LevelEventData EquipmentRemove(string equipmentId);
        TargetRequestEventData TargetRequest(TargetRequestType type, string associatedId);
        ScenarioEventData LevelChange(int levelNumber, PlayerStartLocation playerStartLocation);
        ScenarioEventData PlayerDeath(string deathMessage);
        ScenarioEventData Save();
        ScenarioEventData Tick();
        ScenarioEventData StatisticsUpdate(ScenarioUpdateType type, string contentRogueName);
        DialogEventData Dialog(DialogEventType type);
        DialogEventData DialogAlterationEffect(IAlterationEffect effect);
        DialogEventData DialogNote(string noteMessage, string noteTitle);
        DialogEventData DialogPlayerAdvancement(Player player, int playerPoints);
    }
}
