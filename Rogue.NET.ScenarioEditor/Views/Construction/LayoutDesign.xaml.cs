using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class LayoutDesign : UserControl
    {
        [ImportingConstructor]
        public LayoutDesign()
        {
            InitializeComponent();

            this.DataContextChanged += LayoutDesign_DataContextChanged;
        }

        private void LayoutDesign_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var config = e.NewValue as ScenarioConfigurationContainerViewModel;
            if (config == null)
                return;

            this.DataContext = new PlacementGroupViewModel(config
                    .DungeonTemplate
                    .LayoutTemplates
                    .Select(template => new PlacementViewModel()
                    {
                        ImageSource = null,
                        Template = template
                    }));
        }
    }
}
