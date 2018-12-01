using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class LevelView : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public LevelView(GameViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = viewModel;

            InitializeComponent();

            this.HelpButton.Click += (sender, e) =>
            {
                eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
                {
                    Type = DialogEventType.Help
                });
            };
        }

        private void CenterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void UpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DownButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void LeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void RightButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CollapseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
