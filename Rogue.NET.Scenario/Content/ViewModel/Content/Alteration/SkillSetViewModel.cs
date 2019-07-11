using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Scenario.Events.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillSetViewModel : ScenarioImageViewModel
    {
        public int _levelMax;
        public int _levelLearned;
        public bool _isActive;
        public bool _isTurnedOn;
        public bool _isLearned;
        public bool _hasReligionRequirement;
        public bool _hasLearnedSkills;
        public double _skillProgress;
        public string _religionName;

        SkillViewModel _activeSkill;

        public int LevelMax
        {
            get { return _levelMax; }
            set { this.RaiseAndSetIfChanged(ref _levelMax, value); }
        }
        public int LevelLearned
        {
            get { return _levelLearned; }
            set { this.RaiseAndSetIfChanged(ref _levelLearned, value); }
        }
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
        public bool IsLearned
        {
            get { return _isLearned; }
            set { this.RaiseAndSetIfChanged(ref _isLearned, value); InvalidateCommands(); }
        }
        public bool HasLearnedSkills
        {
            get { return _hasLearnedSkills; }
            set { this.RaiseAndSetIfChanged(ref _hasLearnedSkills, value); InvalidateCommands(); }
        }
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligionRequirement, value); InvalidateCommands(); }
        }
        public string ReligionName
        {
            get { return _religionName; }
            set { this.RaiseAndSetIfChanged(ref _religionName, value); }
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
            this.LevelMax = skillSet.Skills.Count;
            this.LevelLearned = skillSet.LevelLearned;
            this.IsActive = skillSet.IsActive;
            this.IsTurnedOn = skillSet.IsTurnedOn;
            this.IsLearned = skillSet.IsLearned;

            this.HasLearnedSkills = skillSet.Skills.Any(x => x.IsLearned);
            this.HasReligionRequirement = skillSet.HasReligionRequirement;
            this.ReligionName = skillSet.Religion.RogueName;

            this.Skills = new ObservableCollection<SkillViewModel>(skillSet.Skills.Select(x =>
            {
                return new SkillViewModel(x, eventAggregator)
                {
                    Alteration = new SpellViewModel(x.Alteration),
                    Description = encyclopedia[x.Alteration.RogueName].LongDescription,
                    IsLearned = x.IsLearned,
                    IsSkillPointRequirementMet = player.SkillPoints >= x.SkillPointRequirement,
                    IsLevelRequirementMet = player.Level >= x.LevelRequirement,
                    SkillPointRequirement = x.SkillPointRequirement,
                    LevelRequirement = x.LevelRequirement,
                };
            }));

            this.ActiveSkill = skillSet.SelectedSkill != null ? this.Skills.First(x => x.Id == skillSet.SelectedSkill.Id) : null;

            // Hook-up commands
            this.ChangeSkillLevelDownCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ChangeSkillLevelDown, Compass.Null, this.Id));

            }, () => this.IsLearned);

            this.ChangeSkillLevelUpCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ChangeSkillLevelUp, Compass.Null, this.Id));

            }, () => this.IsLearned);

            this.ActivateSkillCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelActionType.ActivateSkillSet, Compass.Null, this.Id));

            }, () => this.IsLearned);

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
