using Rogue.NET.Core.Logic.Interface;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Religion;

namespace Rogue.NET.Core.Logic
{
    [Export(typeof(IReligionEngine))]
    public class ReligionEngine : IReligionEngine
    {
        readonly IPlayerProcessor _playerProcessor;
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;

        public event EventHandler<IScenarioUpdate> ScenarioUpdateEvent;
        public event EventHandler<ISplashUpdate> SplashUpdateEvent;
        public event EventHandler<IDialogUpdate> DialogUpdateEvent;
        public event EventHandler<ILevelUpdate> LevelUpdateEvent;
        public event EventHandler<IAnimationUpdate> AnimationUpdateEvent;
        public event EventHandler<ILevelProcessingAction> LevelProcessingActionEvent;

        [ImportingConstructor]
        public ReligionEngine(
            IPlayerProcessor playerProcessor,
            IModelService modelService, 
            IScenarioMessageService scenarioMessageService)
        {
            _playerProcessor = playerProcessor;
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
        }

        public void Affiliate(string religionName, double affiliationLevel)
        {
            var player = _modelService.Player;

            // Non-Affiliated
            if (!player.ReligiousAlteration.IsAffiliated())
            {
                // Get Religion
                var religion = _modelService.Religions.First(x => x.RogueName == religionName);

                // Set new affiliation
                player.ReligiousAlteration.Affiliate(religion, affiliationLevel);
            }
            // Keeps current religion
            else if (player.ReligiousAlteration.ReligionName == religionName)
            {
                // Increase existing affiliation level
                player.ReligiousAlteration.SetAffiliationLevel(player.ReligiousAlteration.Affiliation + affiliationLevel);
            }
            else
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
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.ReligiousAlteration.ReligionName + " does not allow Renunciation!");
                return LevelContinuationAction.DoNothing;
            }

            // Renounces
            else
            {
                // Religion Name
                var religionName = player.ReligiousAlteration.ReligionName;

                // Renunciation Animations
                var animations = player.ReligiousAlteration.Renounce();

                // Religious Affiliated Equipmpent
                foreach (var equipment in player.Equipment.Values.Where(x => x.IsEquipped))
                {
                    if (equipment.HasReligiousAffiliationRequirement &&
                        equipment.ReligiousAffiliationRequirement.ReligionName == religionName)
                    {
                        // Cursed equipment will turn on its owner
                        if (equipment.IsCursed)
                        {
                            QueueScenarioPlayerDeath("Cursed " + _modelService.GetDisplayName(equipment) + " turned on its onwer...");
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

                // Religious Affiliated Skills
                foreach (var skillSet in player.SkillSets.Where(x => x.IsLearned))
                {
                    if (skillSet.HasReligiousAffiliationRequirement &&
                        skillSet.ReligiousAffiliationRequirement.ReligionName == religionName)
                    {
                        // Un-Learn skill set
                        skillSet.IsLearned = false;
                        _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has lost use of the skill " + skillSet.RogueName);

                        // Deactivate Skill
                        if (skillSet.IsTurnedOn || skillSet.IsActive)
                        {
                            _playerProcessor.DeActivateSkills(player);

                            // Update Player Symbol
                            QueueLevelUpdate(LevelUpdateType.PlayerLocation, player.Id);
                        }

                        // De-Select skill
                        skillSet.DeSelectSkill();

                        // Un-Learn skills
                        skillSet.Skills.Where(x => x.IsLearned).ForEach(x =>
                        {
                            x.IsLearned = false;
                            _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has lost use of the skill " + x.Alteration.DisplayName);
                        });
                    }
                }

                // Queue Animations
                if (animations.Any())
                    AnimationUpdateEvent(this, new AnimationUpdate()
                    {
                        Animations = animations,
                        SourceLocation = player.Location,
                        TargetLocations = _modelService.GetVisibleEnemies()
                                                       .Select(x => x.Location)
                                                       .Actualize()
                    });

                // Queue Player Update
                LevelUpdateEvent(this, new LevelUpdate()
                {
                    LevelUpdateType = LevelUpdateType.PlayerAll
                });

                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, player.RogueName + " has renounced " + religionName);

                return LevelContinuationAction.ProcessTurn;
            }
        }

        public void ApplyEndOfTurn()
        {
            throw new NotImplementedException();
        }

        #region (private) Event Methods
        private void QueueLevelUpdate(LevelUpdateType type, string contentId)
        {
            LevelUpdateEvent(this, new LevelUpdate()
            {
                LevelUpdateType = type,
                ContentIds = new string[] { contentId }
            });
        }
        private void QueueScenarioPlayerDeath(string deathMessage)
        {
            ScenarioUpdateEvent(this, new ScenarioUpdate()
            {
                ScenarioUpdateType = ScenarioUpdateType.PlayerDeath,
                PlayerDeathMessage = deathMessage
            });
        }
        #endregion
    }
}
