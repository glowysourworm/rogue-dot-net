using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    public partial class AnimationAura : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationAura()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(AnimationParametersMain); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;
            this.DataContext = model;

            if (model == null)
                return;

            switch ((model as AnimationTemplate).Type)
            {
                case AnimationType.AuraSelf:
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
                model.Type = AnimationType.AuraSelf;

            else
                model.Type = AnimationType.AuraTarget;
        }
    }
}
