using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [Export]
    public partial class ListBuilder : UserControl
    {
        public static readonly DependencyProperty SourceItemsSourceProperty =
            DependencyProperty.Register("SourceItemsSource", typeof(IEnumerable), typeof(ListBuilder));

        public static readonly DependencyProperty DestinationItemsSourceProperty =
            DependencyProperty.Register("DestinationItemsSource", typeof(IEnumerable), typeof(ListBuilder));

        public IEnumerable SourceItemsSource
        {
            get { return (IEnumerable)GetValue(SourceItemsSourceProperty); }
            set { SetValue(SourceItemsSourceProperty, value); }
        }
        public IEnumerable DestinationItemsSource
        {
            get { return (IEnumerable)GetValue(DestinationItemsSourceProperty); }
            set { SetValue(DestinationItemsSourceProperty, value); }
        }

        public event EventHandler<object> AddEvent;
        public event EventHandler<object> RemoveEvent;

        [ImportingConstructor]
        public ListBuilder()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.SourceLB.SelectedItems)
            {
                if (AddEvent != null)
                    AddEvent(this, item);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = this.DestinationLB.SelectedItems.Count - 1; i >= 0; i--)
            {
                if (RemoveEvent != null)
                    RemoveEvent(this, this.DestinationLB.SelectedItems[i]);
            }
        }
    }
}
