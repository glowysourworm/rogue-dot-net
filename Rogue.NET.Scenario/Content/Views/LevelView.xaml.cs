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
        [ImportingConstructor]
        public LevelView(GameViewModel viewModel, IEventAggregator eventAggregator)
        {
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
   }
}
