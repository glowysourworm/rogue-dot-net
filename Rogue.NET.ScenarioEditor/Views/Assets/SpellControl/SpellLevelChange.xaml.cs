using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SpellControl
{
    public partial class SpellLevelChange : UserControl
    {
        public SpellLevelChange()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var model = this.DataContext as SpellTemplate;
            if (model != null)
                model.OtherEffectType = (AlterationMagicEffectType)radioButton.Tag;
        }
    }
}
