using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class SkillSetViewModel : ScenarioImageViewModel
    {
        public int _level;
        public int _levelMax;
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
            set { this.RaiseAndSetIfChanged(ref _level, value); InvalidateCommands(); }
        }
        public int LevelMax
        {
            get { return _levelMax; }
            set { this.RaiseAndSetIfChanged(ref _levelMax, value); }
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

        public IAsyncCommand EmphasizeSkillUpCommand { get; set; }
        public IAsyncCommand EmphasizeSkillDownCommand { get; set; }
        public IAsyncCommand ActivateSkillCommand { get; set; }

        public SkillSetViewModel(SkillSet skillSet, IEventAggregator eventAggregator) : base(skillSet)
        {
            this.Level = skillSet.Level;
            this.LevelMax = skillSet.Skills.Count;
            this.DisplayLevel = skillSet.DisplayLevel;
            this.LevelLearned = skillSet.LevelLearned;
            this.Emphasis = skillSet.Emphasis;
            this.IsActive = skillSet.IsActive;
            this.IsTurnedOn = skillSet.IsTurnedOn;
            this.IsLearned = skillSet.IsLearned;
            this.SkillProgress = skillSet.SkillProgress;


            // Hook-up commands
            this.EmphasizeSkillDownCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelAction.EmphasizeSkillDown, Compass.Null, this.Id));

            }, () => this.IsLearned && this.Emphasis > 0 && this.Level < this.LevelMax);

            this.EmphasizeSkillUpCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelAction.EmphasizeSkillUp, Compass.Null, this.Id));

            }, () => this.IsLearned && this.Emphasis < 3 && this.Level < this.LevelMax);

            this.ActivateSkillCommand = new AsyncCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(
                    new UserCommandEventArgs(LevelAction.ActivateSkill, Compass.Null, this.Id));

            }, () => this.IsLearned);
        }

        private void InvalidateCommands()
        {
            if (this.ActivateSkillCommand != null)
                this.ActivateSkillCommand.RaiseCanExecuteChanged();

            if (this.EmphasizeSkillDownCommand != null)
                this.EmphasizeSkillDownCommand.RaiseCanExecuteChanged();

            if (this.EmphasizeSkillUpCommand != null)
                this.EmphasizeSkillUpCommand.RaiseCanExecuteChanged();
        }
    }
}
