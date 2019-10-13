using Prism.Commands;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Scenario.Processing.Event.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.Abstract.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillSetViewModel : ScenarioImageViewModel
    {
        public static readonly DependencyProperty HasLearnedUnavailableSkillsProperty =
            DependencyProperty.Register("HasLearnedUnavailableSkills", typeof(bool), typeof(SkillSetViewModel), new PropertyMetadata(false));

        bool _isActive;
        bool _isTurnedOn;
        bool _hasLearnedSkills;
        bool _hasUnlearnedSkills;
        bool _hasUnlearnedAvailableSkills;

        SkillViewModel _activeSkill;

        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); InvalidateCommands(); }
        }
        public bool IsTurnedOn
        {
            get { return _isTurnedOn; }
            set { this.RaiseAndSetIfChanged(ref _isTurnedOn, value); InvalidateCommands(); }
        }
        public bool HasLearnedSkills
        {
            get { return _hasLearnedSkills; }
            set { this.RaiseAndSetIfChanged(ref _hasLearnedSkills, value); InvalidateCommands(); }
        }
        public bool HasUnlearnedSkills
        {
            get { return _hasUnlearnedSkills; }
            set { this.RaiseAndSetIfChanged(ref _hasUnlearnedSkills, value); InvalidateCommands(); }
        }
        public bool HasUnlearnedAvailableSkills
        {
            get { return _hasUnlearnedAvailableSkills; }
            set { this.RaiseAndSetIfChanged(ref _hasUnlearnedAvailableSkills, value); InvalidateCommands(); }
        }
        public bool HasLearnedUnavailableSkills
        {
            get { return (bool)GetValue(HasLearnedUnavailableSkillsProperty); }
            set { SetValue(HasLearnedUnavailableSkillsProperty, value); InvalidateCommands(); }
        }
        public SkillViewModel ActiveSkill
        {
            get { return _activeSkill; }
            set { this.RaiseAndSetIfChanged(ref _activeSkill, value); }
        }

        public ISimpleAsyncCommand ActivateSkillCommand { get; set; }
        public ICommand ViewSkillsCommand { get; set; }

        public ObservableCollection<SkillViewModel> Skills { get; set; }

        public SkillSetViewModel(SkillSet skillSet, Player player, IRogueEventAggregator eventAggregator) : base(skillSet, skillSet.RogueName)
        {
            this.IsActive = skillSet.IsActive;
            this.IsTurnedOn = skillSet.IsTurnedOn;
            this.HasLearnedSkills = skillSet.Skills.Any(x => x.IsLearned);
            this.HasUnlearnedSkills = skillSet.Skills.Any(x => !x.IsLearned);
            this.HasUnlearnedAvailableSkills = skillSet.Skills.Any(x => !x.IsLearned && x.AreRequirementsMet(player));
            this.HasLearnedUnavailableSkills = skillSet.Skills.Any(x => x.IsLearned && !x.AreRequirementsMet(player));

            this.Skills = new ObservableCollection<SkillViewModel>(skillSet.Skills.Select(x =>
            {
                return new SkillViewModel(x, eventAggregator)
                {
                    Alteration = new AlterationViewModel(x.Alteration.Effect as IAlterationEffectTemplate, 
                                                         x.Alteration.Cost, 
                                                         x.Alteration.Effect.GetSupportsBlocking(x.Alteration), 
                                                         x.Alteration.Effect.GetCostType(x.Alteration),
                                                         x.Alteration.BlockType),
                    AttributeRequirement = x.AttributeRequirement,
                    AttributeLevelRequirement = x.AttributeLevelRequirement,
                    HasAttributeRequirement = x.HasAttributeRequirement,
                    HasCharacterClassRequirement = x.HasCharacterClassRequirement,
                    IsLearned = x.IsLearned,
                    IsSkillPointRequirementMet = player.SkillPoints >= x.SkillPointRequirement || x.IsLearned,
                    IsLevelRequirementMet = player.Level >= x.LevelRequirement,
                    IsAttributeRequirementMet = !x.HasAttributeRequirement || 
                                                 player.GetAttribute(x.AttributeRequirement) > x.AttributeLevelRequirement,
                    IsCharacterClassRequirementMet = !x.HasCharacterClassRequirement ||
                                                      player.Class == x.CharacterClass,
                    SkillPointRequirement = x.SkillPointRequirement,
                    CharacterClass =  new ScenarioImageViewModel(player, player.RogueName)
                    {
                        // TODO: Remove one of these
                        DisplayName = player.Class,
                        RogueName = player.Class
                    },
                    LevelRequirement = x.LevelRequirement,
                };
            }));

            this.ActiveSkill = skillSet.SelectedSkill != null ? this.Skills.First(x => x.Id == skillSet.SelectedSkill.Id) : null;

            // Hook-up commands
            this.ActivateSkillCommand = new SimpleAsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<PlayerCommand>().Publish(
                    new PlayerCommandData(PlayerCommandType.ActivateSkillSet, this.Id));

            }, () => this.HasLearnedSkills);

            this.ViewSkillsCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<RequestNavigateToSkillTreeEvent>().Publish();
            });
        }

        private void InvalidateCommands()
        {
            if (this.ActivateSkillCommand != null)
                this.ActivateSkillCommand.RaiseCanExecuteChanged();
        }
    }
}
