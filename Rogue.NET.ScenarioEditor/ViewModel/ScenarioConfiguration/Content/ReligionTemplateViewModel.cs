using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligionTemplateViewModel : DungeonObjectTemplateViewModel
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttributes;
        bool _hasBonusSkillSet;
        bool _allowsRenunciation;
        bool _allowsReAffiliation;
        bool _isIdentified;
        bool _canStartWith;
        double _bonusAttributeValue;
        CharacterAttribute _bonusAttribute;

        public bool HasAttributeBonus
        {
            get { return _hasBonusAttribute; }
            set { this.RaiseAndSetIfChanged(ref _hasBonusAttribute, value); }
        }
        public bool HasBonusAttackAttributes
        {
            get { return _hasBonusAttackAttributes; }
            set { this.RaiseAndSetIfChanged(ref _hasBonusAttackAttributes, value); }
        }
        public bool AllowsRenunciation
        {
            get { return _allowsRenunciation; }
            set { this.RaiseAndSetIfChanged(ref _allowsRenunciation, value); }
        }
        public bool AllowsReAffiliation
        {
            get { return _allowsReAffiliation; }
            set { this.RaiseAndSetIfChanged(ref _allowsReAffiliation, value); }
        }
        public bool IsIdentified
        {
            get { return _isIdentified; }
            set { this.RaiseAndSetIfChanged(ref _isIdentified, value); }
        }
        public bool CanStartWith
        {
            get { return _canStartWith; }
            set { this.RaiseAndSetIfChanged(ref _canStartWith, value); }
        }
        public double BonusAttributeValue
        {
            get { return _bonusAttributeValue; }
            set { this.RaiseAndSetIfChanged(ref _bonusAttributeValue, value); }
        }
        public CharacterAttribute BonusAttribute
        {
            get { return _bonusAttribute; }
            set { this.RaiseAndSetIfChanged(ref _bonusAttribute, value); }
        }

        public ObservableCollection<AttackAttributeTemplateViewModel> BonusAttackAttributes { get; set; }
        public ObservableCollection<AnimationTemplateViewModel> RenunciationAnimations { get; set; }

        public ReligionTemplateViewModel()
        {
            this.BonusAttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.RenunciationAnimations = new ObservableCollection<AnimationTemplateViewModel>();

            this.AllowsRenunciation = true;
            this.AllowsReAffiliation = false;
            this.HasAttributeBonus = false;
            this.HasBonusAttackAttributes = false;
        }
    }
}