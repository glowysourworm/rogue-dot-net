using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EquipmentControl
{
    [Export]
    public partial class EquipmentParameters : UserControl
    {
        [ImportingConstructor]
        public EquipmentParameters(
                IRogueEventAggregator eventAggregator,
                IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.AmmoTemplateCB.ItemsSource = provider.Consumables.Where(a => a.SubType == ConsumableSubType.Ammo);
            this.CharacterClassCB.ItemsSource = provider.CharacterClasses;
        }
    }
}
