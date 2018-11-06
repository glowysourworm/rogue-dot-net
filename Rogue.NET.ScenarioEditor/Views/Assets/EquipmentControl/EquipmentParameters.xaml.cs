using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    public partial class EquipmentParameters : UserControl
    {
        public EquipmentParameters()
        {
            InitializeComponent();

            // TODO
            //this.AttackSpellCB.ItemsSource = config.MagicSpells;
            //this.CurseSpellCB.ItemsSource = config.MagicSpells;
            //this.EquipSpellCB.ItemsSource = config.MagicSpells;
            //this.AmmoTemplateCB.ItemsSource = config.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
        }
    }
}
