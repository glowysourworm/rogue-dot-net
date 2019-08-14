using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentParameters : UserControl
    {
        [ImportingConstructor]
        public EquipmentParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                // TODO:ALTERATION
                //this.CurseSpellCB.ItemsSource = configuration.MagicSpells;
                //this.EquipSpellCB.ItemsSource = configuration.MagicSpells;
                this.AmmoTemplateCB.ItemsSource = configuration.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
                this.CharacterClassCB.ItemsSource = configuration.CharacterClasses;
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe((configuration) =>
            {
                this.AmmoTemplateCB.ItemsSource = configuration.ConsumableTemplates.Where(a => a.SubType == ConsumableSubType.Ammo);
            });
        }
    }
}
