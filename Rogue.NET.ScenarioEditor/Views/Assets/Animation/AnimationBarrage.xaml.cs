using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    public partial class AnimationBarrage : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationBarrage()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(AnimationParametersExtended); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;
            this.DataContext = model;

            if (model == null)
                return;

            switch ((model as AnimationTemplate).Type)
            {
                case AnimationType.BarrageSelf:
                    this.SelfRB.IsChecked = true;
                    break;
                default:
                    this.TargetRB.IsChecked = true;
                    break;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var model = this.DataContext as AnimationTemplate;
            if (radioButton.Name.Contains("Self"))
                model.Type = AnimationType.BarrageSelf;
            else
                model.Type = AnimationType.BarrageTarget;
        }
    }
}
