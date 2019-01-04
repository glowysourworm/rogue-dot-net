using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligionTemplateViewModel : DungeonObjectTemplateViewModel
    {
        bool _hasBonusAttribute;
        bool _hasBonusAttackAttribute;
        CharacterAttribute _bonusAttribute;
        AttackAttributeTemplateViewModel _bonusAttackAttribute;

        public bool HasAttributeBonus
        {
            get { return _hasBonusAttribute; }
            set { this.RaiseAndSetIfChanged(ref _hasBonusAttribute, value); }
        }
        public bool HasBonusAttackAttribute
        {
            get { return _hasBonusAttackAttribute; }
            set { this.RaiseAndSetIfChanged(ref _hasBonusAttackAttribute, value); }
        }
        public CharacterAttribute BonusAttribute
        {
            get { return _bonusAttribute; }
            set { this.RaiseAndSetIfChanged(ref _bonusAttribute, value); }
        }
        public AttackAttributeTemplateViewModel BonusAttackAttribute
        {
            get { return _bonusAttackAttribute; }
            set { this.RaiseAndSetIfChanged(ref _bonusAttackAttribute, value); }
        }

        public ObservableCollection<ReligiousAffiliationAttackParametersTemplateViewModel> AttackParameters { get; set; }

        public ReligionTemplateViewModel()
        {
            this.AttackParameters = new ObservableCollection<ReligiousAffiliationAttackParametersTemplateViewModel>();
        }
    }
}