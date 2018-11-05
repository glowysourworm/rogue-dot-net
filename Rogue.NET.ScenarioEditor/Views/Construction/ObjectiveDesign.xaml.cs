using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class ObjectiveDesign : UserControl
    {
        public ObjectiveDesign()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfigurationContainerViewModel config)
        {
            this.DoodadLB.ItemsSource = config.DoodadTemplates;
            this.EnemyLB.ItemsSource = config.EnemyTemplates;
            this.ItemsLB.ItemsSource = config.ConsumableTemplates.
                                            Cast<DungeonObjectTemplateViewModel>().
                                            Union(config.EquipmentTemplates.
                                                Cast<DungeonObjectTemplateViewModel>());
        }
    }
}
