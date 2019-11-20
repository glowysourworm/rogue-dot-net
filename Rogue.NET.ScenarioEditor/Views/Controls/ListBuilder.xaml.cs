using Prism.Events;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
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

        public static readonly DependencyProperty SourceSelectedItemProperty =
            DependencyProperty.Register("SourceSelectedItem", typeof(object), typeof(ListBuilder));

        public static readonly DependencyProperty DestinationSelectedItemProperty =
            DependencyProperty.Register("DestinationSelectedItem", typeof(object), typeof(ListBuilder));

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
        public object SourceSelectedItem
        {
            get { return (object)GetValue(SourceSelectedItemProperty); }
            set { SetValue(SourceSelectedItemProperty, value); }
        }
        public object DestinationSelectedItem
        {
            get { return (object)GetValue(DestinationSelectedItemProperty); }
            set { SetValue(DestinationSelectedItemProperty, value); }
        }

        public event EventHandler<object> AddEvent;
        public event EventHandler<object> RemoveEvent;

        public event SelectionChangedEventHandler SourceSelectionChanged;
        public event SelectionChangedEventHandler DestinationSelectionChanged;

        [ImportingConstructor]
        public ListBuilder()
        {
            InitializeComponent();

            this.SourceLB.SelectionChanged += (sender, e) =>
            {
                if (this.SourceSelectionChanged != null)
                    this.SourceSelectionChanged(this, e);
            };

            this.DestinationLB.SelectionChanged += (sender, e) =>
            {
                if (this.DestinationSelectionChanged != null)
                    this.DestinationSelectionChanged(this, e);
            };
        }

        private void TryAdd(object item)
        {
            if (this.DestinationItemsSource is IList)
            {
                (this.DestinationItemsSource as IList).Add(item);
            }
        }
        private void TryRemove(object item)
        {
            if (this.SourceItemsSource is IList)
            {
                (this.SourceItemsSource as IList).Add(item);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.SourceLB.SelectedItems)
            {
                if (AddEvent != null)
                    AddEvent(this, item);

                else
                    TryAdd(item);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = this.DestinationLB.SelectedItems.Count - 1; i >= 0; i--)
            {
                if (RemoveEvent != null)
                    RemoveEvent(this, this.DestinationLB.SelectedItems[i]);

                else
                    TryRemove(this.DestinationLB.SelectedItems[i]);
            }
        }
    }
}
