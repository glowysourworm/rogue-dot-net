using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillViewModel : RogueBaseViewModel
    {
        int _levelRequirement;
        int _skillPointRequirement;
        double _attributeLevelRequirement;
        CharacterAttribute _attributeRequirement;
        string _description;
        ScenarioImageViewModel _religion;
        SpellViewModel _alteration;        

        bool _isLearned;
        bool _isActive;

        bool _hasAttributeRequirement;
        bool _hasReligionRequirement;

        bool _isSkillPointRequirementMet;
        bool _isLevelRequirementMet;
        bool _isAttributeRequirementMet;
        bool _isReligionRequirementMet;

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
        public ScenarioImageViewModel Religion
        {
            get { return _religion; }
            set { this.RaiseAndSetIfChanged(ref _religion, value); }
        }
        public SpellViewModel Alteration
        {
            get { return _alteration; }
            set { this.RaiseAndSetIfChanged(ref _alteration, value); }
        }

        public bool IsLearned
        {
            get { return _isLearned; }
            set { this.RaiseAndSetIfChanged(ref _isLearned, value); }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); }
        }

        public bool HasAttributeRequirement
        {
            get { return _hasAttributeRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasAttributeRequirement, value); }
        }
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligionRequirement, value); }
        }

        public bool IsSkillPointRequirementMet
        {
            get { return _isSkillPointRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isSkillPointRequirementMet, value); OnPropertyChanged("AreAllRequirementsMet"); }
        }
        public bool IsLevelRequirementMet
        {
            get { return _isLevelRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isLevelRequirementMet, value); OnPropertyChanged("AreAllRequirementsMet"); }
        }
        public bool IsAttributeRequirementMet
        {
            get { return _isAttributeRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isAttributeRequirementMet, value); OnPropertyChanged("AreAllRequirementsMet"); }
        }
        public bool IsReligionRequirementMet
        {
            get { return _isReligionRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isReligionRequirementMet, value); OnPropertyChanged("AreAllRequirementsMet"); }
        }
        public bool AreAllRequirementsMet
        {
            get
            {
                return _isSkillPointRequirementMet &&
                       _isLevelRequirementMet &&
                       _isReligionRequirementMet &&
                       _isAttributeRequirementMet;
            }
        }

        public ICommand UnlockCommand { get; set; }
        public ICommand ActivateCommand { get; set; }

        public SkillViewModel(Skill skill, IEventAggregator eventAggregator) : base(skill)
        {
            this.UnlockCommand = new DelegateCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(new UserCommandEventArgs()
                {
                    LevelAction = LevelActionType.UnlockSkill,
                    ItemId = this.Id
                });
            });

            this.ActivateCommand = new DelegateCommand(async () =>
            {
                await eventAggregator.GetEvent<UserCommandEvent>().Publish(new UserCommandEventArgs()
                {
                    LevelAction = LevelActionType.ActivateSkill,
                    ItemId = this.Id
                });
            });
        }
    }
}
