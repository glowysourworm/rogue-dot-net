using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    /// <summary>
    /// Interaction logic for ObjectiveDesign.xaml
    /// </summary>
    public partial class ObjectiveDesign : UserControl
    {
        public ObjectiveDesign()
        {
            InitializeComponent();
        }

        public void SetConfigurationParameters(ScenarioConfiguration config)
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
