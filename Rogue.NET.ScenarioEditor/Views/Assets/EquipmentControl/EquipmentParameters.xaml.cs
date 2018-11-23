using Prism.Events;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentParameters : UserControl
    {
        [ImportingConstructor]
        public EquipmentParameters(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.CurseSpellCB.ItemsSource = configuration.MagicSpells;
                this.EquipSpellCB.ItemsSource = configuration.MagicSpells;
                this.AmmoTemplateCB.ItemsSource = configuration.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe((configuration) =>
            {
                this.AmmoTemplateCB.ItemsSource = configuration.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
            });
        }
    }
}
