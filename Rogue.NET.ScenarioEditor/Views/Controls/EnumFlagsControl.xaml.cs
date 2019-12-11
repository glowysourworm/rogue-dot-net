﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(EnumFlagsControl))]
    public partial class EnumFlagsControl : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EnumFlagsControl), new PropertyMetadata("Header", new PropertyChangedCallback(OnHeaderChanged)));

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumFlagsControl), new PropertyMetadata(new PropertyChangedCallback(OnTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue", typeof(object), typeof(EnumFlagsControl), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public object EnumValue
        {
            get { return (object)GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }

        public class EnumItem : NotifyViewModel
        {
            string _name;
            string _displayName;
            string _description;
            object _value;
            bool _isChecked;

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
            public object Value
            {
                get { return _value; }
                set { this.RaiseAndSetIfChanged(ref _value, value); }
            }
            public bool IsChecked
            {
                get { return _isChecked; }
                set { this.RaiseAndSetIfChanged(ref _isChecked, value); }
            }
        }

        bool _initializing = false;

        [ImportingConstructor]
        public EnumFlagsControl()
        {
            InitializeComponent();
        }

        protected void CreateItemsSource()
        {
            _initializing = true;

            var enumItems = new ObservableCollection<EnumItem>();

            foreach (Enum enumValue in Enum.GetValues(this.EnumType))
            {
                var enumName = Enum.GetName(this.EnumType, enumValue);

                enumItems.Add(new EnumItem()
                {
                    Name = enumName,
                    Value = enumValue,
                    Description = enumValue.GetAttribute<DisplayAttribute>()?.Description ?? "",
                    DisplayName = enumValue.GetAttribute<DisplayAttribute>()?.Name ?? "",
                    IsChecked = this.EnumValue != null ? Enum.GetName(this.EnumType, this.EnumValue) == enumName : false
                });
            }

            this.EnumItemsControl.ItemsSource = enumItems;

            _initializing = false;
        }
        protected void UpdateItemsSource()
        {
            _initializing = true;

            var items = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItem>;

            // Enum Flags are set using the bitwise & operator
            if (items != null)
                items.ForEach(item => item.IsChecked = ((int)item.Value & (int)this.EnumValue) != 0);

            _initializing = false;
        }
        protected void UpdateValue()
        {
            var items = this.EnumItemsControl.ItemsSource as ObservableCollection<EnumItem>;

            // EnumValue is set using the bitwise | operator
            var enumValue = 0;
            if (items != null)
                items.Where(item => item.IsChecked)
                     .ForEach(item =>
                     {
                         enumValue = enumValue | (int)item.Value;
                     });

            this.EnumValue = Enum.ToObject(this.EnumType, enumValue);
        }

        // Update the items source when value changed
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null &&
                control.EnumValue != null)
                control.UpdateItemsSource();
        }

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null)
                control.CreateItemsSource();
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EnumFlagsControl;
            if (control != null &&
                e.NewValue != null)
                control.EnumGroupBox.Header = (string)e.NewValue;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initializing)
                UpdateValue();
        }
    }
}
