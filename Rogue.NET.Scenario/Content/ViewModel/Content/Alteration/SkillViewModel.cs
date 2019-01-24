using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillViewModel : RogueBaseViewModel
    {
        int _levelRequirement;
        int _skillPointRequirement;
        bool _hasReligiousAffiliationRequirement;
        double _religiousAffiliationRequirementLevel;
        string _religiousAffiliationRequirementName;
        string _description;
        SpellViewModel _alteration;

        bool _isLearned;
        bool _isActive;
        bool _isSkillPointRequirementMet;
        bool _isLevelRequirementMet;
        bool _isReligiousAffiliationRequirementMet;

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
        public bool HasReligiousAffiliationRequirement
        {
            get { return _hasReligiousAffiliationRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligiousAffiliationRequirement, value); }
        }
        public double ReligiousAffiliationRequirementLevel
        {
            get { return _religiousAffiliationRequirementLevel; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliationRequirementLevel, value); }
        }
        public string ReligiousAffiliationRequirementName
        {
            get { return _religiousAffiliationRequirementName; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliationRequirementName, value); }
        }
        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
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

        // Necessary for data binding
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
        public bool IsReligiousAffiliationRequirementMet
        {
            get { return _isReligiousAffiliationRequirementMet; }
            set { this.RaiseAndSetIfChanged(ref _isReligiousAffiliationRequirementMet, value); OnPropertyChanged("AreAllRequirementsMet"); }
        }
        public bool AreAllRequirementsMet
        {
            get
            {
                return _isSkillPointRequirementMet &&
                       _isLevelRequirementMet &&
                       (!_hasReligiousAffiliationRequirement ||
                         _isReligiousAffiliationRequirementMet);
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
