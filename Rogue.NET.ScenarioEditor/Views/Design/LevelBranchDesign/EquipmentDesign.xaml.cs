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
    public partial class EquipmentDesign : UserControl
    {
        [ImportingConstructor]
        public EquipmentDesign(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               this.EquipmentLB.SourceItemsSource = configurationData.Configuration.EquipmentTemplates;
                           });

            this.EquipmentLB.SourceItemsSource = scenarioCollectionProvider.Equipment;
        }

        private void EquipmentLB_AddEvent(object sender, object e)
        {
            var viewModel = this.EquipmentLB.DestinationItemsSource as IList<EquipmentGenerationTemplateViewModel>;
            var equipment = e as EquipmentTemplateViewModel;

            if (viewModel != null &&
                viewModel.None(x => x.Name == equipment.Name))
                viewModel.Add(new EquipmentGenerationTemplateViewModel()
                {
                    Asset = equipment,
                    GenerationWeight = 1.0,
                    Name = equipment.Name
                });
        }

        private void EquipmentLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.EquipmentLB.DestinationItemsSource as IList<EquipmentGenerationTemplateViewModel>;
            var equipmentGeneration = e as EquipmentGenerationTemplateViewModel;

            if (viewModel != null)
                viewModel.Remove(equipmentGeneration);
        }
    }
}
