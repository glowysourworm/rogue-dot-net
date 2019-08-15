using Rogue.NET.Common.Extension;
using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class RadioButtonEnumControl : UserControl
    {
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", 
                                        typeof(Type), 
                                        typeof(RadioButtonEnumControl),
                                        new PropertyMetadata(new PropertyChangedCallback(OnEnumTypeChanged)));

        public static readonly DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue",
                                        typeof(object),
                                        typeof(RadioButtonEnumControl),
                                        new PropertyMetadata(new PropertyChangedCallback(OnEnumValueChanged)));

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
            string _enumName;
            string _displayName;
            string _description;
            bool _isChecked;

            public string EnumName
            {
                get { return _enumName; }
                set { this.RaiseAndSetIfChanged(ref _enumName, value); }
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
            public bool IsChecked
            {
                get { return _isChecked; }
                set { this.RaiseAndSetIfChanged(ref _isChecked, value); }
            }
        }

        public RadioButtonEnumControl()
        {
            InitializeComponent();
        }

        private static void OnEnumTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as RadioButtonEnumControl;
            var type = e.NewValue as Type;

            // Update controls for the new enum type
            if (control != null &&
                type != null &&
                type.IsEnum)
            {
                control.EnumList
                       .ItemsSource = type.GetMembers()
                                          .Where(x => x.GetCustomAttributes(typeof(DisplayAttribute), false).Any())
                                          .Select(x => new EnumItem()
                                          {
                                              EnumName = x.Name,
                                              Description = x.GetCustomAttribute<DisplayAttribute>().Description,
                                              DisplayName = x.GetCustomAttribute<DisplayAttribute>().Name,
                                              IsChecked = control.EnumValue != null ? Enum.GetName(control.EnumType, control.EnumValue) == x.Name
                                                                                    : false
                                          })
                                          .Actualize();
            }
        }

        private static void OnEnumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadioButtonEnumControl;
            var itemsSource = control.EnumList.ItemsSource as IEnumerable<EnumItem>;
            var enumValue = e.NewValue;

            if (itemsSource != null &&
                control != null)
            {
                foreach (var item in itemsSource)
                    item.IsChecked = enumValue == null ? 
                                        false :
                                        item.EnumName == Enum.GetName(control.EnumType, enumValue);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var itemsSource = this.EnumList.ItemsSource as IEnumerable<EnumItem>;
            var radioButton = sender as RadioButton;

            if (itemsSource != null &&
                radioButton != null)
            {
                // Enum Name for the selected radio button
                var enumName = radioButton.Tag.ToString();

                // Enum Type Values
                var values = Enum.GetValues(this.EnumType);

                for (int i=0;i<values.Length;i++)
                {
                    if (Enum.GetName(this.EnumType, values.GetValue(i)) == enumName)
                        this.EnumValue = values.GetValue(i);
                }
            }
        }
    }
}
