using Rogue.NET.ScenarioEditor.Views.Controls;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class Doodad : UserControl
    {
        [ImportingConstructor]
        public Doodad(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                // TODO:ALTERATION
                //this.AutomaticSpellCB.ItemsSource = configuration.MagicSpells;
                //this.InvokedSpellCB.ItemsSource = configuration.MagicSpells;
                this.CharacterClassCB.ItemsSource = configuration.CharacterClasses;
            });
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var view = new SymbolEditor();
            view.DataContext = (this.DataContext as DoodadTemplateViewModel).SymbolDetails;
            view.WindowMode = true;
            view.Width = 600;

            DialogWindowFactory.Show(view, "Rogue Symbol Editor");
        }
    }
}
