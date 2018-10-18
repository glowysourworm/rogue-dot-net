using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Model;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Views
{
    /// <summary>
    /// Interaction logic for LevelView.xaml
    /// </summary>
    public partial class LevelView : UserControl
    {
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
