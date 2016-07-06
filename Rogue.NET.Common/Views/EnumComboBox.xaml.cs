using System;
using System.Collections;
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

namespace Rogue.NET.Common.Views
{
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

        public EnumComboBox()
        {
            InitializeComponent();

            //this.DataContext = this;

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

