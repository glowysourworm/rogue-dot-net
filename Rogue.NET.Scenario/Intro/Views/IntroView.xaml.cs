using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using System.Windows.Media.Animation;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Scenario.Intro.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class IntroView : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        Timer _timer = null;
        Storyboard _currentStoryboard = null;
        int _ctr = 0;

        [ImportingConstructor]
        public IntroView(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += IntroDisplay_Loaded;
        }

        private void IntroDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTimer();
        }
        public void Initialize(object[] args)
        {
            InitializeTimer();
        }
        private void InitializeTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _ctr = 0;
            _timer = new Timer();
            _timer.Interval = 2000;
            _timer.Elapsed += new ElapsedEventHandler((obj, e) => 
                {
                    this.Dispatcher.BeginInvoke(new EventHandler<ElapsedEventArgs>(ExecuteOnTimer), new object[] { obj, e });
                });
            _timer.Enabled = true;
            _timer.Start();
        }
        private void ExecuteOnTimer(object sender, ElapsedEventArgs e)
        {
            if (_currentStoryboard != null)
            {
                _currentStoryboard.Stop();
                _currentStoryboard.Remove();
                _currentStoryboard = null;
            }
            if (_ctr == 0)
            {
                _currentStoryboard = this.Resources["RabbitHopLeftStoryboard"] as Storyboard;
                _currentStoryboard.AutoReverse = true;
                _currentStoryboard.Begin();
            }
            else if (_ctr == 1)
            {
                _currentStoryboard = this.Resources["RabbitHopRightStoryboard"] as Storyboard;
                _currentStoryboard.AutoReverse = true;
                _currentStoryboard.Begin();
            }
            else if (_ctr == 2)
            {
                _currentStoryboard = this.Resources["RabbitHopLeftStoryboard"] as Storyboard;
                _currentStoryboard.AutoReverse = true;
                _currentStoryboard.Begin();
            }
            else if (_ctr == 3)
            {
                Storyboard s = this.Resources["RabbitAttackStoryboard"] as Storyboard;
                s.FillBehavior = FillBehavior.HoldEnd;
                s.Begin();
            }
            else if (_ctr == 4)
            {
                //Change Smiley1's eyes to Dead X_X
                this.Smiley1.SmileyExpression = SmileyExpression.Dead;
                this.Smiley2.SmileyExpression = SmileyExpression.FreakedOut;
                this.Smiley3.SmileyExpression = SmileyExpression.FreakedOut;
                this.Smiley4.SmileyExpression = SmileyExpression.FreakedOut;
                this.Smiley1.SmileyColor = Colors.Red;

                Storyboard s = this.Resources["RunawayStoryboard"] as Storyboard;
                s.FillBehavior = FillBehavior.HoldEnd;
                s.Begin();
            }
            else if (_ctr == 5)
            {
                Storyboard s = this.Resources["TitleFadeInStoryboard"] as Storyboard;
                s.FillBehavior = FillBehavior.HoldEnd;
                s.Begin();
            }
            else
            {
                SetFinished();
                return;
            }
            _ctr++;
        }
        private void SetFinished()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            if (_currentStoryboard != null)
            {
                _currentStoryboard.Stop();
                _currentStoryboard.Remove();
                _currentStoryboard = null;
            }

            _eventAggregator.GetEvent<IntroFinishedEvent>().Publish();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            SetFinished();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            SetFinished();
        }
    }
}
