using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

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
            set { this.RaiseAndSetIfChanged(ref _emphasis, value); }
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
            set { this.RaiseAndSetIfChanged(ref _isLearned, value); }
        }
        public double SkillProgress
        {
            get { return _skillProgress; }
            set { this.RaiseAndSetIfChanged(ref _skillProgress, value); }
        }

        public SkillSetViewModel() { }
        public SkillSetViewModel(SkillSet skillSet) : base(skillSet)
        {
            this.Level = skillSet.Level;
            this.DisplayLevel = skillSet.DisplayLevel;
            this.LevelLearned = skillSet.LevelLearned;
            this.Emphasis = skillSet.Emphasis;
            this.IsActive = skillSet.IsActive;
            this.IsTurnedOn = skillSet.IsTurnedOn;
            this.IsLearned = skillSet.IsLearned;
            this.SkillProgress = skillSet.SkillProgress;
        }
    }
}
