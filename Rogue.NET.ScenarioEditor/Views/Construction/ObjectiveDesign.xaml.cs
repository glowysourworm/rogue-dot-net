using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    public partial class ObjectiveDesign : UserControl
    {
        public ObjectiveDesign()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfigurationContainer config)
        {
            this.DoodadLB.ItemsSource = config.DoodadTemplates;
            this.EnemyLB.ItemsSource = config.EnemyTemplates;
            this.ItemsLB.ItemsSource = config.ConsumableTemplates.
                                            Cast<DungeonObjectTemplate>().
                                            Union(config.EquipmentTemplates.
                                                Cast<DungeonObjectTemplate>());
        }
    }
}
