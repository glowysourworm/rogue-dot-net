using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class ReligionViewModel : Image
    {
        public string RogueName { get; set; }
        public ScenarioMetaDataViewModel ReligionMetaData { get; set; }
        public bool HasBonusSkillSet { get; set; }
        public bool HasBonusAttribute { get; set; }
        public bool HasBonusAttackAttributes { get; set; }
        public ScenarioMetaDataViewModel SkillSetMetaData { get; set; }
        public CharacterAttribute BonusAttribute { get; set; }
        public double BonusAttributeValue { get; set; }
        public ObservableCollection<AttackAttribute> BonusAttackAttributes { get; set; }

        public ReligionViewModel()
        {
            this.BonusAttackAttributes = new ObservableCollection<AttackAttribute>();
        }
    }
}
