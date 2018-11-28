using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.View
{
    [Export]
    public partial class EnumComboBox : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            "EnumType", 
            typeof(Type), 
            typeof(EnumComboBox),
            new PropertyMetadata(new PropertyChangedCallback(OnEnumTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty = DependencyProperty.Register(
            "EnumValue", 
            typeof(object), 
            typeof(EnumComboBox),
            new PropertyMetadata(new PropertyChangedCallback(OnEnumValueChanged)));

        public static readonly RoutedEvent EnumValueChangedEvent = EventManager.RegisterRoutedEvent(
            "EnumValueChanged", 
            RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), 
            typeof(EnumComboBox));

        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); SetItemSource(); }
        }
        public object EnumValue
        {
            get { return GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }
        public event RoutedEventHandler EnumValueChanged
        {
            add { AddHandler(EnumValueChangedEvent, value); }
            remove { RemoveHandler(EnumValueChangedEvent, value); }
        }

        [ImportingConstructor]
        public EnumComboBox()
        {
            InitializeComponent();

            this.TheComboBox.SelectionChanged += (obj, e) =>
            {
                if (e.AddedItems.Count > 0)
                {
                    this.EnumValue = e.AddedItems[0];
                }
            };

            RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, this));
        }
        protected virtual void SetItemSource()
        {
            if (this.EnumType.IsEnum)
                this.TheComboBox.ItemsSource = Enum.GetValues(this.EnumType);
        }

        private static void OnEnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EnumComboBox;
            if (instance != null)
                instance.SetItemSource();
        }
        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EnumComboBox;
            if (instance != null)
            {
                instance.TheComboBox.SelectedItem = e.NewValue;
                instance.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, instance));
            }
        }
    }
}

