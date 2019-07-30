using Prism.Commands;
using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Command;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Windows;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillViewModel : RogueBaseViewModel
    {
        public static readonly DependencyProperty IsLearnedProperty =
            DependencyProperty.Register("IsLearned", typeof(bool), typeof(SkillViewModel), new PropertyMetadata(false));

        public static readonly DependencyProperty AreAllRequirementsMetProperty =
            DependencyProperty.Register("AreAllRequirementsMet", typeof(bool), typeof(SkillViewModel), new PropertyMetadata(false));

        int _levelRequirement;
        int _skillPointRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        string _description;
        ScenarioImageViewModel _characterClass;
        SpellViewModel _alteration;        

        bool _isSelected;

        bool _hasAttributeRequirement;
        bool _hasCharacterClassRequirement;

        bool _isSkillPointRequirementMet;
        bool _isLevelRequirementMet;
        bool _isAttributeRequirementMet;
        bool _isCharacterClassRequirementMet;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _levelRequirement, value); }
        }
        public int SkillPointRequirement
        {
            get { return _skillPointRequirement; }
            set { this.RaiseAndSetIfChanged(ref _skillPointRequirement, value); }
        }
        public double AttributeLevelRequirement
        {
            get { return _attributeLevelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _attributeLevelRequirement, value); }
        }
        public CharacterAttribute AttributeRequirement
        {
            get { return _attributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _attributeRequirement, value); }
        }
        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }
        public ScenarioImageViewModel CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }
        public SpellViewModel Alteration
        {
            get { return _alteration; }
            set { this.RaiseAndSetIfChanged(ref _alteration, value); }
        }

        public bool IsLearned
        {
            get { return (bool)GetValue(IsLearnedProperty); }
            set { SetValue(IsLearnedProperty, value); }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public bool HasAttributeRequirement
        {
            get { return _hasAttributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasAttributeRequirement, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }

        public bool IsSkillPointRequirementMet
        {
            get { return _isSkillPointRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isSkillPointRequirementMet, value); UpdateAreAllRequirementsMet(); }
        }
        public bool IsLevelRequirementMet
        {
            get { return _isLevelRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isLevelRequirementMet, value); UpdateAreAllRequirementsMet(); }
        }
        public bool IsAttributeRequirementMet
        {
            get { return _isAttributeRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isAttributeRequirementMet, value); UpdateAreAllRequirementsMet(); }
        }
        public bool IsCharacterClassRequirementMet
        {
            get { return _isCharacterClassRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isCharacterClassRequirementMet, value); UpdateAreAllRequirementsMet(); }
        }
        public bool AreAllRequirementsMet
        {
            get { return (bool)GetValue(AreAllRequirementsMetProperty); }
        }

        public ICommand UnlockCommand { get; set; }
        public ICommand SelectCommand { get; set; }

        public SkillViewModel(Skill skill, IEventAggregator eventAggregator) : base(skill)
        {
            this.UnlockCommand = new DelegateCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>()
                                     .Publish(new PlayerCommandEventArgs(PlayerActionType.UnlockSkill, this.Id));
            });

            this.SelectCommand = new DelegateCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>()
                                     .Publish(new PlayerCommandEventArgs(PlayerActionType.SelectSkill, this.Id));
            });
        }

        protected void UpdateAreAllRequirementsMet()
        {
            SetValue(AreAllRequirementsMetProperty, _isSkillPointRequirementMet &&
                                                    _isLevelRequirementMet &&
                                                    _isCharacterClassRequirementMet &&
                                                    _isAttributeRequirementMet);

            OnPropertyChanged("AreAllRequirementsMet");
        }
    }
}
