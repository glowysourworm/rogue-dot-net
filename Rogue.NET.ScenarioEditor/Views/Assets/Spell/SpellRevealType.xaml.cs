using Rogue.NET.Common;
using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets.Spell
{
    public partial class SpellRevealType : UserControl, IWizardPage
    {
        public SpellRevealType()
        {
            InitializeComponent();
        }
        public Type NextPage
        {
            get { return typeof(SpellParameters); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var template = model as SpellTemplate;
            foreach (RadioButton button in this.RadioStack.Children)
            {
                if (Enum.GetName(typeof(AlterationMagicEffectType), button.Tag) == template.Type.ToString())
                    button.IsChecked = true;
            }
        }

        IWizardViewModel _containerViewModel;


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var model = this.DataContext as SpellTemplate;
            if (model != null)
                model.OtherEffectType = (AlterationMagicEffectType)radioButton.Tag;
        }
    }
}
