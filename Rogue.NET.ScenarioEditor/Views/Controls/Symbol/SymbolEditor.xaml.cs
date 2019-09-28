using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
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

        [ImportingConstructor]
        public SymbolEditor()
        {
            InitializeComponent();
        }

        private void CharacterSymbolButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new CharacterMap();
            view.DataContext = this.DataContext;

            // Can be shown as a dialog
            DialogWindowFactory.Show(view, "Rogue UTF-8 Character Map");
        }
    }
}
