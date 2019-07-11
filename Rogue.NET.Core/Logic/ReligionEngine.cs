using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Processing.Factory.Interface;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(IReligionEngine))]
    public class ReligionEngine : IReligionEngine
    {
        readonly IPlayerProcessor _playerProcessor;
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly IRogueUpdateFactory _rogueUpdateFactory;

        public event EventHandler<RogueUpdateEventArgs> RogueUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ReligionEngine(
            IPlayerProcessor playerProcessor,
            IModelService modelService, 
            IScenarioMessageService scenarioMessageService,
            IRogueUpdateFactory rogueUpdateFactory)
        {
            _playerProcessor = playerProcessor;
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _rogueUpdateFactory = rogueUpdateFactory;
        }

        public void Affiliate(Religion religion)
        {
            var player = _modelService.Player;

            // Non-Affiliated
            if (!player.ReligiousAlteration.IsAffiliated())
            {
                // Set new affiliation
                player.ReligiousAlteration.Affiliate(religion);
            }
            // Keeps current religion
            else if (player.ReligiousAlteration.Religion != religion)
                throw new Exception("Trying to affiliate to new religion before renouncing");
        }

        public LevelContinuationAction RenounceReligion(bool forceRenunciation)
        {
            var player = _modelService.Player;

            // Not Affiliated
            if (!player.ReligiousAlteration.IsAffiliated())
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not Affiliated with any Religion");
                return LevelContinuationAction.DoNothing;
            }

            // Can't Renounce
            else if (!player.ReligiousAlteration.CanRenounce() && !forceRenunciation)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.ReligiousAlteration.Religion.RogueName + " does not allow Renunciation!");
                return LevelContinuationAction.DoNothing;
            }

            // Renounces
            else
            {
                // Religion Name
                var religion = player.ReligiousAlteration.Religion;

                // Renunciation Animations
                var animations = player.ReligiousAlteration.Renounce();

                // Religious Affiliated Equipmpent
                foreach (var equipment in player.Equipment.Values.Where(x => x.IsEquipped))
                {
                    if (equipment.HasReligionRequirement &&
                        equipment.Religion == religion)
                    {
                        // Cursed equipment will turn on its owner
                        if (equipment.IsCursed)
                        {
                            RogueUpdateEvent(this, _rogueUpdateFactory.PlayerDeath("Cursed " + _modelService.GetDisplayName(equipment) + " turned on its onwer..."));
                            return LevelContinuationAction.DoNothing;
                        }

                        // Un-Equip item
                        else
                        {
                            equipment.IsEquipped = false;
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has lost use of their " + _modelService.GetDisplayName(equipment));
                        }
                    }
                }

                // TODO:SKILLSET
                // Religious Affiliated Skills
                //foreach (var skillSet in player.SkillSets.Where(x => x.IsLearned))
                //{
                //    if (skillSet.HasReligionRequirement &&
                //        skillSet.Religion == religion)
                //    {
                //        // Un-Learn skill set
                //        skillSet.IsLearned = false;
                //        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has lost use of the skill " + skillSet.RogueName);

                //        // Deactivate Skill
                //        if (skillSet.IsTurnedOn || skillSet.IsActive)
                //        {
                //            _playerProcessor.DeActivateSkills(player);

                //            // Update Player Symbol
                //            RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerLocation, player.Id));
                //        }

                //        // De-Select skill
                //        skillSet.DeSelectSkill();

                //        // Un-Learn skills
                //        skillSet.Skills.Where(x => x.IsLearned).ForEach(x =>
                //        {
                //            x.IsLearned = false;
                //            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has lost use of the skill " + x.Alteration.DisplayName);
                //        });
                //    }
                //}

                // Queue Animations
                if (animations.Any())
                    RogueUpdateEvent(this, 
                        _rogueUpdateFactory.Animation(animations, 
                                                      player.Location, 
                                                      _modelService.GetVisibleEnemies()
                                                                   .Select(x => x.Location)
                                                                   .Actualize()));

                // Queue Player Update
                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.PlayerAll, ""));

                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has renounced " + religion.RogueName);

                return LevelContinuationAction.ProcessTurn;
            }
        }

        public void IdentifyReligion(Religion religion)
        {
            var isIdentified = _modelService.ScenarioEncyclopedia[religion.RogueName].IsIdentified;

            if (!isIdentified)
            {
                // Identify the religion
                _modelService.ScenarioEncyclopedia[religion.RogueName].IsIdentified = true;

                // Trigger a message
                _scenarioMessageService.Publish(ScenarioMessagePriority.Unique, "The Religion \"{0}\" has been identified", religion.RogueName);

                RogueUpdateEvent(this, _rogueUpdateFactory.Update(LevelUpdateType.EncyclopediaIdentify, religion.Id));
            }
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }
    }
}
