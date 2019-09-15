using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class BrushSelectorControl : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public BrushSelectorControl(IRogueEventAggregator eventAggregator,
                                    IScenarioCollectionProvider scenarioCollectionProvider)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });
        }

        private void Initialize(IScenarioCollectionProvider scenarioCollectionProvider)
        {
            var itemsSource = new List<BrushTemplateViewModel>();
            var names = new List<string>();

            // Have to generate unique names for the brushes (TBD - Maybe use the asset name if it's not too much trouble)
            foreach (var brush in scenarioCollectionProvider.Brushes)
            {
                brush.Name = NameGenerator.Get(names, "Brush");
                names.Add(brush.Name);
                itemsSource.Add(brush);
            }

            this.BrushCB.ItemsSource = itemsSource;
        }

        private void CopyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedBrush = this.BrushCB.SelectedItem as BrushTemplateViewModel;

            if (selectedBrush != null)
            {
                // Publish event with the selected brush
                _eventAggregator.GetEvent<BrushCopyEvent>()
                                .Publish(selectedBrush.DeepClone());
            }
        }
    }
}

