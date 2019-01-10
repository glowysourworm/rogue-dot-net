using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.Common.View
{
    [Export]
    public partial class ValueBar : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ValueBar), 
                new PropertyMetadata(0D, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty AllowNegativeProperty =
            DependencyProperty.Register("AllowNegative", typeof(bool), typeof(ValueBar),
                new PropertyMetadata(0D, new PropertyChangedCallback(OnModeChanged)));
        #endregion

        #region Properties
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public bool AllowNegative
        {
            get { return (bool)GetValue(AllowNegativeProperty); }
            set { SetValue(AllowNegativeProperty, value); }
        }
        #endregion

        public ValueBar()
        {
            InitializeComponent();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valueBar = d as ValueBar;

            // TODO
        }
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var valueBar = d as ValueBar;

            // TODO
        }
    }
}
