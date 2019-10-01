using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
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
    public partial class FriendlyDesign : UserControl
    {
        [ImportingConstructor]
        public FriendlyDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.FriendlyLB.SourceItemsSource = configurationData.Configuration.FriendlyTemplates;
                           });

            this.FriendlyLB.SourceItemsSource = scenarioCollectionProvider.Friendlies;
        }

        private void FriendlyLB_AddEvent(object sender, object e)
        {
            var viewModel = this.FriendlyLB.DestinationItemsSource as IList<FriendlyGenerationTemplateViewModel>;
            var friendly = e as FriendlyTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == friendly.Name))
                viewModel.Add(new FriendlyGenerationTemplateViewModel()
                {
                    Asset = friendly,
                    GenerationWeight = 1.0,
                    Name = friendly.Name
                });
        }

        private void FriendlyLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.FriendlyLB.DestinationItemsSource as IList<FriendlyGenerationTemplateViewModel>;
            var friendlyGeneration = e as FriendlyGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(friendlyGeneration);
        }
    }
}
