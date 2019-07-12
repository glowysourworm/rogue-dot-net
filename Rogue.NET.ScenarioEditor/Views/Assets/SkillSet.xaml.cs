using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class SkillSet : UserControl
    {
        [ImportingConstructor]
        public SkillSet(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.SkillSetBuilder.SourceItemsSource = configuration.MagicSpells;
            });

            this.SkillSetBuilder.AddEvent += SkillSetBuilder_AddEvent;
            this.SkillSetBuilder.RemoveEvent += SkillSetBuilder_RemoveEvent;
        }

        private void SkillSetBuilder_AddEvent(object sender, object e)
        {
            var viewModel = this.DataContext as SkillSetTemplateViewModel;
            if (viewModel == null)
                return;

            var alteration = e as SpellTemplateViewModel;

            viewModel.Skills.Add(new SkillTemplateViewModel()
            {
                Name = alteration.DisplayName,
                Alteration = alteration                
            });
        }
        private void SkillSetBuilder_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DataContext as SkillSetTemplateViewModel;
            if (viewModel == null)
                return;

            viewModel.Skills.Remove(e as SkillTemplateViewModel);
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var view = new SymbolEditor();
            view.DataContext = (this.DataContext as SkillSetTemplateViewModel).SymbolDetails;
            view.WindowMode = true;
            view.Width = 600;

            DialogWindowFactory.Show(view, "Rogue Symbol Editor");
        }
    }
}
