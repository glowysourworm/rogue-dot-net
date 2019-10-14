using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class BlockEffectParameters : UserControl
    {
        [ImportingConstructor]
        public BlockEffectParameters(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configurationData =>
            {
                this.AlterationCategoryCB.ItemsSource = scenarioCollectionProvider.AlterationCategories;
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(collectionProvider =>
            {
                this.AlterationCategoryCB.ItemsSource = scenarioCollectionProvider.AlterationCategories;
            });

            this.AlterationCategoryCB.ItemsSource = scenarioCollectionProvider.AlterationCategories;
        }
    }
}
