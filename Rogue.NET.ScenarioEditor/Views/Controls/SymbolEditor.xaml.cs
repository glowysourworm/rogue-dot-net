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
            var window = new Window();
            var symbolCtrl = new CharacterMap();

            window.Content = symbolCtrl;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            if ((bool)window.ShowDialog())
                this.CharacterTB.Text = symbolCtrl.SelectedCharacter;
        }
    }
}
