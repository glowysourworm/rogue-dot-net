using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class SymbolEditor : UserControl
    {
        public bool WindowMode
        {
            get { return this.ButtonGrid.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    this.ButtonGrid.Visibility = Visibility.Visible;
                else
                    this.ButtonGrid.Visibility = Visibility.Collapsed; 
            }
        }

        public SymbolEditor()
        {
            InitializeComponent();
        }

        private void CharacterSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new CharacterMap();
            var viewModel = this.DataContext as DungeonObjectTemplateViewModel;
            view.DataContext = this.DataContext;
            view.Width = 600;

            if (DialogWindowFactory.Show(view, "Rogue UTF-8 Character Map"))
                viewModel.SymbolDetails.CharacterSymbol = view.SelectedCharacter;
        }
    }
}
