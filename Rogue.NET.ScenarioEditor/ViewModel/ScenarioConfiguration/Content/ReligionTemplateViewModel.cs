using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligionTemplateViewModel : DungeonObjectTemplateViewModel
    {
        string _followerName;
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttributes;
        bool _allowsRenunciation;
        bool _allowsReAffiliation;
        double _bonusAttributeValue;
        CharacterAttribute _bonusAttribute;

        public string FollowerName
        {
            get { return _followerName; }
            set { this.RaiseAndSetIfChanged(ref _followerName, value); }
        }
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
        public ObservableCollection<ReligiousAffiliationAttackParametersTemplateViewModel> AttackParameters { get; set; }

        public ReligionTemplateViewModel()
        {
            this.FollowerName = "Christian";

            this.AttackParameters = new ObservableCollection<ReligiousAffiliationAttackParametersTemplateViewModel>();
            this.BonusAttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();

            this.AllowsRenunciation = true;
            this.AllowsReAffiliation = false;
        }
    }
}