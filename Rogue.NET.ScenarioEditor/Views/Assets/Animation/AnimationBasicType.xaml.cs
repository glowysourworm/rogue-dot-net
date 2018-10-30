using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    [Export]
    public partial class AnimationBasicType : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationBasicType()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get 
            {
                foreach (RadioButton radioButton in this.RadioStack.Children)
                {
                    if (radioButton.IsChecked.HasValue && radioButton.IsChecked.Value)
                    {
                        var type = "Rogue.NET.ScenarioEditor.Views.Assets.Animation.Animation" + radioButton.Tag.ToString();
                        return Type.GetType(type);
                    }
                }

                return typeof(AnimationBasicType);
            }
        }
        public void Inject(IWizardViewModel viewModel, object model)
        {
            _containerViewModel = viewModel;

            this.DataContext = model;

            var animation = model as AnimationTemplate;
            if (animation == null)
                return;

            switch (animation.Type)
            {
                case AnimationType.AuraSelf:
                case AnimationType.AuraTarget:
                    this.AuraRB.IsChecked = true;
                    break;
                case AnimationType.BarrageSelf:
                case AnimationType.BarrageTarget:
                    this.BarrageRB.IsChecked = true;
                    break;
                case AnimationType.BubblesScreen:
                case AnimationType.BubblesSelf:
                case AnimationType.BubblesTarget:
                    this.BubblesRB.IsChecked = true;
                    break;
                case AnimationType.ProjectileSelfToTarget:
                case AnimationType.ProjectileSelfToTargetsInRange:
                case AnimationType.ProjectileTargetsInRangeToSelf:
                case AnimationType.ProjectileTargetToSelf:
                default:
                    this.ProjectileRB.IsChecked = true;
                    break;
                case AnimationType.ScreenBlink:
                    this.BlinkRB.IsChecked = true;
                    break;
                case AnimationType.SpiralSelf:
                case AnimationType.SpiralTarget:
                    this.SpiralRB.IsChecked = true;
                    break;
            }
        }

        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var typeName = radioButton.Name.Replace("RB", "");
            var enumNames = Enum.GetNames(typeof(AnimationType));
            var matchedName = enumNames.First(n => n.Contains(typeName));
            var enumValue = (AnimationType)Enum.Parse(typeof(AnimationType), matchedName);

            var model = this.DataContext as AnimationTemplate;
            model.Type = enumValue;
        }
    }
}
