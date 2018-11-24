using Prism.Events;
using Rogue.NET.Scenario.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Rogue.NET.Scenario.Outro
{
    [Export]
    public partial class OutroDisplay : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        bool _fadeOutStarted = false;

        [ImportingConstructor]
        public OutroDisplay(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();
        }

        private void FadeOut()
        {
            _fadeOutStarted = true;

            var opacityAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(3)));

            var clock = opacityAnimation.CreateClock();

            this.ApplyAnimationClock(OpacityProperty, clock);

            clock.Completed += (obj, e) =>
            {
                _eventAggregator.GetEvent<OutroFinishedEvent>().Publish();
            };

            clock.Controller.Begin();
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!_fadeOutStarted)
                    FadeOut();
            }
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_fadeOutStarted)
                FadeOut();
        }
    }
}
