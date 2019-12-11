using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using System.ComponentModel.DataAnnotations;
using Rogue.NET.Common.ViewModel;

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

        public class EnumItem : NotifyViewModel
        {
            object _value;
            string _name;
            string _displayName;
            string _description;

            public object Value
            {
                get { return _value; }
                set { this.RaiseAndSetIfChanged(ref _value, value); }
            }
            public string Name
            {
                get { return _name; }
                set { this.RaiseAndSetIfChanged(ref _name, value); }
            }
            public string DisplayName
            {
                get { return _displayName; }
                set { this.RaiseAndSetIfChanged(ref _displayName, value); }
            }
            public string Description
            {
                get { return _description; }
                set { this.RaiseAndSetIfChanged(ref _description, value); }
            }

            public EnumItem() { }
        }

        [ImportingConstructor]
        public EnumComboBox()
        {
            InitializeComponent();

            this.TheComboBox.SelectionChanged += (obj, e) =>
            {
                if (e.AddedItems.Count > 0)
                {
                    this.EnumValue = (e.AddedItems[0] as EnumItem).Value;
                }
            };

            RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, this));
        }
        protected virtual void SetItemSource()
        {
            if (this.EnumType != null &&
                this.EnumType.IsEnum)
            {
                var itemSource = new List<EnumItem>();

                foreach (Enum enumValue in Enum.GetValues(this.EnumType))
                {
                    var item = new EnumItem();
                    var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

                    item.Value = enumValue;
                    item.Name = Enum.GetName(this.EnumType, enumValue);
                    item.DisplayName = displayAttribute?.Name ?? item.Name;
                    item.Description = displayAttribute?.Description ?? null;

                    itemSource.Add(item);
                }

                this.TheComboBox.ItemsSource = itemSource;
            }
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

            if (instance != null &&
                e.NewValue != null)
            {
                var itemSource = instance.TheComboBox.ItemsSource as IEnumerable<EnumItem>;

                if (itemSource != null)
                {
                    // NOTE*** Matching by enum name because value didn't show a match (!!!) (Not sure why)
                    instance.TheComboBox.SelectedItem = itemSource.FirstOrDefault(item => item.Name == e.NewValue.ToString());
                    instance.RaiseEvent(new RoutedEventArgs(EnumValueChangedEvent, instance));
                }
            }
        }
    }
}

