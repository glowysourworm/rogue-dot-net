using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Views.Gadgets
{
    public partial class EnemyScopeGadget : UserControl
    {
        Grid _propertyGrid = null;
        Grid _placeholderGrid = null;

        public EnemyScopeGadget()
        {
            InitializeComponent();

            _propertyGrid = this.PropertyGrid;
            _placeholderGrid = new Grid();
            _placeholderGrid.Children.Add(new TextBlock()
            {
                Foreground = Brushes.White,
                Text = "Target Enemy to Show Attributes",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(8),
                FontSize = 16
            });

            this.MainBorder.Child = _placeholderGrid;
        }

        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<EnemyTargetedEvent>().Subscribe((evt) =>
            {
                this.DataContext = evt.TargetedEnemy;
                this.MainBorder.Child = evt.TargetedEnemy == null ? _placeholderGrid : _propertyGrid;
            });
        }
    }
}
