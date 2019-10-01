using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.ScenarioEditor.Views.Design.LevelBranchDesign
{
    [Export]
    public partial class LayoutDesign : UserControl
    {
        [ImportingConstructor]
        public LayoutDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.LayoutLB.SourceItemsSource = configurationData.Configuration.LayoutTemplates;
                           });

            this.LayoutLB.SourceItemsSource = scenarioCollectionProvider.Layouts;
        }

        private void LayoutLB_AddEvent(object sender, object e)
        {
            var viewModel = this.LayoutLB.DestinationItemsSource as IList<LayoutGenerationTemplateViewModel>;
            var layout = e as LayoutTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == layout.Name))
                viewModel.Add(new LayoutGenerationTemplateViewModel()
                {
                    Asset = layout,
                    GenerationWeight = 1.0,
                    Name = layout.Name
                });
        }

        private void LayoutLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.LayoutLB.DestinationItemsSource as IList<LayoutGenerationTemplateViewModel>;
            var layoutGeneration = e as LayoutGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(layoutGeneration);
        }
    }
}
