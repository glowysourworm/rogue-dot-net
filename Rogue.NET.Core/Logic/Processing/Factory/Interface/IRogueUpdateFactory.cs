using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Factory.Interface
{
    public interface IRogueUpdateFactory
    {
        RogueUpdateEventArgs Animation(IEnumerable<AnimationData> animations, CellPoint source, IEnumerable<CellPoint> targets, RogueUpdatePriority priority = RogueUpdatePriority.High);
        RogueUpdateEventArgs Update(LevelUpdateType type, string contentId, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs Update(LevelUpdateType type, string[] contentIds, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs ConsumableAddUpdate(string consumableId, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs ConsumableRemove(string consumableId, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs EquipmentAddUpdate(string equipmentId, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs EquipmentRemove(string equipmentId, RogueUpdatePriority priority = RogueUpdatePriority.Low);
        RogueUpdateEventArgs LevelChange(int levelNumber, PlayerStartLocation playerStartLocation);
        RogueUpdateEventArgs PlayerDeath(string deathMessage);
        RogueUpdateEventArgs Save();
        RogueUpdateEventArgs Tick();
        RogueUpdateEventArgs StatisticsUpdate(ScenarioUpdateType type, string contentRogueName);
        RogueUpdateEventArgs Dialog(DialogEventType type);
        RogueUpdateEventArgs DialogModifyEquipment(EquipmentModifyAlterationEffect effect);
        RogueUpdateEventArgs DialogNote(string noteMessage, string noteTitle);
        RogueUpdateEventArgs DialogPlayerAdvancement(Player player, int playerPoints);
    }
}
