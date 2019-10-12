using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class AlterationGeneral : UserControl
    {
        [ImportingConstructor]
        public AlterationGeneral(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
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
