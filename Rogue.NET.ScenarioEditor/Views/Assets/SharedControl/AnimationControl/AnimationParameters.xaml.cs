using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl
{
    [Export]
    public partial class AnimationParameters : UserControl
    {
        [ImportingConstructor]
        public AnimationParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.OutlineCB.ItemsSource = configuration.BrushTemplates;
                this.ColorCB.ItemsSource = configuration.BrushTemplates;
            });

            this.DataContextChanged += AnimationParameters_DataContextChanged;
        }

        private void AnimationParameters_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var animationTemplate = e.NewValue as AnimationTemplate;
            if (animationTemplate == null)
                return;

            // Set visibility for extended parameters
            switch (animationTemplate.Type)
            {
                case AnimationType.ChainSelfToTargetsInRange:
                case AnimationType.ProjectileSelfToTarget:
                case AnimationType.ProjectileTargetToSelf:
                case AnimationType.ProjectileSelfToTargetsInRange:
                case AnimationType.ProjectileTargetsInRangeToSelf:
                case AnimationType.AuraSelf:
                case AnimationType.AuraTarget:
                case AnimationType.ScreenBlink:
                    this.ChildCountGrid.Visibility = Visibility.Collapsed;
                    this.ErradicityGrid.Visibility = Visibility.Collapsed;
                    this.RadiusFromFocusGrid.Visibility = Visibility.Collapsed;
                    this.SpiralRateGrid.Visibility = Visibility.Collapsed;
                    this.RoamRadiusGrid.Visibility = Visibility.Collapsed;
                    break;
                case AnimationType.BubblesSelf:
                case AnimationType.BubblesTarget:
                case AnimationType.BubblesScreen:
                case AnimationType.BarrageSelf:
                case AnimationType.BarrageTarget:
                    this.ChildCountGrid.Visibility = Visibility.Visible;
                    this.ErradicityGrid.Visibility = Visibility.Visible;
                    this.RadiusFromFocusGrid.Visibility = Visibility.Visible;
                    this.SpiralRateGrid.Visibility = Visibility.Collapsed;
                    this.RoamRadiusGrid.Visibility = Visibility.Visible;
                    break;
                case AnimationType.SpiralSelf:
                case AnimationType.SpiralTarget:
                    this.ChildCountGrid.Visibility = Visibility.Visible;
                    this.ErradicityGrid.Visibility = Visibility.Visible;
                    this.RadiusFromFocusGrid.Visibility = Visibility.Visible;
                    this.SpiralRateGrid.Visibility = Visibility.Visible;
                    this.RoamRadiusGrid.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }
    }
}
