using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class CharacterClassTemplateViewModel : DungeonObjectTemplateViewModel
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttributes;
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
        public CharacterClassTemplateViewModel()
        {
            this.BonusAttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.HasAttributeBonus = false;
            this.HasBonusAttackAttributes = false;
        }
    }
}