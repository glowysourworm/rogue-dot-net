using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class SkillSetViewModel : ScenarioImageViewModel
    {
        public int _level;
        public int _displayLevel;
        public int _levelLearned;
        public int _emphasis;
        public bool _isActive;
        public bool _isTurnedOn;
        public bool _isLearned;
        public double _skillProgress;

        public int Level
        {
            get { return _level; }
            set { this.RaiseAndSetIfChanged(ref _level, value); }
        }
        public int DisplayLevel
        {
            get { return _displayLevel; }
            set { this.RaiseAndSetIfChanged(ref _displayLevel, value); }
        }
        public int LevelLearned
        {
            get { return _levelLearned; }
            set { this.RaiseAndSetIfChanged(ref _levelLearned, value); }
        }
        public int Emphasis
        {
            get { return _emphasis; }
            set { this.RaiseAndSetIfChanged(ref _emphasis, value); InvalidateCommands(); }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); }
        }
        public bool IsTurnedOn
        {
            get { return _isTurnedOn; }
            set { this.RaiseAndSetIfChanged(ref _isTurnedOn, value); }
        }
        public bool IsLearned
        {
            get { return _isLearned; }
            set { this.RaiseAndSetIfChanged(ref _isLearned, value); InvalidateCommands(); }
        }
        public double SkillProgress
        {
            get { return _skillProgress; }
            set { this.RaiseAndSetIfChanged(ref _skillProgress, value); }
        }

        public ICommand EmphasizeSkillUpCommand { get; set; }
        public ICommand EmphasizeSkillDownCommand { get; set; }
        public ICommand ActivateSkillCommand { get; set; }

        readonly IEventAggregator _eventAggregator;

        public SkillSetViewModel(SkillSet skillSet, IEventAggregator eventAggregator) : base(skillSet)
        {
            _eventAggregator = eventAggregator;

            this.Level = skillSet.Level;
            this.DisplayLevel = skillSet.DisplayLevel;
            this.LevelLearned = skillSet.LevelLearned;
            this.Emphasis = skillSet.Emphasis;
            this.IsActive = skillSet.IsActive;
            this.IsTurnedOn = skillSet.IsTurnedOn;
            this.IsLearned = skillSet.IsLearned;
            this.SkillProgress = skillSet.SkillProgress;

            // Hook-up commands
            this.EmphasizeSkillDownCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new LevelCommandEventArgs(LevelAction.EmphasizeSkillDown, Compass.Null, this.Id));

            }, () => this.IsLearned && this.Emphasis > 0);

            this.EmphasizeSkillUpCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new LevelCommandEventArgs(LevelAction.EmphasizeSkillUp, Compass.Null, this.Id));

            }, () => this.IsLearned && this.Emphasis < 3);

            this.ActivateSkillCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new LevelCommandEventArgs(LevelAction.ActivateSkill, Compass.Null, this.Id));

            }, () => this.IsLearned);
        }

        private void InvalidateCommands()
        {
            OnPropertyChanged("EmphasizeSkillDownCommand");
            OnPropertyChanged("EmphasizeSkillUpCommand");
            OnPropertyChanged("ActivateSkillCommand");
        }
    }
}
