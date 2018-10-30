using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Equipment
{
    public partial class EquipmentParameters : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EquipmentParameters()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EquipmentAttackAttributes); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var config = containerViewModel.SecondaryPayload as ScenarioConfigurationContainer;
            if (config != null)
            {
                this.AttackSpellCB.ItemsSource = config.MagicSpells;
                this.CurseSpellCB.ItemsSource = config.MagicSpells;
                this.EquipSpellCB.ItemsSource = config.MagicSpells;
                this.AmmoTemplateCB.ItemsSource = config.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
            }
        }
    }
}
