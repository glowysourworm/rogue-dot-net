using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class ReligionSelectionViewModel : Image
    {
        public string RogueName { get; set; }
        public ScenarioMetaDataViewModel ReligionMetaData { get; set; }
        public bool HasBonusAttribute { get; set; }
        public bool HasBonusAttackAttributes { get; set; }
        public CharacterAttribute BonusAttribute { get; set; }
        public double BonusAttributeValue { get; set; }
        public ObservableCollection<AttackAttribute> BonusAttackAttributes { get; set; }

        public ReligionSelectionViewModel()
        {
            this.BonusAttackAttributes = new ObservableCollection<AttackAttribute>();
        }
    }
}
