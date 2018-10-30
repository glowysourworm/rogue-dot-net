using Prism.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class LevelView : UserControl
    {
        [ImportingConstructor]
        public LevelView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.Loaded += (obj, e) =>
            {
                this.TheLevelCanvas.InitializeEvents(eventAggregator);
                this.SubPanel.InitializeEvents(eventAggregator);
            };
        }
   }
}
