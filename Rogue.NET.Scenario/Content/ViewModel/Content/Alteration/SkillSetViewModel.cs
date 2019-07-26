using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Scenario.Events.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.ScenarioMetaData;

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

        public IAsyncCommand ChangeSkillLevelUpCommand { get; set; }
        public IAsyncCommand ChangeSkillLevelDownCommand { get; set; }
        public IAsyncCommand ActivateSkillCommand { get; set; }
        public ICommand ViewSkillsCommand { get; set; }

        public ObservableCollection<SkillViewModel> Skills { get; set; }

        public SkillSetViewModel(SkillSet skillSet, Player player, IDictionary<string, ScenarioMetaDataClass> encyclopedia, IEventAggregator eventAggregator) : base(skillSet)
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
                    Alteration = new SpellViewModel(x.Alteration),
                    AttributeRequirement = x.AttributeRequirement,
                    AttributeLevelRequirement = x.AttributeLevelRequirement,
                    Description = encyclopedia[x.Alteration.RogueName].LongDescription,
                    IsLearned = x.IsLearned,
                    IsSkillPointRequirementMet = player.SkillPoints >= x.SkillPointRequirement || x.IsLearned,
                    IsLevelRequirementMet = player.Level >= x.LevelRequirement,
                    IsAttributeRequirementMet = !x.HasAttributeRequirement || 
                                                 player.GetAttribute(x.AttributeRequirement) > x.AttributeLevelRequirement,
                    IsReligionRequirementMet = !x.HasReligionRequirement ||
                                                (player.ReligiousAlteration.IsAffiliated() &&
                                                 player.ReligiousAlteration.Religion.RogueName == x.Religion.RogueName),
                    SkillPointRequirement = x.SkillPointRequirement,
                    Religion =  new ScenarioImageViewModel(x.Religion),
                    LevelRequirement = x.LevelRequirement,
                };
            }));

            this.ActiveSkill = skillSet.SelectedSkill != null ? this.Skills.First(x => x.Id == skillSet.SelectedSkill.Id) : null;

            // Hook-up commands
            this.ChangeSkillLevelDownCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ChangeSkillLevelDown, Compass.Null, this.Id));

            }, () => this.HasLearnedSkills);

            this.ChangeSkillLevelUpCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ChangeSkillLevelUp, Compass.Null, this.Id));

            }, () => this.HasLearnedSkills);

            this.ActivateSkillCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ActivateSkillSet, Compass.Null, this.Id));

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

            if (this.ChangeSkillLevelDownCommand != null)
                this.ChangeSkillLevelDownCommand.RaiseCanExecuteChanged();

            if (this.ChangeSkillLevelUpCommand != null)
                this.ChangeSkillLevelUpCommand.RaiseCanExecuteChanged();
        }
    }
}
